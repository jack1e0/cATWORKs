using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Newtonsoft.Json;
using Firebase.Database;
using System.Threading.Tasks;

public class StatsDisplay : MonoBehaviour
{
    [SerializeField] private Image XPFill;
    [SerializeField] private TMP_Text lvlText;
    [SerializeField] private Image happyFill;

    [SerializeField] private int currXP, maxXP, currLvl;
    [SerializeField] private int currHappy, maxHappy;
    [SerializeField] private int reward;

    [SerializeField] private TMP_Text textDisplay;
    private AudioSource aud;

    private void Start()
    {
        aud = GetComponent<AudioSource>();
        this.currXP = SceneTransition.instance.user.currXP;
        this.currHappy = SceneTransition.instance.user.currHappiness;
        this.currLvl = SceneTransition.instance.user.level;
        this.maxXP = SceneTransition.instance.user.maxXP;

        lvlText.text = currLvl.ToString();
        SetFill(XPFill, (float)currXP, (float)maxXP);
        SetFill(happyFill, (float)currHappy, (float)maxHappy);


        StatsManager.instance.OnXPChange += HandleXPChange;
        StatsManager.instance.onHappyChange += HandleHappyChange;
    }

    private void OnDisable()
    {
        StatsManager.instance.OnXPChange -= HandleXPChange;
        StatsManager.instance.onHappyChange -= HandleHappyChange;
    }

    private async void HandleXPChange(int amt)
    {
        StartCoroutine(ChangeFill(XPFill, (float)currXP, (float)amt, (float)this.maxXP));

        if (currXP + amt < maxXP)
        {
            currXP += amt;
        }

        await UpdateXP();
    }

    private async void HandleHappyChange(double amt)
    {
        if (amt > 0)
        {
            if (currHappy < maxHappy)
            {
                StatsManager.instance.happinessFull = false;
                StartCoroutine(ChangeFill(happyFill, (float)currHappy, (float)amt, (float)this.maxHappy));
                currHappy += Mathf.Min((int)amt, maxHappy - currHappy);
                StatsManager.instance.happinessPercent = (float)currHappy / (float)maxHappy;
            }
            else
            {
                StatsManager.instance.happinessFull = true;
            }
            SceneTransition.instance.user.prevExitTime = System.DateTime.Now;
            await UpdatePrevTime();

        }
        else
        { // TODO: not completely implemented yet
            if (currHappy + (int)amt >= 0)
            {
                currHappy += (int)amt;
            }
            else
            {
                currHappy = 0;
            }
            SetFill(happyFill, currHappy, maxHappy);
            StatsManager.instance.happinessPercent = (float)currHappy / (float)maxHappy;
        }

        await UpdateHappiness();
    }

    IEnumerator ChangeFill(Image fill, float init, float amt, float max)
    {
        aud.Play();
        float i = 0;
        while (i <= 1)
        {
            fill.fillAmount = Mathf.Lerp(init / max, Mathf.Min(1, (init + amt) / max), i);
            i += 0.05f;
            yield return null;
        }

        if (fill == XPFill && init + amt >= max)
        {
            yield return new WaitForSeconds(0.1f);
            LevelUp();
        }
    }

    async void LevelUp()
    {
        SetFill(XPFill, 0, maxXP);
        currLvl++;
        lvlText.text = currLvl.ToString();

        currXP = 0;
        maxXP += 2;
        if (reward <= 20)
        {
            reward += 2;
        }

        string congrats;

        if (currLvl == 2)
        {
            SceneTransition.instance.user.unlockedAccessoryDict.Add("SPROUT", 1);
            congrats = $"Levelled up, gained {reward} catfood!/nP.S. Check the closet :D";
            await UpdateUnlockedAccessories();
        }

        if (currLvl == 5)
        {
            SceneTransition.instance.user.unlockedAccessoryDict.Add("GLASSES", 1);
            congrats = $"Levelled up, gained {reward} catfood!/nP.S. Check the closet :D";
            await UpdateUnlockedAccessories();
        }

        if (currLvl == 4 || currLvl == 7 || currLvl == 10)
        {
            SceneTransition.instance.user.growth = Mathf.Min(2, SceneTransition.instance.user.growth + 1);
            Debug.Log("growth: " + SceneTransition.instance.user.growth);
            await UpdateGrowth();
            RoomSceneManager.instance.catControl.Initialize();
            congrats = $"Levelled up! Your cat has grown as well! Gained {reward} catfood";
        }
        else
        {
            congrats = $"Congrats! You levelled up! Gained {reward} catfood";
        }
        StartCoroutine(RoomSceneManager.instance.DisplayNotifs(congrats));
        CatfoodManager.instance.IncreaseCatfood(reward);
        await UpdateXP();

    }

    void SetFill(Image fill, float amt, float max)
    {
        fill.fillAmount = amt / max;
    }

    private async Task UpdateHappiness()
    {
        SceneTransition.instance.user.currHappiness = currHappy;

        string currHap = JsonConvert.SerializeObject(SceneTransition.instance.user.currHappiness);

        DatabaseReference DBreference = FirebaseDatabase.DefaultInstance.RootReference;
        await DBreference.Child("users").Child(SceneTransition.instance.user.userId).Child("currHappiness").SetValueAsync(currHap);
    }

    private async Task UpdateXP()
    {
        SceneTransition.instance.user.currXP = currXP;
        SceneTransition.instance.user.maxXP = maxXP;
        SceneTransition.instance.user.level = currLvl;

        string xp = JsonConvert.SerializeObject(SceneTransition.instance.user.currXP);
        string maxxp = JsonConvert.SerializeObject(SceneTransition.instance.user.maxXP);
        string lvl = JsonConvert.SerializeObject(SceneTransition.instance.user.level);

        DatabaseReference DBreference = FirebaseDatabase.DefaultInstance.RootReference;
        await DBreference.Child("users").Child(SceneTransition.instance.user.userId).Child("currXP").SetValueAsync(xp);
        await DBreference.Child("users").Child(SceneTransition.instance.user.userId).Child("maxXP").SetValueAsync(maxxp);
        await DBreference.Child("users").Child(SceneTransition.instance.user.userId).Child("level").SetValueAsync(lvl);
    }

    private async Task UpdateGrowth()
    {
        string grow = JsonConvert.SerializeObject(SceneTransition.instance.user.growth);

        DatabaseReference DBreference = FirebaseDatabase.DefaultInstance.RootReference;
        await DBreference.Child("users").Child(SceneTransition.instance.user.userId).Child("growth").SetValueAsync(grow);
    }

    private async Task UpdatePrevTime()
    {
        string prev = JsonConvert.SerializeObject(SceneTransition.instance.user.prevExitTime);

        DatabaseReference DBreference = FirebaseDatabase.DefaultInstance.RootReference;
        await DBreference.Child("users").Child(SceneTransition.instance.user.userId).Child("prevExitTime").SetValueAsync(prev);
    }

    private async Task UpdateUnlockedAccessories()
    {
        string unlocked = JsonConvert.SerializeObject(SceneTransition.instance.user.unlockedAccessoryDict);

        DatabaseReference DBreference = FirebaseDatabase.DefaultInstance.RootReference;
        await DBreference.Child("users").Child(SceneTransition.instance.user.userId).Child("unlockedAccessoryDict").SetValueAsync(unlocked);
    }
}
