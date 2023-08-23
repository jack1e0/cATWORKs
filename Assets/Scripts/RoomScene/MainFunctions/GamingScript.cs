using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GamingScript : MonoBehaviour
{
    private Button button;
    [SerializeField] private GameObject gameMenu;
    [SerializeField] private Button cross;
    [SerializeField] private Button drawing;
    [SerializeField] private Button flappy;

    private void Awake()
    {
        gameMenu.SetActive(false);
        button = gameObject.GetComponent<Button>();
        button.onClick.AddListener(GameMenu);
        drawing.onClick.AddListener(Draw);
        flappy.onClick.AddListener(Flap);
        cross.onClick.AddListener(Cross);
    }

    private void GameMenu()
    {
        RoomSceneManager.instance.ButtonPressBefore(CatState.NONE);
        gameMenu.SetActive(true);
        gameMenu.GetComponent<CanvasGroup>().alpha = 0;
        LeanTween.alphaCanvas(gameMenu.GetComponent<CanvasGroup>(), 1, 0.1f);
    }

    private void Draw()
    {
        SceneTransition.instance.ChangeScene("DrawingGame");
    }

    private void Flap()
    {
        SceneTransition.instance.ChangeScene("FlappycatScene");
    }

    private void Cross()
    {
        RoomSceneManager.instance.ButtonPressAfter();
        StartCoroutine(Fade());
    }

    IEnumerator Fade()
    {
        LeanTween.alphaCanvas(gameMenu.GetComponent<CanvasGroup>(), 0, 0.1f);
        yield return new WaitForSeconds(0.1f);
        gameMenu.SetActive(false);
    }
}

