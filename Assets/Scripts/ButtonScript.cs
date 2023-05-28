using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class ButtonScript : MonoBehaviour
{

    public float timer = 5f;
    public float currTime = 0f;
    // public Text countdown;
    public TMP_Text countdown;
    public GameObject timerScreen;

    // Start is called before the first frame update
    void Start()
    {
        currTime = timer;
        timerScreen.SetActive(false);

    }

    public void StartCountdown()
    {
        timerScreen.SetActive(true);

        StartCoroutine(Countdown(5));
    }

    private IEnumerator Countdown(float timeInSeconds) {
        while (timeInSeconds >= 0) {
            currTime = timeInSeconds;
            countdown.text = currTime.ToString("0");
            timeInSeconds--;
            yield return new WaitForSeconds(1);
        }
        timerScreen.SetActive(false);
    }
}
