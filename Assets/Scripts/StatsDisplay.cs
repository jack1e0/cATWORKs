using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatsDisplay : MonoBehaviour {
    [SerializeField] private int currXP, maxXP, currLvl;
    [SerializeField] private int currFood, maxFood;

    private void OnEnable() {
        StatsManager.instance.OnXPChange += HandleXPChange;
        StatsManager.instance.onFoodChange += HandleFoodChange;
    }

    private void OnDisable() {
        StatsManager.instance.OnXPChange -= HandleXPChange;
        StatsManager.instance.onFoodChange -= HandleFoodChange;
    }

    private void HandleXPChange(int amt) {
        currXP += amt;
        if (currXP >= maxXP) {
            currLvl++;
            currXP = 0;
            maxXP += 20;
        }
    }

    private void HandleFoodChange(int amt) {
        if (amt > 0) {
            if (currFood + amt <= maxFood) {
                currFood += amt;
            }
        } else {
            if (currFood + amt >= 0) {
                currFood += amt;
            }
        }
    }
}
