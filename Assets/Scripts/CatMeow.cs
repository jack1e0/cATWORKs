using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatMeow : MonoBehaviour {
    public static CatMeow instance;
    private AudioSource audioSource;
    [SerializeField] private float volume;

    private void Awake() {
        if (instance == null) {
            instance = this;
        }
        audioSource = GetComponent<AudioSource>();
    }

    public void Meow() {
        audioSource.volume = this.volume;
        audioSource.Play();
    }
}
