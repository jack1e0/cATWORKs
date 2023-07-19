using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GamingScript : MonoBehaviour {
    private Button button;

    private void Awake() {
        button = gameObject.GetComponent<Button>();
        button.onClick.AddListener(GameScene);
    }

    public void GameScene() {
        CatBehaviourManager.instance.ButtonPressBefore(CatState.NONE);
        SceneTransition.instance.ChangeScene("DrawingGame");
    }
}
