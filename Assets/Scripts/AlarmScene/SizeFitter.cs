using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SizeFitter : MonoBehaviour {
    public static SizeFitter instance;

    private GameObject container;
    private RectTransform containerRect;

    private GameObject scroller;
    private ScrollRect scrollrect;

    private GameObject placeholder;
    private RectTransform placeholderRect;

    private RectTransform rect;

    private void Awake() {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(this);
        } else {
            Destroy(this);
            instance = GameObject.FindGameObjectWithTag("Parent").GetComponent<SizeFitter>();
        }

        instance.Instantiate();
    }

    public void Instantiate() {
        rect = GetComponent<RectTransform>();
        container = GameObject.FindGameObjectWithTag("Container");
        containerRect = container.GetComponent<RectTransform>();
        scroller = GameObject.FindGameObjectWithTag("Scroller");
        scrollrect = scroller.GetComponent<ScrollRect>();
        placeholder = GameObject.FindGameObjectWithTag("Placeholder");
        placeholderRect = placeholder.GetComponent<RectTransform>();
    }

    public IEnumerator Expand(float height) {
        if (placeholderRect.sizeDelta.y > height) {
            placeholderRect.sizeDelta = new Vector2(placeholderRect.sizeDelta.x, placeholderRect.sizeDelta.y - height);
        }

        yield return null; // Wait for next frame for contentsizefitter to accurately calculate changes
        LayoutRebuilder.MarkLayoutForRebuild(containerRect);
    }

    public IEnumerator Contract(float height) {
        Debug.Log("placeholdersize: " + placeholderRect.sizeDelta.y);
        if (containerRect.rect.height - height < scroller.GetComponent<RectTransform>().rect.height) {
            placeholderRect.sizeDelta = new Vector2(placeholderRect.sizeDelta.x, placeholderRect.sizeDelta.y + height);
        }

        yield return null;
        LayoutRebuilder.MarkLayoutForRebuild(containerRect);
    }
}
