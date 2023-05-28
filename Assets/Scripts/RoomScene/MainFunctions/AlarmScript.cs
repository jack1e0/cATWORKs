using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Attached to Calendar gameobject
/// </summary>
public class AlarmScript : MonoBehaviour {

    public float timer = 10f;
    private float currTime = 0f;
    [SerializeField] private TMP_Text countdown;
    [SerializeField] private GameObject timerScreen;
    private Button button;
    private bool alarmPresent;

    private void Awake() {
        alarmPresent = false;
        button = gameObject.GetComponent<Button>();
    }

    void Start() {
        currTime = timer;
        timerScreen.SetActive(false);
        button.onClick.AddListener(StartAlarm);
    }

    public void StartAlarm() {
        if (!alarmPresent) {
            CatBehaviourManager.instance.ButtonControl(false);
            string msg = "Started alarm for 10s later!";
            StartCoroutine(CatBehaviourManager.instance.DisplayNotifs(msg));
            StartCoroutine(Countdown());
        } else {
            Debug.Log("Alarm already set");
        }

    }

    private IEnumerator Countdown() {
        alarmPresent = true;
        currTime = this.timer;
        while (currTime >= 6) {
            currTime--;
            yield return new WaitForSeconds(1);
        }
        timerScreen.SetActive(true);
        while (currTime >= 0) {
            countdown.text = currTime.ToString("0");
            currTime--;
            yield return new WaitForSeconds(1);
        }
        timerScreen.SetActive(false);
        string msg = "Time to start studying!";
        StartCoroutine(CatBehaviourManager.instance.DisplayNotifs(msg));
        alarmPresent = false;
        CatBehaviourManager.instance.ButtonControl(true);
    }
}
