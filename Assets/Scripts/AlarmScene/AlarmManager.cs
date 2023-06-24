using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlarmManager : MonoBehaviour {
    // TODO: implement alarm setter
    // [SerializeField] private GameObject alarmSetterPage;

    [SerializeField] private GameObject alarmUnit;
    [SerializeField] private GameObject parent;


    public void AddAlarm() {
        GameObject newUnit = Instantiate(alarmUnit, parent.transform);
        StartCoroutine(SizeFitter.instance.Expand(newUnit));
    }
}
