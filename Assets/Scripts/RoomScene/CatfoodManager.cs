using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Newtonsoft.Json;
using Firebase.Database;
using System.Threading.Tasks;

public class CatfoodManager : MonoBehaviour {

    public static CatfoodManager instance;
    private TMP_Text catfoodText;
    public int catfoodCount;
    public int earnedCatfood;
    public int earnedXP;

    private bool notFirstEnter;

    void Awake() {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(this);
        } else {
            Destroy(this);
            instance = GameObject.FindGameObjectWithTag("Manager").GetComponent<CatfoodManager>();
        }
        instance.Instantiate();
    }

    public void Instantiate() {
        catfoodText = GameObject.FindGameObjectWithTag("Catfood").GetComponent<TMP_Text>();
        if (!notFirstEnter) {
            Debug.Log("Instantiating catfood count");
            catfoodCount = SceneTransition.instance.user.catfoodCount;
            notFirstEnter = true;
        }
        catfoodText.text = catfoodCount.ToString("0");

    }

    public async void IncreaseCatfood(int dif) {
        catfoodCount += dif;
        catfoodText.text = catfoodCount.ToString("0");
        await UpdateCatfood();
    }

    public async Task<bool> DecreaseCatfood(int dif) {
        if (dif <= catfoodCount) {
            int newCount = catfoodCount -= dif;
            catfoodCount = newCount;
            catfoodText.text = catfoodCount.ToString("0");
            await UpdateCatfood();
            return true;
        } else {
            return false;
        }
    }

    private async Task UpdateCatfood() {
        SceneTransition.instance.user.catfoodCount = catfoodCount;

        string catfood = JsonConvert.SerializeObject(SceneTransition.instance.user.catfoodCount);

        DatabaseReference DBreference = FirebaseDatabase.DefaultInstance.RootReference;
        await DBreference.Child("users").Child(SceneTransition.instance.user.userId).Child("catfoodCount").SetValueAsync(catfood);
    }

    public int CalculateCatfood(float studyDuration) {
        if (studyDuration < 0) {
            return 0;
        }
        int food = Mathf.CeilToInt(studyDuration / 2 * (StatsManager.instance.happinessPercent) * 5);
        int earned = Mathf.Max(2, food);
        Debug.Log("catfood earned: " + earned);
        earnedCatfood = earned;
        return earned;
    }

    public int CalculateXP(float studyDuration) {
        if (studyDuration < 0) {
            return 0;
        }
        earnedXP = Mathf.CeilToInt(studyDuration * 1.2f);
        Debug.Log("XP earned: " + earnedXP);
        return earnedXP;
    }
}
