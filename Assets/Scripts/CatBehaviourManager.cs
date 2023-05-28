using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum CatState {
    NONE,
    STUDY,
    SLEEP,
    SIT
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

    [Header("Buttons")]
    [SerializeReference] private List<GameObject> buttons = new List<GameObject>();

    private CatState currentState;
    private float timeSinceStateChange;
    private GameObject currCat;
    private Coroutine randomStateCoroutine;
    public Text timer;

    // Making a singleton class
    public static CatBehaviourManager instance;

    void Awake() {
        if (instance == null) {
            instance = this;
        }
    }

    void Start() {
        timeSinceStateChange = 0;

        // TODO: instead of setting a random state upon start, possible to keep track of state that scene is previously left off at
        currentState = GetRandomState();

        // Instantiate corresponding cat in scene
        currCat = GetCat(currentState);
        randomStateCoroutine = StartCoroutine(RandomStateChange());
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
            case CatState.SIT:
                return Instantiate(sitPrefab, new Vector3(1.5f, -3f, 0), Quaternion.identity);
            case CatState.STUDY:
                return Instantiate(studyPrefab, new Vector3(0.5f, -0.7f, 0), Quaternion.identity);
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
                //Debug.Log(nextState);
                yield return null;
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
                //return Random.Range(20, 30);
                return 5;
            case CatState.SIT:
                //return Random.Range(5, 20);
                return 5;
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
        StartCoroutine(FadeTransition(currCat, GetCat(nextState)));
        currentState = nextState;
    }

    private IEnumerator FadeTransition(GameObject curr, GameObject next) {
        // Set visibility of next to invisible first
        Color nextColor = next.GetComponent<Image>().color;
        nextColor.a = 0;
        next.GetComponent<Image>().color = nextColor;

        // Proceed to fade out curr
        Image img = curr.GetComponent<Image>();
        while (img.color.a > 0) {
            Color color = img.color;
            color.a -= 0.05f;
            img.color = color;
            yield return null;
        }

        yield return new WaitForSeconds(0.8f);
        Destroy(curr);
        StartCoroutine(FadeIn(next));
    }

    private IEnumerator FadeIn(GameObject next) {
        currCat = next;
        Image img = next.GetComponent<Image>();
        while (img.color.a < 1) {
            Color color = img.color;
            color.a += 0.05f;
            img.color = color;
            yield return null;
        }
    }

    // Once Study() is triggered, 
    public void StudyButton() {
        ButtonControl(false);
        StopCoroutine(randomStateCoroutine);
        TransitionToNextState(CatState.STUDY);
        Debug.Log("10 second study session started!");
        StartCoroutine(Study(10));
    }

    private IEnumerator Study(float timeInSeconds) {
        while (timeInSeconds >= 0) {
            timeInSeconds--;
            yield return new WaitForSeconds(1);
        }

        // Random behaviour resumes after studying
        Debug.Log("Studying done!");
        ButtonControl(true);
        randomStateCoroutine = StartCoroutine(RandomStateChange());
    }

    public void FeedingButton() {
        ButtonControl(false);
        StopCoroutine(randomStateCoroutine);
        TransitionToNextState(CatState.SIT);
        Debug.Log("start eating!");
        StartCoroutine(Eat(10));
    }

    private IEnumerator Eat(float timeInSeconds) {
        while (timeInSeconds >= 0) {
            timeInSeconds--;
            yield return new WaitForSeconds(1);
        }

        Debug.Log("Eating done!");
        ButtonControl(true);
        randomStateCoroutine = StartCoroutine(RandomStateChange());
    }

    // When studying, player should not be able to press any buttons
    private void ButtonControl(bool active) {
        foreach (GameObject obj in buttons) {
            Button butt = obj.GetComponent<Button>();
            butt.interactable = active;
        }
    }
}
