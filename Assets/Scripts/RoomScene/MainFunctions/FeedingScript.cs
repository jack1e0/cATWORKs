using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FeedingScript : MonoBehaviour {
    private Button button;
    [SerializeField] private float eatDuration = 5f;
    private bool isFeed;
    [SerializeField] private Image catfood;

    private void Awake() {
        isFeed = false;
        button = gameObject.GetComponent<Button>();
    }

    void Start() {
        button.onClick.AddListener(StartEat);
        catfood.enabled = false;
    }

    public void StartEat() {
        bool canFeed = CatfoodManager.instance.DecreaseCatfood(1);
        if (!canFeed) {
            string msg = "Catfood insufficient! Study to earn more.";
            StartCoroutine(CatBehaviourManager.instance.DisplayNotifs(msg));
        }

        if (!isFeed && canFeed) {
            string msg = "Cat eating!";
            StartCoroutine(CatBehaviourManager.instance.DisplayNotifs(msg));

            catfood.enabled = true;
            Color col = catfood.color;
            catfood.color = new Color(col.r, col.g, col.b, 0);
            LeanTween.alpha(catfood.rectTransform, 1, 0.1f);

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

        LeanTween.alpha(catfood.rectTransform, 0, 0.1f);
        yield return new WaitForSeconds(0.1f);
        catfood.enabled = false;
        Debug.Log("Eating done!");
        CatMeow.instance.Meow();

        StatsManager.instance.ChangeHappy(5);
        CatBehaviourManager.instance.ButtonPressAfter();
        isFeed = false;
    }
}
