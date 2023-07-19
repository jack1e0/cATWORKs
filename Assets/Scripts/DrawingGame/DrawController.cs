using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.IO;

public class DrawController : MonoBehaviour {
    float brushSize;
    Color selectedColor;

    int sorting;
    private Stack<GameObject> drawnLines;
    private GameObject lastDrawn;

    [SerializeField] private GameObject linePrefab;
    LineRender activeLine;

    [SerializeField] private GameObject red;
    [SerializeField] private GameObject green;
    [SerializeField] private GameObject blue;
    [SerializeField] private GameObject yellow;
    [SerializeField] private GameObject black;
    [SerializeField] private GameObject white;

    [SerializeField] private GameObject small;
    [SerializeField] private GameObject mid;
    [SerializeField] private GameObject big;

    [SerializeField] private Button undo;
    [SerializeField] private Button save;
    [SerializeField] private Button cross;

    private float activeButtonSize = 1.2f;

    [SerializeField] private RenderTexture rt;
    private int fileCounter;


    private void Awake() {
        sorting = 0;
        drawnLines = new Stack<GameObject>();
        red.GetComponent<Button>().onClick.AddListener(Red);
        green.GetComponent<Button>().onClick.AddListener(Green);
        blue.GetComponent<Button>().onClick.AddListener(Blue);
        yellow.GetComponent<Button>().onClick.AddListener(Yellow);
        black.GetComponent<Button>().onClick.AddListener(Black);
        white.GetComponent<Button>().onClick.AddListener(White);

        small.GetComponent<Button>().onClick.AddListener(Small);
        mid.GetComponent<Button>().onClick.AddListener(Mid);
        big.GetComponent<Button>().onClick.AddListener(Big);

        undo.onClick.AddListener(Undo);
        save.onClick.AddListener(Save);
        cross.onClick.AddListener(Cross);

        Red();
        Mid();
    }

    void Update() {

        if (Input.touchCount > 0) {
            Touch touch = Input.GetTouch(0);
            Debug.Log(touch.position);
            if (touch.phase == TouchPhase.Began && WithinBounds(touch.position)) {
                Debug.Log("within bounds");
                lastDrawn = Instantiate(linePrefab);
                activeLine = lastDrawn.GetComponent<LineRender>();
                lastDrawn.GetComponent<LineRenderer>().material.SetColor("_Color", selectedColor);
                lastDrawn.GetComponent<LineRenderer>().startWidth = brushSize;
                lastDrawn.GetComponent<LineRenderer>().sortingOrder = sorting;
                sorting++;
            }
            if (activeLine != null) {
                Vector2 worldPos = Camera.main.ScreenPointToRay(touch.position).GetPoint(40);
                activeLine.UpdatePoint(worldPos);
            }
            if (touch.phase == TouchPhase.Ended && activeLine != null) {
                activeLine = null;
                drawnLines.Push(lastDrawn);
            }
        }
    }

    private bool WithinBounds(Vector2 pos) {
        return pos.x > 41.3f && pos.x < 1038 && pos.y > 408 && pos.y < 2050;
    }

    private void Undo() {
        if (drawnLines.Count > 0) {
            Destroy(drawnLines.Pop());
        }
    }

    private void Save() {
        Capture();
    }

    public void Capture() {
        RenderTexture activeRenderTexture = RenderTexture.active;
        Camera.main.targetTexture = rt;
        RenderTexture.active = Camera.main.targetTexture;

        Camera.main.Render();

        Texture2D image = new Texture2D(Camera.main.targetTexture.width, Camera.main.targetTexture.height);
        image.ReadPixels(new Rect(0, 0, Camera.main.targetTexture.width, Camera.main.targetTexture.height), 0, 0);
        image.Apply();
        RenderTexture.active = activeRenderTexture;
        Camera.main.targetTexture = null;

        byte[] bytes = image.EncodeToPNG();
        Destroy(image);

        // string path = Application.dataPath + "/Drawings/" + fileCounter + ".png";
        // Debug.Log(path);
        // File.WriteAllBytes(Application.dataPath + "/Drawings/" + fileCounter + ".png", bytes);
        // fileCounter++;

        // Save the screenshot to Gallery/Photos
        string name = string.Format("{0}_Capture{1}_{2}.png", Application.productName, "{0}", System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"));
        Debug.Log("Permission result: " + NativeGallery.SaveImageToGallery(bytes, Application.productName + " Captures", name));
    }

    private void Cross() {
        SceneTransition.instance.ChangeScene("RoomScene");
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
        brushSize = 0.1f;
    }

    private void Mid() {
        ResetButtonColor();
        SetButtonColor(mid);
        brushSize = 0.35f;
    }

    private void Big() {
        ResetButtonColor();
        SetButtonColor(big);
        brushSize = 1.25f;
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
