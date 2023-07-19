using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DrawController : MonoBehaviour {
    int brushSize;
    [SerializeField] private GameObject canvas;
    private Texture2D tex;
    Color selectedColor;

    [SerializeField] private GameObject red;
    [SerializeField] private GameObject green;
    [SerializeField] private GameObject blue;
    [SerializeField] private GameObject yellow;
    [SerializeField] private GameObject black;
    [SerializeField] private GameObject white;

    [SerializeField] private GameObject small;
    [SerializeField] private GameObject mid;
    [SerializeField] private GameObject big;


    private float activeButtonSize = 1.2f;


    private void Awake() {
        // Clone texture
        Texture2D initTex = canvas.GetComponent<Renderer>().material.mainTexture as Texture2D;
        tex = new Texture2D(initTex.width, initTex.height, initTex.format, false);
        tex.SetPixels32(initTex.GetPixels32());
        tex.Apply();
        canvas.GetComponent<Renderer>().material.mainTexture = tex;

        red.GetComponent<Button>().onClick.AddListener(Red);
        green.GetComponent<Button>().onClick.AddListener(Green);
        blue.GetComponent<Button>().onClick.AddListener(Blue);
        yellow.GetComponent<Button>().onClick.AddListener(Yellow);
        black.GetComponent<Button>().onClick.AddListener(Black);
        white.GetComponent<Button>().onClick.AddListener(White);

        small.GetComponent<Button>().onClick.AddListener(Small);
        mid.GetComponent<Button>().onClick.AddListener(Mid);
        big.GetComponent<Button>().onClick.AddListener(Big);

        Red();
        Mid();
    }

    void Update() {
        if (Input.touchCount > 0) {
            Touch touch = Input.GetTouch(0);
            RaycastHit hit = new RaycastHit();
            Physics.Raycast(Camera.main.ScreenPointToRay(touch.position), out hit, 500);
            if (hit.collider == canvas.GetComponent<MeshCollider>()) {
                Vector2 textureCoord = hit.textureCoord;

                int pixelX = (int)(textureCoord.x * tex.width);
                int pixelY = (int)(textureCoord.y * tex.height);

                Vector2Int paintPixelPos = new Vector2Int(pixelX, pixelY);
                for (int i = pixelX - brushSize / 2; i < pixelX + brushSize / 2; i++) {
                    for (int j = pixelY - brushSize / 2; j < pixelY + brushSize / 2; j++) {
                        // if (Mathf.Pow(i - pixelX, 2) + Mathf.Pow(j - pixelY, 2) <= Mathf.Pow(brushSize / 2, 2)) {
                        //     tex.SetPixel(i, j, selectedColor);
                        // }
                        tex.SetPixel(i, j, selectedColor);
                    }
                }

                tex.Apply();
            }
        }
    }

    private void Red() {
        ResetButtonSize();
        SetButtonSize(red);
        selectedColor = red.GetComponent<Image>().color;
    }

    private void Green() {
        ResetButtonSize();
        SetButtonSize(green);
        selectedColor = green.GetComponent<Image>().color;
    }

    private void Blue() {
        ResetButtonSize();
        SetButtonSize(blue);
        selectedColor = blue.GetComponent<Image>().color;
    }

    private void Yellow() {
        ResetButtonSize();
        SetButtonSize(yellow);
        selectedColor = yellow.GetComponent<Image>().color;
    }

    private void Black() {
        ResetButtonSize();
        SetButtonSize(black);
        selectedColor = black.GetComponent<Image>().color;
    }

    private void White() {
        ResetButtonSize();
        SetButtonSize(white);
        selectedColor = white.GetComponent<Image>().color;
    }

    private void SetButtonSize(GameObject active) {
        active.GetComponent<RectTransform>().localScale = new Vector3(activeButtonSize, activeButtonSize, activeButtonSize);
    }

    private void ResetButtonSize() {
        red.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
        green.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
        blue.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
        yellow.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
        black.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
        white.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
    }

    private void Small() {
        ResetButtonColor();
        SetButtonColor(small);
        brushSize = 30;
    }

    private void Mid() {
        ResetButtonColor();
        SetButtonColor(mid);
        brushSize = 70;
    }

    private void Big() {
        ResetButtonColor();
        SetButtonColor(big);
        brushSize = 150;
    }

    private void SetButtonColor(GameObject active) {
        active.GetComponent<Image>().color = Color.gray;
    }

    private void ResetButtonColor() {
        small.GetComponent<Image>().color = Color.black;
        mid.GetComponent<Image>().color = Color.black;
        big.GetComponent<Image>().color = Color.black;
    }
}
