using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Attached to Notebook gameobject
/// </summary>
public class StudyScript : MonoBehaviour {
    public float studyDuration;
    private bool isStudying;
    private bool endStudying;
    private Animator animator;
    private Button button;

    [SerializeField] private GameObject menuScreen;

    void Awake() {
        isStudying = false;
        animator = gameObject.GetComponent<Animator>();
        button = gameObject.GetComponent<Button>();
    }

    private void Start() {
        button.onClick.AddListener(MenuAppear);
        menuScreen.SetActive(false);
    }

    private void MenuAppear() {
        if (isStudying) {
            EndStudy();
        } else {
            menuScreen.SetActive(true);
        }
    }

    public void StudySequence() {
        string msg = "10s study session started!";
        StartCoroutine(CatBehaviourManager.instance.DisplayNotifs(msg));
        StartCoroutine(StudyCoroutine());
    }

    private IEnumerator StudyCoroutine() {
        isStudying = true;
        menuScreen.SetActive(false);
        CatBehaviourManager.instance.ButtonPressBefore(CatState.STUDY);
        float timeInSeconds = studyDuration;
        while (timeInSeconds >= 0) {
            timeInSeconds--;
            yield return new WaitForSeconds(1);
        }

        // Random behaviour resumes after studying
        string msg = "Study session completed! Press button to claim catfood.";
        StartCoroutine(CatBehaviourManager.instance.DisplayNotifs(msg));
        animator.SetBool("StudyFinish", true);
        button.interactable = true;
    }

    private void EndStudy() {
        // Placeholder value for increase in catfood after a study session
        CatfoodManager.instance.IncreaseCatfood(10);

        animator.SetBool("StudyFinish", false);
        isStudying = false;
        CatBehaviourManager.instance.ButtonPressAfter();
    }

    public void Back() {
        menuScreen.SetActive(false);
    }
}
