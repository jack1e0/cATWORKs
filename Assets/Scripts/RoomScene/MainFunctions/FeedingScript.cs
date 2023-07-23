using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FeedingScript : MonoBehaviour {
    private Button button;
    [SerializeField] private float eatDuration = 3f;
    private bool isFeed;
    [SerializeField] private Image catfood;
    private AudioSource aud;

    private void Awake() {
        isFeed = false;
        button = GetComponent<Button>();
        aud = GetComponent<AudioSource>();
    }

    void Start() {
        button.onClick.AddListener(StartEat);
        catfood.enabled = false;
    }

    public async void StartEat() {
        bool canFeed = await CatfoodManager.instance.DecreaseCatfood(3);

        if (StatsManager.instance.happinessFull) {
            string msg = "Cat is full...";
            StartCoroutine(RoomSceneManager.instance.DisplayNotifs(msg));
        } else if (!canFeed) {
            string msg = "Catfood insufficient! Study to earn more.";
            StartCoroutine(RoomSceneManager.instance.DisplayNotifs(msg));
        } else if (!isFeed && canFeed) {
            string msg = "Cat eating!";
            StartCoroutine(RoomSceneManager.instance.DisplayNotifs(msg));

            catfood.enabled = true;
            Color col = catfood.color;
            catfood.color = new Color(col.r, col.g, col.b, 0);
            LeanTween.alpha(catfood.rectTransform, 1, 0.1f);

            RoomSceneManager.instance.ButtonPressBefore(CatState.EAT);
            Debug.Log("Start eating!");
            StartCoroutine(EatCoroutine());
        }
    }

    private IEnumerator EatCoroutine() {
        isFeed = true;
        aud.Play();
        yield return new WaitForSeconds(eatDuration);
        aud.Stop();
        LeanTween.alpha(catfood.rectTransform, 0, 0.1f);
        yield return new WaitForSeconds(0.1f);
        catfood.enabled = false;
        Debug.Log("Eating done!");
        CatMeow.instance.Meow();

        StatsManager.instance.ChangeHappy(5);
        RoomSceneManager.instance.ButtonPressAfter();
        isFeed = false;
    }
}
