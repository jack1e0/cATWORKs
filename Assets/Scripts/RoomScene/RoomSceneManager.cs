using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using Unity.Notifications.Android;
using Newtonsoft.Json;


public class RoomSceneManager : MonoBehaviour {
    [SerializeField] private GameObject catPrefab;
    public CatController catControl;
    private GameObject[] buttons;

    private Image notifs;

    public bool justStudied;
    private bool notFirstEnteredGame;
    public bool quitStudy;

    private bool notifsExist;

    // Making a singleton class
    public static RoomSceneManager instance;

    void Awake() {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(this);
        } else {
            Destroy(gameObject);
            instance = GameObject.FindGameObjectWithTag("Manager").GetComponent<RoomSceneManager>();
        }
    }

    private void OnEnable() {
        SceneManager.sceneLoaded += Instantiate;
    }

    private void OnDisable() {
        SceneManager.sceneLoaded -= Instantiate;
    }

    public void Instantiate(Scene scene, LoadSceneMode mode) {
        if (scene.name == "RoomScene") {
            Debug.Log("enter room scene!!");

            buttons = GameObject.FindGameObjectsWithTag("Button");
            notifs = GameObject.FindGameObjectWithTag("Notifs").GetComponent<Image>();
            notifs.GetComponent<Button>().onClick.AddListener(SkipNotifs);
            notifs.enabled = false;

            if (!SceneTransition.instance.user.firstTime) {
                InstantiateCats();
            }

            DateTime prev = SceneTransition.instance.user.prevExitTime;
            double mins = (System.DateTime.Now - prev).TotalMinutes;
            Debug.Log("mins: " + mins);
            StatsManager.instance.ChangeHappy(-1 * mins);


            if (justStudied) {
                Debug.Log("HERE");
                StartCoroutine(EndStudy());
                justStudied = false;
            }

            if (SceneTransition.instance.firstEnteredRoom && !SceneTransition.instance.user.firstTime) {
                string msg = "Welcome back, " + SceneTransition.instance.user.username + "!";
                StartCoroutine(DisplayNotifs(msg));
                SceneTransition.instance.firstEnteredRoom = false;
            }

            var intentData = AndroidNotificationCenter.GetLastNotificationIntent();
            if (intentData != null && intentData.Channel == "alarm_channel") {
                DateTime alarmTime = DateTime.ParseExact(intentData.ToString(), "HHmm", null, System.Globalization.DateTimeStyles.None);
                TimeSpan timeDif = System.DateTime.Now - alarmTime;
                if (timeDif.TotalMinutes <= 10f) {
                    string msg = "Entered within 10mins of alarm time";
                    StartCoroutine(DisplayNotifs(msg));
                    CatfoodManager.instance.IncreaseCatfood(10);
                }
            }

        }
    }

    public void InstantiateCats() {
        catControl = Instantiate(catPrefab, Vector3.zero, Quaternion.identity).GetComponent<CatController>();
        catControl.Initialize();
    }

    public void ButtonPressBefore(CatState state) {
        catControl.ButtonBefore();
        catControl.TransitionToNextState(state);
        ButtonControl(false);
    }

    public void ButtonPressAfter() {
        ButtonControl(true);
        catControl.RestartCat();
    }

    // When studying, player should not be able to press any buttons
    public void ButtonControl(bool active) {
        foreach (GameObject obj in buttons) {
            Button butt = obj.GetComponent<Button>();
            butt.interactable = active;
        }
    }

    public IEnumerator DisplayNotifs(string str) {
        notifs.enabled = true;
        notifsExist = true;
        LeanTween.moveLocalX(notifs.gameObject, -307f, 0.2f);
        notifs.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().text = str;
        yield return new WaitForSeconds(5);
        if (notifsExist) {
            StartCoroutine(Skip());
        }
    }

    private void SkipNotifs() {
        StartCoroutine(Skip());
    }

    IEnumerator Skip() {
        notifsExist = false;
        LeanTween.moveLocalX(notifs.gameObject, -1390f, 0.2f);
        yield return new WaitForSeconds(0.2f);
        notifs.enabled = false;
    }

    public IEnumerator EndStudy() {
        Debug.Log(CatfoodManager.instance);
        int catfoodEarned = CatfoodManager.instance.earnedCatfood;
        int xpEarned = CatfoodManager.instance.earnedXP;
        string msg;
        if (quitStudy) {
            msg = "Quit study session... Try again?";
            quitStudy = false;
        } else {
            if (xpEarned == 0) {
                msg = $"Study session ended! Try studying longer for more rewards :)";
            } else {
                msg = $"Study session ended!\n+{catfoodEarned} catfood, +{xpEarned} XP";
            }
        }
        StartCoroutine(DisplayNotifs(msg));

        // Wait for next frame so CatfoodManager finish Awake()
        yield return null;
        CatfoodManager.instance.IncreaseCatfood(catfoodEarned);
        if (xpEarned != 0) {
            StatsManager.instance.AddXP(xpEarned);
            CatMeow.instance.Meow();
        }

        Debug.Log("ENDED");
    }
}
