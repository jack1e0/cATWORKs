using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CatfoodManager : MonoBehaviour {

    public static CatfoodManager instance;
    private TMP_Text catfoodText;
    private int catfoodCount;
    public int toChange;

    void Awake() {
        DontDestroyOnLoad(this);

        // if (instance == null) {
        //     instance = this;
        // } else { // if there is already another previous instance of manager, keep that.
        //     instance = this;
        //     Destroy(instance.gameObject);
        // }

        // if (instance != null && instance != this) {
        //     Debug.Log(instance.catfoodCount);
        //     Destroy(this);
        //     Destroy(this.gameObject);
        // } else {
        //     instance = this;
        //     catfoodText = GameObject.FindGameObjectWithTag("Catfood").GetComponent<TMP_Text>();
        //     catfoodCount = int.Parse(catfoodText.text);
        // }

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
        catfoodCount = int.Parse(catfoodText.text);
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
        int food = Mathf.CeilToInt(studyDuration / 6);
        int earned = Mathf.Max(2, food);
        Debug.Log("catfood earned: " + earned);
        toChange = earned;
        return earned;
    }
}
