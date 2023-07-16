using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TechniqueManager : MonoBehaviour {

    public static TechniqueManager instance;
    public TechniqueDetails techniqueData;

    private GameObject[] techniques;

    private void Awake() {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(this);
        } else {
            Destroy(this);
            instance = GameObject.FindGameObjectWithTag("Manager").GetComponent<TechniqueManager>();
        }
    }

    public void Instantiate() {
        techniques = new GameObject[4] {
            GameObject.FindGameObjectWithTag("Technique1"),
            GameObject.FindGameObjectWithTag("Technique2"),
            GameObject.FindGameObjectWithTag("Technique3"),
            GameObject.FindGameObjectWithTag("Technique4")
        };

        techniques[0].GetComponent<Button>().onClick.AddListener(Custom);
        techniques[1].GetComponent<Button>().onClick.AddListener(Pomodoro);
        techniques[2].GetComponent<Button>().onClick.AddListener(Timeblocking);
        techniques[3].GetComponent<Button>().onClick.AddListener(Eisenhower);
    }

    public void Custom() {
        // Load respective scriptable object
        TechniqueDetails obj = Resources.Load<TechniqueDetails>("Custom");
        techniqueData = Instantiate(obj);

        SceneTransition.instance.ChangeScene("StudySceneCustom");
    }

    public void Pomodoro() {
        TechniqueDetails obj = Resources.Load<TechniqueDetails>("Pomodoro");
        techniqueData = Instantiate(obj);

        SceneTransition.instance.ChangeScene("PomodoroScene");
    }

    public void Timeblocking() {

    }

    public void Eisenhower() {

    }
}
