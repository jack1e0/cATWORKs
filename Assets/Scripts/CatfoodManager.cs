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

    public void UpdateCatfoodCount(int difference) {
        catfoodCount += difference;
        catfoodText.text = catfoodCount.ToString();
    }
}
