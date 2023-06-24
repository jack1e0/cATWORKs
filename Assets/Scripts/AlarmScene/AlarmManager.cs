using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AlarmManager : MonoBehaviour {
    // TODO: implement alarm setter
    // [SerializeField] private GameObject alarmSetterPage;


    // const string ACTION_SET_ALARM = "android.intent.action.SET_ALARM";
    // const string EXTRA_HOUR = "android.intent.extra.alarm.HOUR";
    // const string EXTRA_MINUTES = "android.intent.extra.alarm.MINUTES";
    // const string EXTRA_MESSAGE = "android.intent.extra.alarm.MESSAGE";


    [SerializeField] private GameObject alarmUnit;
    [SerializeField] private GameObject parent;


    public void AddAlarm() {
        GameObject newUnit = Instantiate(alarmUnit, parent.transform);
        StartCoroutine(SizeFitter.instance.Expand(newUnit));
        //SetAlarm("hihi", 2, 40);
    }

    public void Back() {
        SceneManager.LoadScene("RoomScene");
    }

    // private void SetAlarm(string msg, int hour, int minutes) {
    //     AndroidJavaObject intentAJO = new AndroidJavaObject("android.content.Intent", ACTION_SET_ALARM);
    //     intentAJO
    //         .Call<AndroidJavaObject>("putEXTRA", EXTRA_MESSAGE, msg)
    //         .Call<AndroidJavaObject>("putEXTRA", EXTRA_HOUR, hour)
    //         .Call<AndroidJavaObject>("putEXTRA", EXTRA_MINUTES, minutes);

    //     GetUnityActivity().Call("startActivity", intentAJO);
    // }

    // private AndroidJavaObject GetUnityActivity() {
    //     using (var unityPlayer = new AndroidJavaClass("com.unity2d.player.UnityPlayer")) {
    //         return unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
    //     }
    // }
}
