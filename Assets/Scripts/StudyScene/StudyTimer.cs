using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using Unity.Notifications.Android;

public class StudyTimer : MonoBehaviour {
    [SerializeField] private GameObject screen;
    [SerializeField] private TMP_Text title;
    [SerializeField] private GameObject studyCat;
    [SerializeField] private TMP_Text timer;
    private GameObject timeFill;
    [SerializeField] private GameObject parent;
    private GameObject timeline;
    [SerializeField] private GameObject skip;
    [SerializeField] private GameObject popUp;
    [SerializeField] private TMP_Text popupText;
    [SerializeField] private GameObject quitPopUp;
    [Space(10)]

    [Header("Study Music")]
    private int musicIndex;
    [SerializeField] private AudioClip[] studyMusics;
    [SerializeField] private AudioSource audSource;
    [SerializeField] private GameObject musicButton;
    [SerializeField] private Button forward;
    [SerializeField] private Button backward;
    [SerializeField] private TMP_Text musicName;

    private string studyName;
    private float[] studyStages;
    private int currStage;
    private float duration;
    private float durationLeftInSecs;
    private float totalDuration;
    private float durationStudied;
    private Image fill;
    private Animator studyCatAnim;
    private Button studyCatButton;

    private int catfoodEarned;
    private Coroutine runningCoroutine;

    private bool isCoroutineRunning;
    private bool isBreak;


    private void Awake() {
        quitPopUp.SetActive(false);
        popUp.SetActive(false);
        currStage = 0;
        durationStudied = 0;

        skip.GetComponent<Button>().onClick.AddListener(Skip);
        musicButton.GetComponent<Button>().onClick.AddListener(PlayMusic);
        forward.onClick.AddListener(Forward);
        backward.onClick.AddListener(Backward);

        BGM.instance.StopBGM();
        musicIndex = 0;
        audSource.clip = studyMusics[musicIndex];
        musicName.text = studyMusics[musicIndex].name;
        audSource.Play();

        studyName = TechniqueManager.instance.techniqueData.techniqueName;
        studyStages = TechniqueManager.instance.techniqueData.studyStages;

        studyCatAnim = studyCat.GetComponent<Animator>();
        studyCatButton = studyCat.GetComponent<Button>();

        studyCatButton.onClick.AddListener(StudyCatTap);

        timeFill = Instantiate(TechniqueManager.instance.techniqueData.circularBar, parent.transform);
        fill = timeFill.transform.GetChild(1).GetComponent<Image>();

        timeline = Instantiate(TechniqueManager.instance.techniqueData.timeLine, screen.transform);
        InstantiateTimer(this.currStage);
        runningCoroutine = StartCoroutine(StartTiming());
    }

    private void Start() {
        // Creating Android Notification Channel to send messages through
        var channel = new AndroidNotificationChannel() {
            Id = "warning_channel",
            Name = "Warning Channel",
            Importance = Importance.Default,
            Description = "Notification upon exit study",
        };
        AndroidNotificationCenter.RegisterNotificationChannel(channel);
    }

    private void InstantiateTimer(int currStage) {
        duration = studyStages[currStage];
        durationLeftInSecs = duration * 60f;
        totalDuration = duration;

        if (currStage % 2 != 0) { // if break time
            title.text = "Break!";
            isBreak = true;
            Color breakColor = new Color(Constants.breakColourR, Constants.breakColourG, Constants.breakColourB);
            fill.color = breakColor;
            timer.color = breakColor;
        } else {
            isBreak = false;
            title.text = studyName;
            Color studyColor = new Color(Constants.studyColourR, Constants.studyColourG, Constants.studyColourB);
            fill.color = studyColor;
            timer.color = studyColor;
        }

        studyCatButton.interactable = false;
        studyCatAnim.SetBool("StudyFinish", false);
    }

    private IEnumerator StartTiming() {
        isCoroutineRunning = true;
        float tempDuration = duration * 60f;
        while (tempDuration >= 0) {
            float minutes = Mathf.Floor(tempDuration / 60f);
            if (minutes < 10) {
                // Making nice display for time (00:00)
                timer.text = $"{0}{minutes}:{tempDuration % 60:00}";
            } else {
                timer.text = $"{Mathf.Floor(tempDuration / 60f)}:{tempDuration % 60:00}";
            }

            fill.fillAmount = Mathf.InverseLerp(totalDuration * 60f, 0, tempDuration);
            tempDuration--;
            durationLeftInSecs--;

            if (currStage % 2 == 0) { // if not break time
                durationStudied++;
            }
            yield return new WaitForSeconds(1f);
        }
        StageEnd();
    }

    private void StageEnd() {
        isCoroutineRunning = false;
        currStage++;
        studyCatAnim.SetBool("StudyFinish", true);
        studyCatButton.interactable = true;
        skip.GetComponent<Button>().interactable = false;
    }

    public void StudyCatTap() {
        if (currStage <= studyStages.Length - 1) {
            NextStage();
        } else {
            StartCoroutine(ChangeScene());
        }
    }

    private void NextStage() {
        InstantiateTimer(this.currStage);
        timeline.GetComponent<ShiftStudyStage>().ShiftIndicator();
        skip.GetComponent<Button>().interactable = true;
        runningCoroutine = StartCoroutine(StartTiming());
    }

    private IEnumerator ChangeScene() {
        CatfoodManager.instance.CalculateCatfood(durationStudied / 60f);
        CatfoodManager.instance.CalculateXP(durationStudied / 60f);
        CatBehaviourManager.instance.justStudied = true;
        yield return new WaitForSeconds(0.5f);
        audSource.Stop();
        BGM.instance.isPlaying = true;
        BGM.instance.audSource.Play();
        SceneTransition.instance.ChangeScene("RoomScene");
    }

    public void PlayMusic() {
        if (audSource.isPlaying) {
            audSource.Pause();
            Color grey = new Color(1, 1, 1, 0.3f);
            musicButton.GetComponent<Image>().color = grey;
            forward.GetComponent<Image>().color = grey;
            backward.GetComponent<Image>().color = grey;
        } else {
            audSource.Play();
            musicButton.GetComponent<Image>().color = Color.white;
            forward.GetComponent<Image>().color = Color.white;
            backward.GetComponent<Image>().color = Color.white;
        }
    }

    public void Forward() {
        if (audSource.isPlaying) {
            audSource.Stop();
            if (musicIndex == studyMusics.Length - 1) {
                musicIndex = 0;
            } else {
                musicIndex++;
            }
            audSource.clip = studyMusics[musicIndex];
            musicName.text = studyMusics[musicIndex].name;
            Canvas.ForceUpdateCanvases();
            audSource.Play();
        }
    }

    public void Backward() {
        if (audSource.isPlaying) {
            audSource.Stop();
            if (musicIndex == 0) {
                musicIndex = studyMusics.Length - 1;
            } else {
                musicIndex--;
            }
            audSource.clip = studyMusics[musicIndex];
            musicName.text = studyMusics[musicIndex].name;
            Canvas.ForceUpdateCanvases();
            audSource.Play();
        }
    }

    public void Skip() {
        StopCoroutine(runningCoroutine);
        isCoroutineRunning = false;
        StartCoroutine(PopUp());
    }

    public void Leave() {
        StartCoroutine(NoPopUp(popUp));
        duration = -1;
        StageEnd();
    }

    public void Back() {
        StartCoroutine(NoPopUp(popUp));
        duration = durationLeftInSecs / 60f;
        Debug.Log("duration: " + duration);
        runningCoroutine = StartCoroutine(StartTiming());
    }

    private void OnApplicationPause(bool pauseStatus) {
        if (isCoroutineRunning && pauseStatus) {
            StopCoroutine(runningCoroutine);
            quitPopUp.SetActive(true);

            AndroidNotification warning = new AndroidNotification();
            warning.Title = "Come back!";
            warning.Text = "You were in the middle of a study session!";
            warning.FireTime = System.DateTime.Now;

            AndroidNotificationCenter.CancelAllDisplayedNotifications();
            AndroidNotificationCenter.SendNotification(warning, "warning_channel");
        }
    }

    public void Quit() {
        durationStudied = -1;
        CatBehaviourManager.instance.quitStudy = true;
        StartCoroutine(ChangeScene());
    }

    public void Unquit() {
        StartCoroutine(NoPopUp(quitPopUp));
        duration = durationLeftInSecs / 60f;
        Debug.Log("duration: " + duration);
        runningCoroutine = StartCoroutine(StartTiming());
    }

    IEnumerator PopUp() {
        popUp.SetActive(true);
        if (isBreak) {
            popupText.text = "Are you sure? Breaks are important as well!";
        } else {
            popupText.text = "Are you sure? Your remaining rewards for this stage will be lost!";
        }

        LeanTween.scale(popUp.transform.GetChild(0).gameObject, new Vector3(1.1f, 1.1f, 1.1f), 0.1f);
        yield return new WaitForSeconds(0.1f);
        LeanTween.scale(popUp.transform.GetChild(0).gameObject, new Vector3(1, 1, 1), 0.1f);
        yield return new WaitForSeconds(0.1f);
    }

    IEnumerator NoPopUp(GameObject pop) {
        LeanTween.scale(pop.transform.GetChild(0).gameObject, new Vector3(0.9f, 0.9f, 0.9f), 0.1f);
        yield return new WaitForSeconds(0.1f);
        pop.SetActive(false);
    }
}
