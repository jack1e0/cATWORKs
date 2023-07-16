using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Buttons : MonoBehaviour {
    void Awake() {
        gameObject.GetComponent<Button>().onClick.AddListener(StartTechnique);
    }

    public void StartTechnique() {
        SceneTransition.instance.ChangeScene("StudyScene");
    }

    public void Back() {
        SceneTransition.instance.ChangeScene("RoomScene");
    }
}
