using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class SceneTransition : MonoBehaviour {
    public static SceneTransition instance;
    private Image blocker;
    private AudioSource audSource;

    [SerializeField] private AudioClip bgm;
    [SerializeField] private AudioClip drawingGame;
    [SerializeField] private AudioClip flappyCat;

    void Awake() {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(this);
        } else {
            Destroy(gameObject);
            instance = GameObject.FindGameObjectWithTag("SceneManager").GetComponent<SceneTransition>();
        }
    }

    private void OnEnable() {
        SceneManager.sceneLoaded += Instantiate;
    }

    private void OnDisable() {
        SceneManager.sceneLoaded -= Instantiate;
    }

    private void Instantiate(Scene scene, LoadSceneMode mode) {
        gameObject.GetComponent<Canvas>().worldCamera = Camera.main;
        blocker = GetComponent<Image>();
        audSource = GetComponent<AudioSource>();
        blocker.enabled = true;
        StartCoroutine(FadeOut());
    }

    public void ChangeScene(string sceneName) {
        if (sceneName == "FlappyCat") {
            Screen.orientation = ScreenOrientation.LandscapeLeft;
            BGM.instance.audSource.clip = flappyCat;
        } else if (sceneName == "DrawingGame") {
            Screen.orientation = ScreenOrientation.Portrait;
            BGM.instance.audSource.clip = drawingGame;
        } else {
            Screen.orientation = ScreenOrientation.Portrait;
            BGM.instance.audSource.clip = bgm;
        }
        audSource.volume = 0.5f;
        audSource.Play();
        if (!BGM.instance.isPlaying) {
            BGM.instance.isPlaying = true;
            BGM.instance.audSource.Play();
        }
        Debug.Log("fading in");
        LeanTween.alpha(blocker.rectTransform, 0.5f, Constants.sceneExitTime);
        SceneManager.LoadScene(sceneName);
    }

    IEnumerator FadeOut() {
        Debug.Log("fading out");
        LeanTween.alpha(blocker.rectTransform, 0, Constants.sceneEntranceTime);
        yield return new WaitForSeconds(Constants.sceneEntranceTime);
    }
}
