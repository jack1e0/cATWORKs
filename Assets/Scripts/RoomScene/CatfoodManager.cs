using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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

    public void IncreaseCatfood(int dif) {
        catfoodCount += dif;
        catfoodText.text = catfoodCount.ToString("0");
    }

    public bool DecreaseCatfood(int dif) {
        if (dif <= catfoodCount) {
            int newCount = catfoodCount -= dif;
            catfoodCount = newCount;
            catfoodText.text = catfoodCount.ToString("0");
            return true;
        } else {
            return false;
        }
    }

    public int CalculateCatfood(float studyDuration) {
        if (studyDuration < 0) {
            return 0;
        }
        int food = Mathf.CeilToInt(studyDuration / 2);
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
