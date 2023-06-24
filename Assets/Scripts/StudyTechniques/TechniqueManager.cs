using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TechniqueManager : MonoBehaviour {

    public static TechniqueManager instance;
    public static TechniqueDetails techniqueData;

    private void Awake() {
        DontDestroyOnLoad(this);

        // if (instance == null) {
        //     instance = this;
        // } else { // if there is already another previous instance of manager, keep that.
        //     instance = this;
        //     Destroy(instance.gameObject);
        // }

        if (instance != null && instance != this) {
            Destroy(this);
        } else {
            instance = this;
        }
    }

    public void CustomStudySequence() {
        CatBehaviourManager.instance.ButtonPressBefore(CatState.NONE);
        SceneManager.LoadScene("StudySceneCustom");

        // Load respective scriptable object
        TechniqueDetails obj = Resources.Load<TechniqueDetails>("Custom");
        techniqueData = Instantiate(obj);
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
