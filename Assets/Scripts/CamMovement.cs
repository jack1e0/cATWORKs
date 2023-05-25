using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamMovement : MonoBehaviour {
    [SerializeField] private Camera camera;
    private Vector3 curr;
    private Vector3 mousePos;
    private float difference;
    private bool isDrag;
    private bool atLeft;
    private bool atRight;

    private void LateUpdate() {
        curr = camera.transform.position;

        if (Input.GetMouseButton(0)) {
            difference = camera.ScreenToWorldPoint(Input.mousePosition).x - camera.transform.position.x;
            //if (curr.x - difference)
            if (!isDrag) {
                isDrag = true;
                mousePos = camera.ScreenToWorldPoint(Input.mousePosition);
            }
        } else {
            isDrag = false;
        }

        if (isDrag) {
            if ((mousePos.x - difference) >= -1.5f && (mousePos.x - difference) <= 1.5f) {
                camera.transform.position = new Vector3(mousePos.x - difference, curr.y, curr.z);
            }

        }
    }
}
