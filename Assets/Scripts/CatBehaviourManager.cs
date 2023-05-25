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

    private CatState currentState;
    private float timeSinceStateChange;
    private GameObject currCat;

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
        StartCoroutine(ChangeStates());
    }

    private IEnumerator ChangeStates() {
        while (true) {
            timeSinceStateChange++;
            yield return new WaitForSeconds(1);

            if (timeSinceStateChange > GetStateDuration(currentState)) {
                CatState nextState = GetNextRandomState(currentState);
                Debug.Log(nextState);
                TransitionToNextState(nextState);
                timeSinceStateChange = 0;
            }
        }
    }

    private CatState GetRandomState() {
        int rand = Random.Range(0, 2);
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
                return Instantiate(sleepPrefab, new Vector3(-1.17f, -2.4f, 0), Quaternion.identity);
            case CatState.SIT:
                return Instantiate(sitPrefab, new Vector3(1.5f, -3f, 0), Quaternion.identity);
            default:
                return null;
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
}
