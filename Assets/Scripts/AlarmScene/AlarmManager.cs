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
    private int id; // to be saved, everytime notif is scheduled, increase id by 1

    [SerializeField] private GameObject alarmUnit;
    [SerializeField] private GameObject addScreen;
    [SerializeField] private GameObject blocker;
    [SerializeField] private GameObject repeatPopUp;
    [SerializeField] private GameObject customPopUp;
    [SerializeField] private TMP_Text repeatPlaceholder;
    [SerializeField] private GameObject hourInput;
    [SerializeField] private GameObject minInput;

    private GameObject parent;

    private int hour;
    private int min;
    private int time;
    private string repeat = "None";

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
                                                                                         ? "Every Monday"
                                                                                         : keyValue.Value[1] == 2
                                                                                         ? "Every Tuesday"
                                                                                         : keyValue.Value[1] == 3
                                                                                         ? "Every Wednesday"
                                                                                         : keyValue.Value[1] == 4
                                                                                         ? "Every Thursday"
                                                                                         : keyValue.Value[1] == 5
                                                                                         ? "Every Friday"
                                                                                         : keyValue.Value[1] == 6
                                                                                         ? "Every Saturday"
                                                                                         : keyValue.Value[1] == 7
                                                                                         ? "Every Sunday"
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
            Id = "alarm_channel",
            Name = "Alarm Channel",
            Importance = Importance.High,
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
        addScreen.SetActive(true);
        addScreen.GetComponent<CanvasGroup>().alpha = 0;
        LeanTween.alphaCanvas(addScreen.GetComponent<CanvasGroup>(), 1, 0.1f);
    }

    public void DeleteAlarm(GameObject alarm) {
        int deletedID = alarm.GetComponent<AlarmSetter>().notifID;
        alarmDict.Remove(deletedID);
        AndroidNotificationCenter.CancelScheduledNotification(deletedID);

        float height = alarm.GetComponent<RectTransform>().rect.height;
        VerticalLayoutGroup.Destroy(alarm);
        StartCoroutine(SizeFitter.instance.Contract(height));
    }

    public void Back() {
        SceneTransition.instance.ChangeScene("RoomScene");
    }

    // Button functions in addScreen

    public void CancelAdd() {
        StartCoroutine(FadeAddScreen());
        hourInput.GetComponent<TMP_InputField>().text = string.Empty;
        minInput.GetComponent<TMP_InputField>().text = string.Empty;
        this.hour = 0;
        this.min = 0;
        this.repeat = "None";
    }

    public void ConfirmAlarm() {
        GameObject newUnit = Instantiate(alarmUnit, parent.transform);
        int time = CalcTime();
        ScheduleNotification(time);

        newUnit.GetComponent<AlarmSetter>().SetValues(id, time, this.repeat);
        alarmDict.Add(id, new int[2] {time, repeat == "Everyday"
                                    ? 0
                                    : repeat == "Every Monday"
                                    ? 1
                                    : repeat == "Every Tuesday"
                                    ? 2
                                    : repeat == "Every Wednesday"
                                    ? 3
                                    : repeat == "Every Thursday"
                                    ? 4
                                    : repeat == "Every Friday"
                                    ? 5
                                    : repeat == "Every Saturday"
                                    ? 6
                                    : repeat == "Every Sunday"
                                    ? 7
                                    : 8 });
        id++;

        float height = newUnit.GetComponent<RectTransform>().rect.height;
        StartCoroutine(SizeFitter.instance.Expand(height));

        StartCoroutine(FadeAddScreen());
        hourInput.GetComponent<TMP_InputField>().text = string.Empty;
        minInput.GetComponent<TMP_InputField>().text = string.Empty;
        this.hour = 0;
        this.min = 0;
        this.repeat = "None";
    }

    IEnumerator FadeAddScreen() {
        LeanTween.alphaCanvas(addScreen.GetComponent<CanvasGroup>(), 0, 0.1f);
        yield return new WaitForSeconds(0.1f);
        addScreen.SetActive(false);
    }


    private void ScheduleNotification(int time) { // time in 24h
        AndroidNotification notification = new AndroidNotification();
        notification.Title = "Psst..";
        notification.Text = "Time to study!";

        DateTime timeTday = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, (time - time % 100) / 100, time % 100, 0); // the time TODAY
        int daysAway = 0;
        DayOfWeek day = DayOfWeek.Sunday; // default val

        switch (repeat) {
            case "None":
                notification.FireTime = timeTday;
                notification.IntentData = time.ToString();
                AndroidNotificationCenter.SendNotificationWithExplicitID(notification, "alarm_channel", id);
                return;
            case "Everyday":
                notification.FireTime = timeTday;
                notification.IntentData = time.ToString();
                notification.RepeatInterval = new System.TimeSpan(1, 0, 0, 0);
                AndroidNotificationCenter.SendNotificationWithExplicitID(notification, "alarm_channel", id);
                return;
            case "Every Monday":
                day = DayOfWeek.Monday;
                break;
            case "Every Tuesday":
                day = DayOfWeek.Tuesday;
                break;
            case "Every Wednesday":
                day = DayOfWeek.Wednesday;
                break;
            case "Every Thursday":
                day = DayOfWeek.Thursday;
                break;
            case "Every Friday":
                day = DayOfWeek.Friday;
                break;
            case "Every Saturday":
                day = DayOfWeek.Saturday;
                break;
            case "Every Sunday":
                day = DayOfWeek.Sunday;
                break;
        }

        daysAway = (int)timeTday.DayOfWeek - (int)day < 0
                 ? (int)day - (int)timeTday.DayOfWeek
                 : (int)timeTday.DayOfWeek - (int)day > 0
                 ? 7 - ((int)timeTday.DayOfWeek - (int)day)
                 : 0;

        notification.FireTime = timeTday.AddDays(daysAway);
        notification.RepeatInterval = new System.TimeSpan(7, 0, 0, 0);
        notification.IntentData = time.ToString();
        Debug.Log("intent data: " + notification.IntentData);
        Debug.Log("days away: " + daysAway);
        Debug.Log("fire time: " + notification.FireTime + ", intervals: " + notification.RepeatInterval);

        AndroidNotificationCenter.SendNotificationWithExplicitID(notification, "alarm_channel", id);
        Debug.Log("notif id: " + id);
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

    public void ShowRepeatPopUp() {
        StartCoroutine(Slide(repeatPopUp, 0));
        blocker.SetActive(true);
    }

    private void HideRepeatPopUp() {
        StartCoroutine(Slide(repeatPopUp, 1));
    }

    private void ShowCustompPopUp() {
        StartCoroutine(Slide(customPopUp, 0));
    }

    private void HideCustomPopUp() {
        StartCoroutine(Slide(customPopUp, 1));
        blocker.SetActive(false);
    }

    IEnumerator Slide(GameObject popUp, int upOrDown) {

        Vector3 origPos = popUp.transform.position;
        Vector3 newpos;
        if (upOrDown == 0) {
            if (popUp.Equals(repeatPopUp)) {
                newpos = new Vector3(0, -3.5f, 0);
            } else {
                newpos = new Vector3(0, -0.2f, 0);
            }
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

    public void Everyday() {
        this.repeat = "Everyday";
        repeatPlaceholder.text = "Everyday";
        HideRepeatPopUp();
        blocker.SetActive(false);

    }

    public void Custom() {
        HideRepeatPopUp();
        ShowCustompPopUp();
    }

    public void None() {
        this.repeat = "None";
        repeatPlaceholder.text = "None";
        HideRepeatPopUp();
        blocker.SetActive(false);
    }

    // Custom pop up methods:

    public void Sunday() {
        this.repeat = "Every Sunday";
        repeatPlaceholder.text = "Every Sunday";
        HideCustomPopUp();
    }

    public void Monday() {
        this.repeat = "Every Monday";
        repeatPlaceholder.text = "Every Monday";
        HideCustomPopUp();
    }
    public void Tuesday() {
        this.repeat = "Every Tuesday";
        repeatPlaceholder.text = "Every Tuesday";
        HideCustomPopUp();
    }
    public void Wednesday() {
        this.repeat = "Every Wednesday";
        repeatPlaceholder.text = "Every Wednesday";
        HideCustomPopUp();
    }
    public void Thursday() {
        this.repeat = "Every Thursday";
        repeatPlaceholder.text = "Every Thursday";
        HideCustomPopUp();
    }
    public void Friday() {
        this.repeat = "Every Friday";
        repeatPlaceholder.text = "Every Friday";
        HideCustomPopUp();
    }
    public void Saturday() {
        this.repeat = "Every Saturday";
        repeatPlaceholder.text = "Every Saturday";
        HideCustomPopUp();
    }

}
