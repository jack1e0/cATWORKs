using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class StudyTimerCustom : MonoBehaviour {

    [SerializeField] private GameObject title;
    [SerializeField] private GameObject timeInput;
    [SerializeField] private GameObject studyCat;
    [SerializeField] private GameObject timeDisplay;
    [SerializeField] private GameObject timeFill;
    [SerializeField] private GameObject skip;

    [SerializeField] private GameObject popUp;

    private float duration;
    private float durationLeftInSecs;
    private float totalDuration;
    private TMP_Text timer;
    private Image fill;
    private Animator studyCatAnim;
    private Button studyCatButton;

    private int catfoodEarned;
    private Coroutine runningCoroutine;

    private void Awake() {
        skip.GetComponent<Button>().onClick.AddListener(Skip);
        skip.SetActive(false);
        popUp.SetActive(false);

        title.GetComponent<TMP_Text>().text = "Custom";
        studyCat.SetActive(false);
        timer = timeDisplay.GetComponent<TMP_Text>();
        fill = timeFill.GetComponent<Image>();
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

        // // Testing
        // catfoodEarned = CatfoodManager.instance.CalculateCatfood(1);
        // StudyCatTap();
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
        CatfoodManager.instance.CalculateCatfood(duration);
        CatfoodManager.instance.CalculateXP(duration);
    }

    public void StudyCatTap() {
        StartCoroutine(ChangeScene());
    }

    private IEnumerator ChangeScene() {
        CatBehaviourManager.instance.justStudied = true;
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene("RoomScene");
    }

    public void Skip() {
        StopCoroutine(runningCoroutine);
        popUp.SetActive(true);
    }

    public void Leave() {
        popUp.SetActive(false);
        duration = -1;
        FinishStudy();
    }

    public void Back() {
        popUp.SetActive(false);
        duration = durationLeftInSecs / 60f;
        Debug.Log("duration: " + duration);
        runningCoroutine = StartCoroutine(StartStudy());
    }
}
