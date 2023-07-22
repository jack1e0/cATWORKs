using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using Newtonsoft.Json;
using Firebase.Database;

public class SceneTransition : MonoBehaviour {
    public static SceneTransition instance;
    private Image blocker;
    private AudioSource audSource;

    [SerializeField] private AudioClip bgm;
    [SerializeField] private AudioClip drawingGame;
    [SerializeField] private AudioClip flappyCat;

    private string currScene;
    public UserData user;
    public bool firstEnteredRoom;

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
        BGM.instance.isPlaying = true;
        gameObject.GetComponent<Canvas>().worldCamera = Camera.main;
        blocker = GetComponent<Image>();
        audSource = GetComponent<AudioSource>();
        blocker.enabled = true;
        StartCoroutine(FadeOut());
    }

    public void ChangeScene(string sceneName) {
        if (sceneName == "FlappyCat") {
            Screen.orientation = ScreenOrientation.LandscapeLeft;
            BGM.instance.ChangeClip(flappyCat);
        } else if (sceneName == "DrawingGame") {
            Screen.orientation = ScreenOrientation.Portrait;
            BGM.instance.ChangeClip(drawingGame);
        } else {
            Screen.orientation = ScreenOrientation.Portrait;
            BGM.instance.ChangeClip(bgm);
        }

        if (SceneManager.GetActiveScene().name == "RoomScene" && sceneName == "DrawingGame") {
            BGM.instance.audSource.Play();
        }
        if (SceneManager.GetActiveScene().name == "DrawingGame" && sceneName == "RoomScene") {
            BGM.instance.audSource.Play();
        }

        this.audSource.Play();
        LeanTween.alpha(blocker.rectTransform, 0.5f, Constants.sceneExitTime);
        SceneManager.LoadScene(sceneName);
    }

    IEnumerator FadeOut() {
        LeanTween.alpha(blocker.rectTransform, 0, Constants.sceneEntranceTime);
        yield return new WaitForSeconds(Constants.sceneEntranceTime);
    }

    private void OnApplicationQuit() {
        UpdateDatabase();
    }

    private async void UpdateDatabase() {
        user.currXP = StatsManager.instance.currXP;
        user.currHappiness = StatsManager.instance.currHappy;
        user.level = StatsManager.instance.currLvl;

        string prevExitTime = JsonConvert.SerializeObject(user.prevExitTime);

        DatabaseReference DBreference = FirebaseDatabase.DefaultInstance.RootReference;

        await DBreference.Child("users").Child(user.userId).Child("prevExitTime").SetValueAsync(prevExitTime);

    }
}
