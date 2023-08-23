using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipeSpawnScript : MonoBehaviour
{
    public BirdScript bs;
    public GameObject pipe;
    public float spawnRate = 1.5f;
    private float timer = 0;
    public float heightOffset = 10;


    void Update()
    {
        if (bs.startGame)
        {
            if (timer < spawnRate)
            {
                timer += Time.deltaTime;
            }
            else
            {
                SpawnPipe();
                timer = 0;
            }
        }
    }

    void SpawnPipe()
    {
        float max = transform.position.y + heightOffset;
        float min = transform.position.y - heightOffset;

        Instantiate(pipe, new Vector3(transform.position.x, Random.Range(min, max), 0), transform.rotation);
    }
}
