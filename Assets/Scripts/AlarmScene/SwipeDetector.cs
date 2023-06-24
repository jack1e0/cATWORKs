using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class SwipeDetector : MonoBehaviour {

    public static SwipeDetector instance;
    private delegate void Swipe(int direction);
    private event Swipe swipePerformed;


    [SerializeField] private InputAction pos, press;
    [SerializeField] private float swipeThreshold = 30f;
    private bool isCancelling;

    private Vector3 initPos;
    private Vector3 currPos => pos.ReadValue<Vector2>();


    private void Awake() {
        if (instance == null) {
            instance = this;
        }

        isCancelling = false;
        pos.Enable();
        press.Enable();

        press.performed += _ => { initPos = currPos; };
        press.canceled += _ => DetectSwipe();
    }

    private void DetectSwipe() {
        Vector2 delta = currPos - initPos;
        int direction = 0; // stores whether swiped left or right

        if (!isCancelling) { // when alarm is in normal state
            if (delta.x < -1 * swipeThreshold) { // Only able to swipe left to delete 
                direction = -1;
            }
        } else { // when already swipe left -> delete button is showing
            if (delta.x > swipeThreshold) {
                direction = 1;
            }
        }

        if (direction != 0 && swipePerformed != null) {
            swipePerformed(direction);
        }
    }
}