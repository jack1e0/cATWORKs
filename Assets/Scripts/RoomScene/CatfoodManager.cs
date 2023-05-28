using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CatfoodManager : MonoBehaviour {

    public static CatfoodManager instance;
    private TMP_Text catfoodText;
    private int catfoodCount;

    void Awake() {
        if (instance == null) {
            instance = this;
        }
        catfoodText = gameObject.GetComponent<TMP_Text>();
        catfoodCount = int.Parse(catfoodText.text);
    }

    public void IncreaseCatfood(int dif) {
        int newCount = catfoodCount += dif;
        catfoodCount = newCount;
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
}
