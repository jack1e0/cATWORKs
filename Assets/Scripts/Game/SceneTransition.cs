using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour {
    public static SceneTransition instance;
    private Image blocker;
    private AudioSource audSource;

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
        audSource.volume = 0.15f;
        audSource.Play();
        StartCoroutine(FadeIn(sceneName));
    }

    IEnumerator FadeOut() {
        Debug.Log("fading in");
        LeanTween.alpha(blocker.rectTransform, 0, Constants.sceneTransitionTime);
        yield return new WaitForSeconds(Constants.sceneTransitionTime);
    }

    IEnumerator FadeIn(string name) {
        Debug.Log("fading out");
        LeanTween.alpha(blocker.rectTransform, 1, Constants.sceneTransitionTime);
        yield return null;
        SceneManager.LoadScene(name);
    }


}
