using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatsManager : MonoBehaviour {
    public static StatsManager instance;

    public delegate void XPChangeHandler(int amt);
    public event XPChangeHandler OnXPChange;

    public delegate void HappyChangeHandler(int amt);
    public event HappyChangeHandler onHappyChange;

    public int currXP;
    public int currHappy;
    public int currLvl;

    public bool happinessFull;
    private bool notFirstEnter;

    private void Awake() {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(this);
        } else {
            Destroy(this);
            instance = GameObject.FindGameObjectWithTag("Manager").GetComponent<StatsManager>();
        }
        instance.Instantiate();
    }

    private void Instantiate() {
        if (!notFirstEnter) {
            this.currXP = SceneTransition.instance.user.currXP;
            this.currHappy = SceneTransition.instance.user.currHappiness;
            this.currLvl = SceneTransition.instance.user.level;
            notFirstEnter = true;
        }
    }

    public void AddXP(int amt) {
        OnXPChange?.Invoke(amt);
    }

    public void ChangeHappy(int amt) {
        onHappyChange?.Invoke(amt);
    }

    public void SetStats(int currXP, int currHappy, int currLvl) {
        this.currXP = currXP;
        this.currHappy = currHappy;
        this.currLvl = currLvl;
    }
}
