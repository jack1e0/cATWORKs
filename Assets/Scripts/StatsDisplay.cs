using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StatsDisplay : MonoBehaviour {
    [SerializeField] private Image XPFill;
    [SerializeField] private TMP_Text lvlText;
    [SerializeField] private Image happyFill;

    [SerializeField] private int currXP, maxXP, currLvl;
    [SerializeField] private int currHappy, maxHappy;

    private void Awake() {
        this.currXP = StatsManager.instance.currXP;
        this.currHappy = StatsManager.instance.currHappy;
        this.currLvl = StatsManager.instance.currLvl;
    }

    private void OnEnable() {
        StatsManager.instance.OnXPChange += HandleXPChange;
        StatsManager.instance.onHappyChange += HandleHappyChange;
    }

    private void OnDisable() {
        StatsManager.instance.SetStats(this.currXP, this.currHappy, this.currLvl);

        StatsManager.instance.OnXPChange -= HandleXPChange;
        StatsManager.instance.onHappyChange -= HandleHappyChange;
    }

    private void HandleXPChange(int amt) {
        StartCoroutine(ChangeFill(XPFill, (float)currXP, (float)amt, (float)this.maxXP));
        currXP += amt;
        if (currXP >= maxXP) {
            currLvl++;
            lvlText.text = currLvl.ToString();

            currXP = 0;
            maxXP += 20;
            SetFill(XPFill, 0, (float)maxXP);
        }
    }

    private void HandleHappyChange(int amt) {
        if (amt > 0) {
            if (currHappy < maxHappy) {
                StartCoroutine(ChangeFill(happyFill, (float)currHappy, (float)amt, (float)this.maxHappy));
                currHappy += Mathf.Min(amt, maxHappy - currHappy);
            }
        } else { // TODO: not completely implemented yet
            if (currHappy + amt >= 0) {
                currHappy += amt;
            }
        }
    }

    IEnumerator ChangeFill(Image fill, float init, float amt, float max) {
        float i = 0;
        while (i <= 1) {
            fill.fillAmount = Mathf.Lerp(init / max, Mathf.Min(1, (init + amt) / max), i);
            i += 0.1f;
            yield return null;
        }
    }

    void SetFill(Image fill, float amt, float max) {
        fill.fillAmount = amt / max;
    }
}
