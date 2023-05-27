using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Attached to Notebook gameobject
/// </summary>
public class Study : MonoBehaviour {
    public float studyDuration;
    private bool isStudying;
    private bool endStudying;
    private Animator animator;
    private Button button;

    void Awake() {
        isStudying = false;
        animator = gameObject.GetComponent<Animator>();
        button = gameObject.GetComponent<Button>();
    }

    private void Start() {
        button.onClick.AddListener(StudySequence);
    }

    private void StudySequence() {
        if (isStudying) {
            EndStudy();
        } else {
            StartCoroutine(StudyCoroutine());
        }
    }

    private IEnumerator StudyCoroutine() {
        isStudying = true;
        CatBehaviourManager.instance.ButtonPressBefore(CatState.STUDY);
        float timeInSeconds = studyDuration;
        while (timeInSeconds >= 0) {
            timeInSeconds--;
            yield return new WaitForSeconds(1);
        }

        // Random behaviour resumes after studying
        Debug.Log("Studying done!");
        animator.SetBool("StudyFinish", true);
        button.interactable = true;
    }

    private void EndStudy() {
        Debug.Log("Ended study session!");

        // Placeholder value for increase in catfood after a study session
        CatfoodManager.instance.UpdateCatfoodCount(10);

        animator.SetBool("StudyFinish", false);
        isStudying = false;
        CatBehaviourManager.instance.ButtonPressAfter();
    }
}
