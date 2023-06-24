using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class StudyTimerCustom : MonoBehaviour {
    private float duration;
    [SerializeField] private GameObject timeInput;
    [SerializeField] private GameObject studyCat;
    [SerializeField] private GameObject timeDisplay;
    [SerializeField] private GameObject timeFill;
    private TMP_Text timer;
    private Image fill;
    private Animator studyCatAnim;
    private Button studyCatButton;

    private int catfoodEarned;

    private void Awake() {
        studyCat.SetActive(false);
        timer = timeDisplay.GetComponent<TMP_Text>();
        fill = timeFill.GetComponent<Image>();
        studyCatAnim = studyCat.GetComponent<Animator>();
        studyCatButton = studyCat.GetComponent<Button>();
    }


    // Reading user input
    public void ReadInput(string number) {
        duration = int.Parse(number);
    }

    // locking in user input and start study
    public void EnterInput() {
        // timeInput.SetActive(false);
        // studyCat.SetActive(true);
        // studyCatButton.interactable = false;
        // timer.text = duration.ToString();
        // StartCoroutine(StartStudy());
        catfoodEarned = CatfoodManager.instance.CalculateCatfood(1);
        StudyCatTap();
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

            fill.fillAmount = Mathf.InverseLerp(duration * 60f, 0, tempDuration);
            tempDuration--;
            yield return new WaitForSeconds(1f);
        }

        FinishStudy();
    }

    private void FinishStudy() {
        studyCatAnim.SetBool("StudyFinish", true);
        studyCatButton.interactable = true;
        catfoodEarned = CatfoodManager.instance.CalculateCatfood(duration);
    }

    public void StudyCatTap() {
        StartCoroutine(ChangeScene());
    }

    private IEnumerator ChangeScene() {
        CatBehaviourManager.instance.justStudied = true;
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene("RoomScene");
    }



}
