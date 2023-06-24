using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

/// <summary>
/// Attached to Calendar gameobject
/// 
/// Alarm mechanics:
/// 
/// When click on alarm, it will take user to a new page, with a list of alarms (both active and inactive), with a plus button that can add alarms
/// similar to clock app
/// 
/// If user enters game within 5 min after alarm, game will display "time to start studying", and will drop bonus cat foods or smt
/// if enter game after 5 min after alarm, game will display "missed study session" and bonus cat food will not drop
/// 
/// TODO: keep track of the timings of alarm, or after notifs show then take note of time. then note time when enter game. then take different in time.
///       upon enter game within 5 min, the only button that will be working is the study button.
///
/// </summary>
public class AlarmScript : MonoBehaviour {

    [SerializeField] private TMP_Text countdown;
    [SerializeField] private GameObject timerScreen;
    private Button button;
    private bool needToStudy;

    private void Awake() {
        button = gameObject.GetComponent<Button>();
        timerScreen.SetActive(false);

        button.onClick.AddListener(StartAlarm);
    }

    public void StartAlarm() {
        CatBehaviourManager.instance.ButtonPressBefore(CatState.NONE);
        SceneManager.LoadScene("AlarmScene");
    }

}
