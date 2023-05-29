using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FeedingScript : MonoBehaviour {
    private Button button;
    [SerializeField] private float eatDuration = 5f;
    private bool isFeed;
    [SerializeField] private GameObject catfood;

    private void Awake() {
        isFeed = false;
        button = gameObject.GetComponent<Button>();
    }

    void Start() {
        button.onClick.AddListener(StartEat);
        catfood.SetActive(false);
    }

    public void StartEat() {
        bool canFeed = CatfoodManager.instance.DecreaseCatfood(10);
        if (!canFeed) {
            string msg = "Catfood insufficient! Study to earn more.";
            StartCoroutine(CatBehaviourManager.instance.DisplayNotifs(msg));
        }

        if (!isFeed && canFeed) {
            string msg = "Cat eating!";
            StartCoroutine(CatBehaviourManager.instance.DisplayNotifs(msg));
            catfood.SetActive(true);
            CatBehaviourManager.instance.ButtonPressBefore(CatState.EAT);
            Debug.Log("Start eating!");
            StartCoroutine(EatCoroutine());
        }
    }

    private IEnumerator EatCoroutine() {
        isFeed = true;
        float timeInSeconds = eatDuration;
        while (timeInSeconds >= 0) {
            timeInSeconds--;
            yield return new WaitForSeconds(1);
        }

        catfood.SetActive(false);
        Debug.Log("Eating done!");
        CatBehaviourManager.instance.ButtonPressAfter();
        isFeed = false;
    }
}
