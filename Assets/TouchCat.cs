using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TouchCat : MonoBehaviour, IPointerDownHandler {

    public void OnPointerDown(PointerEventData eventData) {
        Debug.Log("PRESSED");
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(eventData.position);
        if (Physics.Raycast(ray, out hit)) {
            Debug.DrawRay(ray.origin, ray.direction, Color.green, 5);
            if (hit.transform.gameObject.tag == "Cat") {
                Debug.Log("hit cat");
                if (CatBehaviourManager.instance.currentState == CatState.SIT) {
                    CatMeow.instance.Meow();
                }
            }
        }
    }

}
