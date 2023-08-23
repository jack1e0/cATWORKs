using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipeMiddleScript : MonoBehaviour
{
    public LogicScript logic;

    void Start()
    {
        logic = GameObject.FindGameObjectWithTag("Logic").GetComponent<LogicScript>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("added point");
        if (collision.gameObject.tag == "Bird")
        {
            logic.AddScore(1);
            Debug.Log("added point");
        }
    }
}
