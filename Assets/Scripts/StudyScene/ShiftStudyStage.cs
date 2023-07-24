using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShiftStudyStage : MonoBehaviour {
    [SerializeField] public GameObject indicator;

    public void ShiftIndicator() {
        Vector3 pos = indicator.transform.localPosition;
        if (pos.x <= 414f) {
            indicator.transform.localPosition = new Vector3(pos.x + 207f, pos.y, pos.z);
        }
    }
}
