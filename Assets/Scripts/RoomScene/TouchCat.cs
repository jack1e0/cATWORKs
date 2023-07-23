using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TouchCat : MonoBehaviour, IPointerDownHandler {

    public void OnPointerDown(PointerEventData eventData) {

        Ray ray = Camera.main.ScreenPointToRay(eventData.position);
        Debug.DrawRay(ray.origin, ray.direction * 200, Color.green, 5);
        int layerMask = LayerMask.GetMask("Cat");

        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, 200, layerMask);
        if (hit.collider != null && hit.collider.gameObject.tag == "Cat") {
            Debug.Log("hit cat");
            if (RoomSceneManager.instance.catControl.currentState == CatState.SIT) {
                if (Random.Range(0, 4) == 2) {
                    CatMeow.instance.Meow();
                    StatsManager.instance.ChangeHappy(1);
                }
            } else if (RoomSceneManager.instance.catControl.currentState == CatState.SLEEP) {
                CatMeow.instance.Snore();
            }
        }
    }

}
