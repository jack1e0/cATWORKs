using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;
using TMPro;

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
    [SerializeField] private GameObject addScreen;
    [SerializeField] private GameObject blocker;
    [SerializeField] private GameObject popUp;
    [SerializeField] private TMP_Text repeatPlaceholder;
    [SerializeField] private GameObject hourInput;
    [SerializeField] private GameObject minInput;

    private GameObject parent;
    private List<GameObject> alarms;

    private bool isPopUpActive;
    private int hour;
    private int min;
    private int time;
    private string repeat = "Default";

    private void Awake() {
        if (instance == null) {
            instance = this;
            alarms = new List<GameObject>();
            //DontDestroyOnLoad(this);
        }
        // else {
        //     Destroy(gameObject);
        //     instance = GameObject.FindGameObjectWithTag("AlarmManager").GetComponent<AlarmManager>();
        // }
        instance.Instantiate();
    }

    private async void Instantiate() {
        addScreen.SetActive(false);
        blocker.SetActive(false);
        addButton = GameObject.FindGameObjectWithTag("Button");
        addButton.GetComponent<Button>().onClick.AddListener(AddAlarm);
        backButton = GameObject.FindGameObjectWithTag("Back");
        backButton.GetComponent<Button>().onClick.AddListener(Back);

        parent = GameObject.FindGameObjectWithTag("Parent");

        float heights = 0;
        foreach (GameObject alarm in alarms) {
            Instantiate(alarm, parent.transform);

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
        addScreen.SetActive(true);
    }

    public void DeleteAlarm(GameObject alarm) {
        float height = alarm.GetComponent<RectTransform>().rect.height;
        VerticalLayoutGroup.Destroy(alarm);
        StartCoroutine(SizeFitter.instance.Contract(height));

        // TODO: removing specific alarm
        alarms.RemoveAt(0);
    }

    public void Back() {
        SceneManager.LoadScene("RoomScene");
    }

    // Button functions in addScreen

    public void CancelAdd() {
        addScreen.SetActive(false);
        hourInput.GetComponent<TMP_InputField>().text = string.Empty;
        minInput.GetComponent<TMP_InputField>().text = string.Empty;
        this.hour = 0;
        this.min = 0;
    }

    public void ConfirmAlarm() {
        GameObject newUnit = Instantiate(alarmUnit, parent.transform);
        newUnit.GetComponent<AlarmSetter>().SetValues(CalcTime(), repeat);

        alarms.Add(alarmUnit);
        float height = newUnit.GetComponent<RectTransform>().rect.height;
        StartCoroutine(SizeFitter.instance.Expand(height));
        addScreen.SetActive(false);
        hourInput.GetComponent<TMP_InputField>().text = string.Empty;
        minInput.GetComponent<TMP_InputField>().text = string.Empty;
        this.hour = 0;
        this.min = 0;
        //SetAlarm("hihi", 2, 40);
    }

    public void ReadHours(string hours) {
        int value;
        bool isInt = int.TryParse(hours, out value);
        if (!isInt || value < 0 || value > 24) {
            hourInput.GetComponent<TMP_InputField>().text = string.Empty;
        } else {
            this.hour = value;
        }
    }

    public void ReadMins(string mins) {
        int value;
        bool isInt = int.TryParse(mins, out value);
        if (!isInt || value < 0 || value > 60) {
            minInput.GetComponent<TMP_InputField>().text = string.Empty;
        } else {
            this.min = value;
        }
    }

    private int CalcTime() {
        int time = this.hour * 100 + this.min;
        Debug.Log("TIME:" + time);
        return time;
    }

    public void ShowPopUp() {
        StartCoroutine(Slide(0));
        blocker.SetActive(true);
        isPopUpActive = true;
    }

    private void HidePopUp() {
        StartCoroutine(Slide(1));
        blocker.SetActive(false);
        isPopUpActive = false;
    }

    IEnumerator Slide(int upOrDown) {
        Vector3 origPos = popUp.transform.position;
        Vector3 newpos;
        if (upOrDown == 0) {
            newpos = new Vector3(0, -3.2f, 0);
        } else {
            newpos = new Vector3(0, -7, 0);
        }
        float i = 0;
        while (i <= 1.2f) {
            popUp.transform.position = Vector3.Lerp(origPos, newpos, i);
            i += 0.2f;
            yield return null;
        }
    }

    public void Option1() {
        this.repeat = "Everyday";
        repeatPlaceholder.text = "Everyday";
        HidePopUp();
    }

    public void Option2() {
        this.repeat = "Every weekday";
        repeatPlaceholder.text = "Every weekday";
        HidePopUp();
    }

    public void Option3() {
        this.repeat = "Every weekend";
        repeatPlaceholder.text = "Every weekend";
        HidePopUp();
    }

    public void Option4() {
        this.repeat = "None";
        repeatPlaceholder.text = "None";
        HidePopUp();
    }

    // public void OnPointerDown(PointerEventData eventData) {
    //     Debug.Log("pressed but");
    //     if (isPopUpActive) {
    //         Debug.Log("PRESSED");
    //         HidePopUp();
    //     }
    // }

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
