using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdScript : MonoBehaviour
{
    public Rigidbody2D myRigidbody;
    public float flapStrength;
    public LogicScript logic;
    private bool birdIsAlive = true;
    public bool startGame;

    private void Start()
    {
        startGame = false;
        myRigidbody.gravityScale = 0;
    }

    void Update()
    {
        if (!startGame && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        // for testing: if (!startGame && Input.GetMouseButtonDown(0))
        {
            startGame = true;
            myRigidbody.gravityScale = 6;
        }
        if (startGame && birdIsAlive && Input.touchCount > 0)
        {
            if (Input.GetTouch(0).phase == TouchPhase.Began)
            {
                myRigidbody.velocity = Vector2.up * flapStrength;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        Debug.Log("crash!");
        logic.GameOver();
        birdIsAlive = false;
    }
}
