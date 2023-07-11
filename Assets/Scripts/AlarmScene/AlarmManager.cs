using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;
using TMPro;
using Unity.Notifications.Android;

public class AlarmManager : MonoBehaviour {
    private Dictionary<int, int[]> alarmDict; // a mapping of notif id to its time and repeats

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

    private bool isPopUpActive;
    private int hour;
    private int min;
    private int time;
    private string repeat = "Default";

    private void Awake() {
        if (instance == null) {
            instance = this;
            alarmDict = new Dictionary<int, int[]>();
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
        foreach (KeyValuePair<int, int[]> keyValue in alarmDict) {
            GameObject newUnit = Instantiate(alarmUnit, parent.transform);
            newUnit.GetComponent<AlarmSetter>().SetValues(keyValue.Key, keyValue.Value[0], keyValue.Value[1] == 0
                                                                                         ? "Everyday"
                                                                                         : keyValue.Value[1] == 1
                                                                                         ? "Every weekday"
                                                                                         : keyValue.Value[1] == 2
                                                                                         ? "Every weekend"
                                                                                         : "None"
                                                         );

            heights += newUnit.GetComponent<RectTransform>().rect.height;
        }

        await WaitFrame();
        StartCoroutine(SizeFitter.instance.Expand(heights));
    }

    private void Start() {
        // Creating Android Notification Channel to send messages through
        var channel = new AndroidNotificationChannel() {
            Id = "channel_id",
            Name = "Alarm Channel",
            Importance = Importance.Default,
            Description = "Alarm notifications",
        };
        AndroidNotificationCenter.RegisterNotificationChannel(channel);
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
        int deletedID = alarm.GetComponent<AlarmSetter>().notifID;
        alarmDict.Remove(deletedID);

        float height = alarm.GetComponent<RectTransform>().rect.height;
        VerticalLayoutGroup.Destroy(alarm);
        StartCoroutine(SizeFitter.instance.Contract(height));
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
        int time = CalcTime();
        int notifID = ScheduleNotification(time);

        newUnit.GetComponent<AlarmSetter>().SetValues(notifID, time, this.repeat);
        alarmDict.Add(notifID, new int[2] {time, repeat == "Everyday"
                                    ? 0
                                    : repeat == "Every weekday"
                                    ? 1
                                    : repeat == "Every weekend"
                                    ? 2
                                    : 3 });

        float height = newUnit.GetComponent<RectTransform>().rect.height;
        StartCoroutine(SizeFitter.instance.Expand(height));

        addScreen.SetActive(false);
        hourInput.GetComponent<TMP_InputField>().text = string.Empty;
        minInput.GetComponent<TMP_InputField>().text = string.Empty;
        this.hour = 0;
        this.min = 0;
        //SetAlarm("hihi", 2, 40);
    }


    private int ScheduleNotification(int time) { // time in 24h
        var notification = new AndroidNotification();
        notification.Title = "Psst..";
        notification.Text = "Time to study!";

        DateTime dateVal = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, (time - time % 100) / 100, time % 100, 0);
        if (repeat == "None") {
            notification.FireTime = dateVal;
        } else if (repeat == "Everyday") {
            notification.FireTime = dateVal;
            notification.RepeatInterval = new System.TimeSpan(1, 0, 0, 0);
        }


        return AndroidNotificationCenter.SendNotification(notification, "channel_id");
    }

    // Managing user input while setting time

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


    // Popup methods:

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

}
