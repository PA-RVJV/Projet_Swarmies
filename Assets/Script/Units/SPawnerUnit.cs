using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SpawnerUnit : MonoBehaviour
{
    [SerializeField]
    private GameObject spawnWorker;
    [SerializeField]
    private int numberMax = 5;
    public Vector3 spawnPoint;
    public float timeTilNextSpawn = 5f;

    private int currentCount = 0;

    // Start is called before the first frame update

    void Start()
    {
        StartCoroutine(Spawner());
    }

    private IEnumerator Spawner()
    {
        while (currentCount < numberMax)
        {
            yield return new WaitForSeconds(timeTilNextSpawn);
            Instantiate(spawnWorker, spawnPoint, Quaternion.identity);
            currentCount++;
        }
    }
}
