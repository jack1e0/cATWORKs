using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using Unity.Notifications.Android;

public class StudyTimerCustom : MonoBehaviour {
    [SerializeField] private GameObject screen;
    [SerializeField] private GameObject title;
    [SerializeField] private GameObject timeInput;
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private Button inputButton;
    [SerializeField] private TMP_Text timer;
    [SerializeField] private Image fill;
    [SerializeField] private GameObject skip;

    [SerializeField] private GameObject popUp;
    [SerializeField] private GameObject quitPopUp;
    [Space(10)]

    [SerializeField] private GameObject cat0;
    [SerializeField] private GameObject cat1;
    [SerializeField] private GameObject cat2;
    private GameObject studyCat;

    [Header("Study Music")]
    private int musicIndex;
    [SerializeField] private AudioClip[] studyMusics;
    [SerializeField] private AudioSource audSource;
    [SerializeField] private GameObject musicButton;
    [SerializeField] private Button forward;
    [SerializeField] private Button backward;
    [SerializeField] private TMP_Text musicName;


    private float duration;
    private float durationLeftInSecs;
    private float totalDuration;
    private Animator studyCatAnim;
    private Button studyCatButton;

    private int catfoodEarned;
    private Coroutine runningCoroutine;

    private bool isCoroutineRunning;

    private void Awake() {
        isCoroutineRunning = false;
        inputButton.interactable = false;
        quitPopUp.SetActive(false);
        popUp.SetActive(false);

        skip.GetComponent<Button>().onClick.AddListener(Skip);
        musicButton.GetComponent<Button>().onClick.AddListener(PlayMusic);
        forward.onClick.AddListener(Forward);
        backward.onClick.AddListener(Backward);

        skip.SetActive(false);
        musicButton.SetActive(false);
        forward.gameObject.SetActive(false);
        backward.gameObject.SetActive(false);
        musicName.enabled = false;

        title.GetComponent<TMP_Text>().text = "Custom";
        if (SceneTransition.instance.user.growth == 0) {
            studyCat = Instantiate(cat0, screen.transform);
        } else if (SceneTransition.instance.user.growth == 1) {
            studyCat = Instantiate(cat1, screen.transform);
        } else {
            studyCat = Instantiate(cat2, screen.transform);
        }
        studyCatAnim = studyCat.GetComponent<Animator>();
        studyCatButton = studyCat.GetComponent<Button>();
        studyCat.SetActive(false);

        studyCatButton.onClick.AddListener(StudyCatTap);
    }


    // Reading user input
    public void ReadInput(string number) {
        int value;
        bool isInt = int.TryParse(number, out value);
        if (!isInt || value < 0 || value > 500) {
            inputButton.interactable = false;
            inputField.text = string.Empty;
        } else {
            inputButton.interactable = true;
            this.duration = value;
        }
    }

    // locking in user input and start study
    public void EnterInput() {
        timeInput.SetActive(false);
        studyCat.SetActive(true);
        studyCatButton.interactable = false;
        timer.text = duration.ToString();
        durationLeftInSecs = duration * 60f;
        totalDuration = duration;

        runningCoroutine = StartCoroutine(StartStudy());
        skip.SetActive(true);
        musicButton.SetActive(true);
        forward.gameObject.SetActive(true);
        backward.gameObject.SetActive(true);
        musicName.enabled = true;

        BGM.instance.StopBGM();
        musicIndex = 0;
        audSource.clip = studyMusics[musicIndex];
        musicName.text = studyMusics[musicIndex].name;
        audSource.Play();

    }

    private IEnumerator StartStudy() {
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
            yield return new WaitForSeconds(1f);
        }

        FinishStudy();
    }

    private void FinishStudy() {
        isCoroutineRunning = false;
        studyCatAnim.SetBool("StudyFinish", true);
        studyCatButton.interactable = true;
    }

    public void StudyCatTap() {
        StartCoroutine(ChangeScene());
    }

    private IEnumerator ChangeScene() {
        CatfoodManager.instance.CalculateCatfood(duration);
        CatfoodManager.instance.CalculateXP(duration);
        RoomSceneManager.instance.justStudied = true;
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
        FinishStudy();
    }

    public void Back() {
        StartCoroutine(NoPopUp(popUp));
        duration = durationLeftInSecs / 60f;
        Debug.Log("duration: " + duration);
        runningCoroutine = StartCoroutine(StartStudy());
    }

    private void OnApplicationPause(bool pauseStatus) {
        if (isCoroutineRunning && pauseStatus) {
            if (runningCoroutine != null) {
                StopCoroutine(runningCoroutine);
            }
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
        duration = -1;
        RoomSceneManager.instance.quitStudy = true;
        StartCoroutine(ChangeScene());
    }

    public void Unquit() {
        StartCoroutine(NoPopUp(quitPopUp));
        duration = durationLeftInSecs / 60f;
        Debug.Log("duration: " + duration);
        runningCoroutine = StartCoroutine(StartStudy());
    }

    IEnumerator PopUp() {
        popUp.SetActive(true);
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
