using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// Attached to Notebook gameobject
/// 
/// Study mechanics:
/// 
/// When study button pressed, brings users to techniques menu
/// Clicking on each technique brings user to unique interface (or same?) that displays respective info and timings. 
/// and a clock to visually display timing separation. and a Start button.
/// Some techniques require user input (can be postoned to later)
/// customize --> periodic alternating study time and break time? 
/// 
/// Once Start is pressed, brings user to another page, with cat studying at desk on a blank background, and clock running on screen.
/// phone access is restricted --> when user press home/back button, popup will show --> "are you sure you wna quit? catfood will be lost"
///
/// Whenever break, cat will start dancing? "Break" button appears for players to indicate when they are on break. then clock continues to run.
/// Then when its time to study again, cat will be idle? "Study" button appear for players to indicate when they start studying. then clock continues.
/// When study is over, cat dances and confetti begins and "Tap to claim" button pops up
/// 
/// Play animation of gained catfood and coins going into the total count --> like hayday 
/// 
/// </summary>
public class StudyScript : MonoBehaviour {
    public static StudyScript instance;
    public bool isStudying;
    [HideInInspector] public Animator animator;
    [HideInInspector] public Button button;

    [SerializeField] private GameObject menuScreen;
    [SerializeField] private GameObject closeButton;

    void Awake() {
        if (instance == null) {
            instance = this;
        }

        isStudying = false;
        animator = gameObject.GetComponent<Animator>();
        button = gameObject.GetComponent<Button>();

        button.onClick.AddListener(MenuAppear);
        closeButton.GetComponent<Button>().onClick.AddListener(Back);

        menuScreen.SetActive(false);
    }

    private void MenuAppear() {
        RoomSceneManager.instance.ButtonPressBefore(CatState.NONE);
        menuScreen.SetActive(true);
        menuScreen.GetComponent<CanvasGroup>().alpha = 0;
        LeanTween.alphaCanvas(menuScreen.GetComponent<CanvasGroup>(), 1, 0.1f);
        TechniqueManager.instance.Instantiate();
    }

    public void Back() {
        RoomSceneManager.instance.ButtonPressAfter();
        StartCoroutine(Fade());
    }

    IEnumerator Fade() {
        LeanTween.alphaCanvas(menuScreen.GetComponent<CanvasGroup>(), 0, 0.1f);
        yield return new WaitForSeconds(0.1f);
        menuScreen.SetActive(false);
    }
}
