using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SPawnerUnit : MonoBehaviour
{

    public GameObject spawnMairie;
    public Vectors3 spawnPiont;
    public int MaxN = 20;
    public int timeTilNextSpawn = 5;
    int x = 0;

    float timer = 0;

    // Start is called before the first frame update
    void Start()
    {
        timer = 0;
        spawnPiont.x = x;
    }


    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        Spawn();
    }

        void Spawn()
    {
        if (timer >= timeTilNextSpawn)
        {
            x = Random.Range(0,MaxN);
            spawnPiont.x = x;
            Instantiate(spawnMairie,spawnPiont,Quaternion.identity);
            timer = 0;
        }
    }
}
