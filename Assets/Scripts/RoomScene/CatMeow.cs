using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatMeow : MonoBehaviour {
    public static CatMeow instance;
    private AudioSource audioSource;
    [SerializeField] private float volume;

    [SerializeField] private AudioClip[] clips;

    private void Awake() {
        if (instance == null) {
            instance = this;
        }
        audioSource = GetComponent<AudioSource>();
    }

    public void Meow() {
        audioSource.volume = this.volume;
        audioSource.clip = clips[0];
        audioSource.Play();
    }

    public void Snore() {
        audioSource.volume = 1;
        audioSource.clip = clips[1];
        audioSource.Play();
    }
}
