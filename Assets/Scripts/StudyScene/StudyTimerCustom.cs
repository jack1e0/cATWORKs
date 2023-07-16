using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using Unity.Notifications.Android;

public class StudyTimerCustom : MonoBehaviour {

    [SerializeField] private GameObject title;
    [SerializeField] private GameObject timeInput;
    [SerializeField] private GameObject studyCat;
    [SerializeField] private TMP_Text timer;
    [SerializeField] private Image fill;
    [SerializeField] private GameObject skip;
    [SerializeField] private GameObject musicButton;

    [SerializeField] private GameObject popUp;

    private float duration;
    private float durationLeftInSecs;
    private float totalDuration;
    private Animator studyCatAnim;
    private Button studyCatButton;

    private int catfoodEarned;
    private Coroutine runningCoroutine;

    [SerializeField] private GameObject quitPopUp;

    private void Awake() {
        quitPopUp.SetActive(false);
        skip.GetComponent<Button>().onClick.AddListener(Skip);
        musicButton.GetComponent<Button>().onClick.AddListener(ControlMusic);
        skip.SetActive(false);
        musicButton.SetActive(false);
        popUp.SetActive(false);

        title.GetComponent<TMP_Text>().text = "Custom";
        studyCat.SetActive(false);
        studyCatAnim = studyCat.GetComponent<Animator>();
        studyCatButton = studyCat.GetComponent<Button>();

        studyCatButton.onClick.AddListener(StudyCatTap);
    }


    // Reading user input
    public void ReadInput(string number) {
        duration = int.Parse(number);
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
    }

    private IEnumerator StartStudy() {
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
        studyCatAnim.SetBool("StudyFinish", true);
        studyCatButton.interactable = true;
    }

    public void StudyCatTap() {
        StartCoroutine(ChangeScene());
    }

    private IEnumerator ChangeScene() {
        CatfoodManager.instance.CalculateCatfood(duration);
        CatfoodManager.instance.CalculateXP(duration);
        CatBehaviourManager.instance.justStudied = true;
        yield return new WaitForSeconds(0.5f);
        SceneTransition.instance.ChangeScene("RoomScene");
    }

    public void ControlMusic() {
        if (BGM.instance.isPlaying) {
            BGM.instance.isPlaying = false;
            BGM.instance.audSource.Pause();
            musicButton.GetComponent<Image>().color = new Color(1, 1, 1, 0.3f);
        } else {
            BGM.instance.isPlaying = true;
            BGM.instance.audSource.Play();
            musicButton.GetComponent<Image>().color = new Color(1, 1, 1, 1);
        }
    }

    public void Skip() {
        StopCoroutine(runningCoroutine);
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
        if (pauseStatus) {
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
        duration = -1;
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
