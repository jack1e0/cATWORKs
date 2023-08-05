using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LightControl : MonoBehaviour
{
    private bool isSwitchedOn;
    [SerializeField] private GameObject on;
    [SerializeField] private GameObject off;

    private void Awake()
    {
        isSwitchedOn = false;
        on.SetActive(false);
        off.SetActive(true);
    }

    public void Switch()
    {
        Debug.Log("switch");
        if (isSwitchedOn)
        {
            // switch off
            on.SetActive(false);
            off.SetActive(true);
            isSwitchedOn = false;
        }
        else
        {
            on.SetActive(true);
            off.SetActive(false);
            isSwitchedOn = true;
        }
    }
}
