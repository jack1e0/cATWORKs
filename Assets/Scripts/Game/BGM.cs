using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGM : MonoBehaviour {
    public static BGM instance;
    public AudioSource audSource;
    public bool isPlaying;
    void Awake() {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(this);
        } else {
            Destroy(gameObject);
            instance = GameObject.FindGameObjectWithTag("BGM").GetComponent<BGM>();
        }
        instance.isPlaying = true;
        instance.audSource = instance.gameObject.GetComponent<AudioSource>();
    }
}
