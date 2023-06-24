using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TechniqueManager : MonoBehaviour {

    public static TechniqueManager instance;
    public static TechniqueDetails techniqueData;

    private GameObject[] techniques;

    private void Awake() {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(this);
        } else {
            Destroy(this);
            instance = GameObject.FindGameObjectWithTag("Manager").GetComponent<TechniqueManager>();
        }
        instance.Instantiate();
    }

    public void Instantiate() {
        techniques = new GameObject[4] {
            GameObject.FindGameObjectWithTag("Technique1"),
            GameObject.FindGameObjectWithTag("Technique2"),
            GameObject.FindGameObjectWithTag("Technique3"),
            GameObject.FindGameObjectWithTag("Technique4")
        };

        techniques[0].GetComponent<Button>().onClick.AddListener(Custom);
        techniques[0].GetComponent<Button>().onClick.AddListener(Pomodoro);
        techniques[0].GetComponent<Button>().onClick.AddListener(Timeblocking);
        techniques[0].GetComponent<Button>().onClick.AddListener(Eisenhower);
    }

    public void Custom() {
        CatBehaviourManager.instance.ButtonPressBefore(CatState.NONE);
        SceneManager.LoadScene("StudySceneCustom");

        // Load respective scriptable object
        TechniqueDetails obj = Resources.Load<TechniqueDetails>("Custom");
        techniqueData = Instantiate(obj);
    }

    public void Pomodoro() {

    }

    public void Timeblocking() {

    }

    public void Eisenhower() {

    }

    // private IEnumerator StudyCoroutine() {
    //     StudyScript.instance.isStudying = true;

    //     CatBehaviourManager.instance.ButtonPressBefore(CatState.STUDY);
    //     float timeInSeconds = studyDuration;
    //     while (timeInSeconds >= 0) {
    //         timeInSeconds--;
    //         yield return new WaitForSeconds(1);
    //     }

    //     // Random behaviour resumes after studying
    //     string msg = "Study session completed! Press button to claim catfood.";
    //     StartCoroutine(CatBehaviourManager.instance.DisplayNotifs(msg));
    //     StudyScript.instance.animator.SetBool("StudyFinish", true);
    //     StudyScript.instance.button.interactable = true;
    // }
}
