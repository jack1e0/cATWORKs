using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StartButton : MonoBehaviour {
    // Start is called before the first frame update
    void Start() {
        gameObject.GetComponent<Button>().onClick.AddListener(StartTechnique);
    }

    public void StartTechnique() {
        SceneManager.LoadScene("StudyScene");
    }

    public void Back() {
        SceneManager.LoadScene("RoomScene");
    }
}
