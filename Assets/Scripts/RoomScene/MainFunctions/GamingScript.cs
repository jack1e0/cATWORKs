using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GamingScript : MonoBehaviour
{
    private Button button;
    [SerializeField]
    private GameObject gameMenu;

    [SerializeField] private Button drawing;
    [SerializeField] private Button flappy;

    private void Awake()
    {
        gameMenu.SetActive(false);
        button = gameObject.GetComponent<Button>();
        button.onClick.AddListener(GameMenu);
        drawing.onClick.AddListener(Draw);
        flappy.onClick.AddListener(Flap);
    }

    private void GameMenu()
    {
        RoomSceneManager.instance.ButtonPressBefore(CatState.NONE);
        gameMenu.SetActive(true);
        //SceneTransition.instance.ChangeScene("DrawingGame");
    }

    private void Draw()
    {
        SceneTransition.instance.ChangeScene("DrawingGame");
    }

    private void Flap()
    {
        SceneTransition.instance.ChangeScene("FlappycatScene");
    }
}
