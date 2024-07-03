using System.Collections;
using System.Collections.Generic;
using PS.Units;
using PS.Units.Player;
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
    public UnitConfigManager unitConfigManager;

    // Start is called before the first frame update

    void Start()
    {
        var t = transform.Find("Spawner");
        if (t)
        {
            spawnPoint = t.position;
        }
        StartCoroutine(Spawner());
    }

    private IEnumerator Spawner()
    {
        while (true) // Change the condition to always run the coroutine
        {
            if (currentCount < numberMax)
            {
                yield return new WaitForSeconds(timeTilNextSpawn);
                GameObject GO = Instantiate(spawnWorker, spawnPoint, Quaternion.identity);
                PlayerUnit pu = GO.GetComponent<PlayerUnit>();
                pu.unitConfig = unitConfigManager;
                
                GO.name = gameObject.name.Remove(gameObject.name.Length - 1);
                GO.transform.parent = transform;
                GO.GetComponent<UnitSpawnCount>().SetSpawner(this); // Set the spawner reference
                currentCount++;
                Debug.Log("Spawned unit. Current count: " + currentCount);
            }
            else
            {
                yield return null;
            }
        }
    }

    // Method to decrement the unit count
    public void DecrementCount()
    {
        currentCount--;
        Debug.Log("Unit destroyed. Current count: " + currentCount);
    }
}
