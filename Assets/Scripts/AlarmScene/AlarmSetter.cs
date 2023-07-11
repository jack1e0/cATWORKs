using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Text;

public class AlarmSetter : MonoBehaviour {
    [SerializeField] private GameObject time;
    [SerializeField] private GameObject repeat;

    public int notifID;
    private TMP_Text timeText;
    private TMP_Text repeatText;

    private void Awake() {
        timeText = time.GetComponent<TMP_Text>();
        repeatText = repeat.GetComponent<TMP_Text>();
    }

    public void SetValues(int notifID, int timing, string repeat) {
        this.notifID = notifID;
        this.timeText.text = FormatTime(timing);
        this.repeatText.text = repeat;
    }

    private string FormatTime(int timing) {
        string final = timing.ToString();
        Debug.Log("final: " + final);
        if (final.Length == 0) { // when e.g. time = 640 (06 40) or 40 (00 40) or 1 (00 01);
            final = "0000";
        } else if (final.Length == 1) {
            final = "000" + final;
        } else if (final.Length == 2) {
            final = "00" + final;
        } else if (final.Length == 3) {
            final = "0" + final;
        }
        StringBuilder sb = new StringBuilder(final);
        sb.Insert(2, ' ');
        return sb.ToString();

    }
}
