using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public enum CatState {
    NONE,
    STUDY,
    SLEEP,
    SIT,
    EAT
}

/// <summary>
/// Controls the state of cat using coroutines that start upon entering game
/// TODO: implement animation between states
/// TODO: implement periodic cat meowing/ squeaking gibberish (similar to animal crossing?)
/// </summary>
public class CatBehaviourManager : MonoBehaviour {

    [Header("Prefabs")]
    [SerializeField] private GameObject studyPrefab;
    [SerializeField] private GameObject sleepPrefab;
    [SerializeField] private GameObject sitPrefab;
    [Space(10)]

    private GameObject[] buttons;

    private TMP_Text notifs;

    private CatState currentState;
    private float timeSinceStateChange;
    private GameObject currCat;
    private Coroutine randomStateCoroutine;

    public bool justStudied;

    // Making a singleton class
    public static CatBehaviourManager instance;

    void Awake() {

        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(this);
        } else {
            Destroy(gameObject);
            instance = GameObject.FindGameObjectWithTag("Manager").GetComponent<CatBehaviourManager>();
        }

        // if (instance != null && instance != this) {
        //     Destroy(this);
        //     return;
        // } else if (instance == null) {
        //     instance = this;
        //     DontDestroyOnLoad(this);
        // }

        // buttons = GameObject.FindGameObjectsWithTag("Button");
        // notifs = GameObject.FindGameObjectWithTag("Notifs").GetComponent<TMP_Text>();

        // notifs.enabled = false;
        // timeSinceStateChange = 0;

        // // TODO: instead of setting a random state upon start, possible to keep track of state that scene is previously left off at
        // currentState = GetRandomState();

        // // Instantiate corresponding cat in scene
        // currCat = GetCat(currentState);
        // randomStateCoroutine = StartCoroutine(RandomStateChange());
        // Debug.Log("started random state change");

        // Debug.Log(justStudied);
        // if (justStudied) {
        //     Debug.Log("HERE");
        //     StartCoroutine(EndStudy());
        //     justStudied = false;
        // }
    }

    private void OnEnable() {
        Debug.Log("Instantiating catbehaviour");
        SceneManager.sceneLoaded += Instantiate;
    }

    private void OnDisable() {
        SceneManager.sceneLoaded -= Instantiate;
    }

    public void Instantiate(Scene scene, LoadSceneMode mode) {
        if (scene.name == "RoomScene") {
            Debug.Log("here");
            randomStateCoroutine = null;
            buttons = GameObject.FindGameObjectsWithTag("Button");
            notifs = GameObject.FindGameObjectWithTag("Notifs").GetComponent<TMP_Text>();

            notifs.enabled = false;
            timeSinceStateChange = 0;
            currentState = GetRandomState();

            // Instantiate corresponding cat in scene
            currCat = GetCat(currentState);
            randomStateCoroutine = StartCoroutine(RandomStateChange());
            Debug.Log("started random state change");

            if (justStudied) {
                Debug.Log("HERE");
                StartCoroutine(EndStudy());
                justStudied = false;
            }
        } else {
            StopAllCoroutines();
            randomStateCoroutine = null;
        }
    }

    private CatState GetRandomState() {
        int rand = UnityEngine.Random.Range(0, 2);
        switch (rand) {
            case 0:
                return CatState.SLEEP;
            case 1:
                return CatState.SIT;
            default:
                return CatState.NONE;
        }
    }

    private GameObject GetCat(CatState state) {
        switch (state) {
            case CatState.SLEEP:
                return Instantiate(sleepPrefab, new Vector3(-1.05f, -2.8f, 0), Quaternion.identity);
            case CatState.EAT:
                return Instantiate(sitPrefab, new Vector3(1.5f, -3f, 0), Quaternion.identity);
            case CatState.STUDY:
                return Instantiate(studyPrefab, new Vector3(0.5f, -0.7f, 0), Quaternion.identity);
            case CatState.SIT:
                return Instantiate(sitPrefab, new Vector3(0.45f, -3.9f, 0), Quaternion.identity);
            default:
                return null;
        }
    }


    private IEnumerator RandomStateChange() {
        while (true) {
            timeSinceStateChange++;
            yield return new WaitForSeconds(1);

            if (timeSinceStateChange > GetStateDuration(currentState)) {
                CatState nextState = GetNextRandomState(currentState);
                TransitionToNextState(nextState);
                timeSinceStateChange = 0;
            }
        }
    }

    // returns how long a state should last in seconds (for testing, may not be real implementation)
    // as of now, sleeping last longer than sitting, but short enough such that code is testable
    private float GetStateDuration(CatState state) {
        switch (state) {
            case CatState.SLEEP:
                return UnityEngine.Random.Range(10, 20);
            //return 5;
            case CatState.SIT:
                return UnityEngine.Random.Range(5, 10);
            //return 5;
            default:
                return -1;
        }
    }

    // generate the next random state that is not the current state
    private CatState GetNextRandomState(CatState excludeState) {
        CatState state = excludeState;
        while (state.Equals(excludeState)) {
            state = GetRandomState();
        }
        return state;
    }

    private void TransitionToNextState(CatState nextState) {
        if (nextState == CatState.NONE) {
            currCat.SetActive(false);
        } else {
            StartCoroutine(FadeTransition(currCat, nextState));
            currentState = nextState;
        }
    }

    private IEnumerator FadeTransition(GameObject curr, CatState nextState) {
        if (curr != null) {
            Image img = curr.GetComponent<Image>();
            while (img.color.a > 0) {
                Color color = img.color;
                color.a -= 0.08f;
                img.color = color;
                yield return null;
            }
        }

        // Destroy all visible cats in scene (just in case there are more than one)
        GameObject[] activeCats = GameObject.FindGameObjectsWithTag("Cat");
        foreach (GameObject cat in activeCats) {
            Destroy(cat);
        }
        // Set visibility of next to invisible first
        GameObject next = GetCat(nextState);
        currCat = next;
        Color nextColor = next.GetComponent<Image>().color;
        nextColor.a = 0;
        next.GetComponent<Image>().color = nextColor;

        yield return new WaitForSeconds(0.5f);
        StartCoroutine(FadeIn(next));
    }

    private IEnumerator FadeIn(GameObject next) {
        if (next != null) {
            Image img = next.GetComponent<Image>();
            while (img.color.a < 1) {
                Color color = img.color;
                color.a += 0.08f;
                img.color = color;
                yield return null;
            }
        }
    }

    public void ButtonPressBefore(CatState state) {
        StopCoroutine(randomStateCoroutine);
        TransitionToNextState(state);
        ButtonControl(false);
    }

    public void ButtonPressAfter() {
        currCat.SetActive(true);
        ButtonControl(true);
        randomStateCoroutine = StartCoroutine(RandomStateChange());
    }

    // When studying, player should not be able to press any buttons
    public void ButtonControl(bool active) {
        foreach (GameObject obj in buttons) {
            Button butt = obj.GetComponent<Button>();
            butt.interactable = active;
        }
    }

    public IEnumerator DisplayNotifs(string str) {
        notifs.enabled = true;
        notifs.text = str;
        yield return new WaitForSeconds(3);
        notifs.enabled = false;
    }

    public IEnumerator EndStudy() {
        Debug.Log(CatfoodManager.instance);
        int catfoodEarned = CatfoodManager.instance.toChange;
        string msg = "Study session ended! " + catfoodEarned + " Catfood earned!";
        StartCoroutine(DisplayNotifs(msg));

        // Wait for next frame so CatfoodManager finish Awake()
        yield return null;
        CatfoodManager.instance.IncreaseCatfood(catfoodEarned);
        CatBehaviourManager.instance.ButtonPressAfter();
        Debug.Log("ENDED");
    }



}
