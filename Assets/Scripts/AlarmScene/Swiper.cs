using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Swiper : MonoBehaviour, IDragHandler {

    private SizeFitter sizeFitter;

    private GameObject scroller;
    private ScrollRect scrollRect;
    [SerializeField] private GameObject above;
    [SerializeField] private GameObject below;

    [SerializeField] private float threshold = 10f;
    [SerializeField] private float moveForce = 0.1f;
    private bool isDeleting;

    private void Start() {
        isDeleting = false;
        scroller = GameObject.FindGameObjectWithTag("Scroller");
        sizeFitter = GetComponentInParent<SizeFitter>();

        scrollRect = scroller.GetComponent<ScrollRect>();
    }

    public void OnDrag(PointerEventData eventData) {

        // Passing control back to scroll
        eventData.pointerDrag = scroller;
        EventSystem.current.SetSelectedGameObject(scroller);

        scrollRect.OnInitializePotentialDrag(eventData);
        scrollRect.OnBeginDrag(eventData);

        float dif = eventData.pressPosition.x - eventData.position.x;
        Debug.Log(dif);
        if (dif > threshold && !isDeleting) { // If swiping left
            Debug.Log("swiping left");
            isDeleting = true;
            StartCoroutine(Move(above, new Vector3(-316, 0, 0)));
        } else if (isDeleting) { // If swiping right when button is exposed
            isDeleting = false;
            StartCoroutine(Move(above, Vector3.zero));
        }
    }

    private IEnumerator Move(GameObject obj, Vector3 newPos) {
        Vector3 currPos = obj.transform.localPosition;
        float i = 0;
        while (i < 1) {
            obj.transform.localPosition = Vector3.Lerp(currPos, newPos, i);
            i += moveForce;
            yield return null;
        }

        obj.transform.localPosition = newPos;
    }

    public void Delete() {
        AlarmManager.instance.DeleteAlarm(gameObject);
    }
}
