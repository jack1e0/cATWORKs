using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SizeFitter : MonoBehaviour {
    public static SizeFitter instance;

    [SerializeField] private GameObject container;
    private RectTransform containerRect;

    [SerializeField] private GameObject scroller;
    private ScrollRect scrollrect;

    [SerializeField] private GameObject placeholder;
    private RectTransform placeholderRect;

    private RectTransform rect;

    private void Awake() {
        if (instance == null) {
            instance = this;
        }

        rect = GetComponent<RectTransform>();
        containerRect = container.GetComponent<RectTransform>();
        scrollrect = scroller.GetComponent<ScrollRect>();
        placeholderRect = placeholder.GetComponent<RectTransform>();
    }

    public IEnumerator Expand(GameObject unit) {
        float height = unit.GetComponent<RectTransform>().rect.height;

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
