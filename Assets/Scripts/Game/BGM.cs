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

        instance.audSource = instance.gameObject.GetComponent<AudioSource>();
    }

    public void ChangeClip(AudioClip clip) {
        this.audSource.clip = clip;
        this.audSource.volume = 0.5f;
        if (!this.isPlaying) {
            this.isPlaying = true;
            this.audSource.Play();
        }
    }

    public void StopBGM() {
        this.isPlaying = false;
        this.audSource.Stop();
    }
}
