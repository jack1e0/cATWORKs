using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

[CreateAssetMenu]
public class TechniqueDetails : ScriptableObject {
    public string techniqueName;
    public GameObject circularBar;
    public GameObject timeLine;

    public float[] studyStages;
}
