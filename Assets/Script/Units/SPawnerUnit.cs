using System;
using System.Collections;
using System.Collections.Generic;
using PS.Units;
using PS.Units.Player;
using UnityEngine;
using UnityEngine.UI;

public class SpawnerUnit : MonoBehaviour
{
    [SerializeField]
    private GameObject spawnWorker;
    [SerializeField]
    private int numberMax = 5;
    public Vector3 spawnPoint;
    public float timeTilNextSpawn = 5f;

    public string unitToSpawn;
    public UnitConfigManager unitConfigManager;
    public Transform myparent;
    
    private int currentCount = 0;
    private int unitToProduct = 0;
    private bool _running = true;

    // UI Elements
    public CaserneDisplay caserneDisplay;
    private int unitsInQueue = 0;
    private float spawnProgress = 0f;

    
    
    // Start is called before the first frame update

    void Start()
    {
        _running = true;
        var t = transform.Find("Spawn");
        if (t)
        {
            spawnPoint = t.position;
        }
        StartCoroutine(Spawner());
    }

    private IEnumerator Spawner()
    {
        while (_running) // Change the condition to always run the coroutine
        {
            if (currentCount != 0 && unitToSpawn != "Worker")
            {
                caserneDisplay.UpdateUnitsInQueueText(currentCount, numberMax);
            }
            
            if (unitToSpawn == "")
            {
                yield return new WaitForSeconds(timeTilNextSpawn);
                continue;
            }
            
            if (unitToSpawn == "Worker" && currentCount < numberMax)
            {
                yield return new WaitForSeconds(timeTilNextSpawn);
                // spawn worker sans le display de production
                SpawnUnit();
                
            }
            else if (currentCount < numberMax)
            {
                unitsInQueue++;
                caserneDisplay.ChangeUnitIcon(unitToSpawn);
                caserneDisplay.UpdateUnitsInQueueText(currentCount, numberMax);
                caserneDisplay.ShowResourceWarning(false);
                spawnProgress = 0f;

                while (spawnProgress < 1f)
                {
                    spawnProgress += Time.deltaTime / timeTilNextSpawn;
                    caserneDisplay.UpdateProgressBar(spawnProgress);
                    yield return null;
                }

                SpawnUnit();
                
                unitsInQueue--;
            }
            else
            {
                yield return null;
            }
        }
    }
    
    private void SpawnUnit()
    {
        GameObject GO = Instantiate(spawnWorker, spawnPoint, Quaternion.identity);
        PlayerUnit pu = GO.GetComponent<PlayerUnit>();
        pu.unitConfig = unitConfigManager;
                
        //GO.name = gameObject.name.Remove(gameObject.name.Length - 1);
        GO.name = unitToSpawn;
                
        // place l'unité dans la bonne catégorie (un objet de coordonnée 0,0,0 de pref)
        if(myparent)
            GO.transform.parent = myparent;
        else
            GO.transform.parent = transform;
                
                
        PlayerUnit pus = GetComponent<PlayerUnit>();
        //Transform ucf = transform.Find("UnitConfigManager");
        if (pus)
        {
            var GOpu = GO.GetComponent<PlayerUnit>();
            GOpu.unitConfig = pus.unitConfig;
            GOpu.unitHandler = pus.unitHandler;
            GOpu.baseStats = GOpu.unitHandler.GetUnitStats(unitToSpawn.ToLower());
        }
                
        GO.GetComponent<UnitSpawnCount>().SetSpawner(this); // Set the spawner reference
        currentCount++;
        Debug.Log("Spawned unit. Current count: " + currentCount);
    }
    
    // Method to decrement the unit count
    public void DecrementCount()
    {
        currentCount--;
        Debug.Log("Unit destroyed. Current count: " + currentCount);
    }
    
    private void OnDestroy()
    {
        _running = false;
    }
    
    
}
