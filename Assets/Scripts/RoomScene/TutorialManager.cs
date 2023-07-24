using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Newtonsoft.Json;
using Firebase.Database;
using System.Threading.Tasks;

public class TutorialManager : MonoBehaviour {
    [SerializeField] private Image screen;
    [SerializeField] private TMP_Text content;
    [SerializeField] private GameObject cat;
    private List<string> texts;
    private bool isTextShowing;
    private int index;

    private void Awake() {
        if (SceneTransition.instance.user.firstTime) {
            SceneTransition.instance.firstEnteredRoom = false;
            texts = new List<string> {
                "Hi " + SceneTransition.instance.user.username + ", I'm your new study buddy!",
                "I will earn you rewards while you work, so do take care of me :)",
                "Other than studying, I do love painting as well, perhaps we could paint together?",
                "And I can't wait to unlock cool stuff when we level up!"
            };
            screen.GetComponent<RectTransform>().localScale = new Vector3(0, 1, 1);
            cat.SetActive(false);
            StartDisplay();
        } else {
            gameObject.SetActive(false);
        }
    }

    void StartDisplay() {
        content.text = "";
        isTextShowing = true;
        index = 0;
        StartCoroutine(Display());
    }

    void Update() {
        if (isTextShowing) {
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) {
                if (content.text == texts[index]) {
                    NextLine();
                } else {
                    StopAllCoroutines();
                    content.text = texts[index];
                }
            }
        }
    }

    private IEnumerator Display() {
        screen.GetComponent<RectTransform>().LeanScaleX(1, 0.7f);
        yield return new WaitForSeconds(0.7f);
        cat.SetActive(true);
        StartCoroutine(DisplayLine());
    }

    private IEnumerator DisplayLine() {
        //one char at a time
        foreach (char c in texts[index].ToCharArray()) {
            content.text += c;
            yield return new WaitForSeconds(0.02f);
        }
    }

    void NextLine() {
        if (index < texts.Count - 1) {
            index++;
            content.text = "";
            StartCoroutine(DisplayLine());
        } else {
            isTextShowing = false;
            CamMovement.instance.control = true;

            RoomSceneManager.instance.InstantiateCats();
            SceneTransition.instance.user.firstTime = false;
            StartCoroutine(await());
        }
    }

    private async Task UpdateFirstTime() {
        string first = JsonConvert.SerializeObject(SceneTransition.instance.user.firstTime);

        DatabaseReference DBreference = FirebaseDatabase.DefaultInstance.RootReference;
        await DBreference.Child("users").Child(SceneTransition.instance.user.userId).Child("firstTime").SetValueAsync(first);
    }

    IEnumerator await() {
        Task t = UpdateFirstTime();
        yield return new WaitUntil(() => t.IsCompleted);
        gameObject.SetActive(false);
    }
}
