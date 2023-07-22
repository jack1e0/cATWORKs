using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Newtonsoft.Json;
using Firebase.Database;
using System.Threading.Tasks;

public class StatsDisplay : MonoBehaviour {
    [SerializeField] private Image XPFill;
    [SerializeField] private TMP_Text lvlText;
    [SerializeField] private Image happyFill;

    [SerializeField] private int currXP, maxXP, currLvl;
    [SerializeField] private int currHappy, maxHappy;
    [SerializeField] private int reward;

    [SerializeField] private TMP_Text textDisplay;

    private void Start() {
        this.currXP = StatsManager.instance.currXP;
        this.currHappy = StatsManager.instance.currHappy;
        this.currLvl = StatsManager.instance.currLvl;

        lvlText.text = currLvl.ToString();
        SetFill(XPFill, (float)currXP, (float)maxXP);
        SetFill(happyFill, (float)currHappy, (float)maxHappy);


        StatsManager.instance.OnXPChange += HandleXPChange;
        StatsManager.instance.onHappyChange += HandleHappyChange;
    }

    private void OnDisable() {
        StatsManager.instance.SetStats(this.currXP, this.maxXP, this.currHappy, this.currLvl);

        StatsManager.instance.OnXPChange -= HandleXPChange;
        StatsManager.instance.onHappyChange -= HandleHappyChange;
    }

    private async void HandleXPChange(int amt) {
        StartCoroutine(ChangeFill(XPFill, (float)currXP, (float)amt, (float)this.maxXP));

        if (currXP + amt < maxXP) {
            currXP += amt;
        }

        StatsManager.instance.currXP = this.currXP;
        await UpdateXP();
    }

    private async void HandleHappyChange(int amt) {
        if (amt > 0) {
            if (currHappy < maxHappy) {
                StatsManager.instance.happinessFull = false;
                StartCoroutine(ChangeFill(happyFill, (float)currHappy, (float)amt, (float)this.maxHappy));
                currHappy += Mathf.Min(amt, maxHappy - currHappy);
            } else {
                StatsManager.instance.happinessFull = true;
            }
        } else { // TODO: not completely implemented yet
            if (currHappy + amt >= 0) {
                currHappy += amt;
            }
        }

        StatsManager.instance.currHappy = this.currHappy;
        await UpdateHappiness();
    }

    IEnumerator ChangeFill(Image fill, float init, float amt, float max) {
        float i = 0;
        while (i <= 1) {
            fill.fillAmount = Mathf.Lerp(init / max, Mathf.Min(1, (init + amt) / max), i);
            i += 0.05f;
            yield return null;
        }

        if (fill == XPFill && init + amt >= max) {
            yield return new WaitForSeconds(0.1f);
            LevelUp();
        }
    }

    async void LevelUp() {
        SetFill(XPFill, 0, maxXP);
        currLvl++;
        lvlText.text = currLvl.ToString();

        currXP = 0;
        maxXP += 20;
        if (reward <= 20) {
            reward += 2;
        }

        string congrats = $"Congrats! You levelled up! Gained {reward} catfood";
        StartCoroutine(CatBehaviourManager.instance.DisplayNotifs(congrats));
        CatfoodManager.instance.IncreaseCatfood(reward);
        await UpdateXP();

    }

    void SetFill(Image fill, float amt, float max) {
        fill.fillAmount = amt / max;
    }

    private async Task UpdateHappiness() {
        SceneTransition.instance.user.currHappiness = currHappy;

        string currHap = JsonConvert.SerializeObject(SceneTransition.instance.user.currHappiness);

        DatabaseReference DBreference = FirebaseDatabase.DefaultInstance.RootReference;
        await DBreference.Child("users").Child(SceneTransition.instance.user.userId).Child("currHappiness").SetValueAsync(currHap);
    }

    private async Task UpdateXP() {
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
}
