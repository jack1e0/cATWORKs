using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;

public class AlarmManager : MonoBehaviour {
    // TODO: implement alarm setter
    // [SerializeField] private GameObject alarmSetterPage;


    // const string ACTION_SET_ALARM = "android.intent.action.SET_ALARM";
    // const string EXTRA_HOUR = "android.intent.extra.alarm.HOUR";
    // const string EXTRA_MINUTES = "android.intent.extra.alarm.MINUTES";
    // const string EXTRA_MESSAGE = "android.intent.extra.alarm.MESSAGE";

    private GameObject addButton;
    private GameObject backButton;

    public static AlarmManager instance;
    [SerializeField] private GameObject alarmUnit;
    private GameObject parent;
    private List<GameObject> alarms;

    private void Awake() {
        if (instance == null) {
            instance = this;
            alarms = new List<GameObject>();
            DontDestroyOnLoad(this);
        } else {
            Destroy(gameObject);
            instance = GameObject.FindGameObjectWithTag("AlarmManager").GetComponent<AlarmManager>();
        }
        instance.Instantiate();
    }

    private async void Instantiate() {
        addButton = GameObject.FindGameObjectWithTag("Button");
        addButton.GetComponent<Button>().onClick.AddListener(AddAlarm);
        backButton = GameObject.FindGameObjectWithTag("Back");
        backButton.GetComponent<Button>().onClick.AddListener(Back);

        parent = GameObject.FindGameObjectWithTag("Parent");

        float heights = 0;
        foreach (GameObject alarm in alarms) {
            Debug.Log("ALARM: " + alarm);
            Debug.Log("INSIE");
            Instantiate(alarm, parent.transform);
            Debug.Log("HEHEHE");

            heights += alarm.GetComponent<RectTransform>().rect.height;
        }

        await WaitFrame();
        StartCoroutine(SizeFitter.instance.Expand(heights));
    }

    // Wait after sizefitter awakes
    private async Task WaitFrame() {
        var currnet = Time.frameCount;

        while (currnet == Time.frameCount) {
            await Task.Yield();
        }
    }

    public void AddAlarm() {
        GameObject newUnit = Instantiate(alarmUnit, parent.transform);
        alarms.Add(alarmUnit);
        float height = newUnit.GetComponent<RectTransform>().rect.height;
        StartCoroutine(SizeFitter.instance.Expand(height));
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
