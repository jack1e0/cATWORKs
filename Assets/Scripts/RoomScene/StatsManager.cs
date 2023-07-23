using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatsManager : MonoBehaviour {
    public static StatsManager instance;

    public delegate void XPChangeHandler(int amt);
    public event XPChangeHandler OnXPChange;

    public delegate void HappyChangeHandler(int amt);
    public event HappyChangeHandler onHappyChange;

    public bool happinessFull;
    public float happinessPercent;
    private bool notFirstEnter;

    private void Awake() {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(this);
        } else {
            Destroy(this);
            instance = GameObject.FindGameObjectWithTag("Manager").GetComponent<StatsManager>();
        }
    }


    public void AddXP(int amt) {
        OnXPChange?.Invoke(amt);
    }

    public void ChangeHappy(int amt) {
        onHappyChange?.Invoke(amt);
    }
}
