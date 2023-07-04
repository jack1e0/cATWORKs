using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatsManager : MonoBehaviour {
    public static StatsManager instance;

    public delegate void XPChangeHandler(int amt);
    public event XPChangeHandler OnXPChange;

    public delegate void FoodChangeHandler(int amt);
    public event FoodChangeHandler onFoodChange;

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

    public void ChangeFood(int amt) {
        onFoodChange?.Invoke(amt);
    }
}
