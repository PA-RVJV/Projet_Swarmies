using System;
using System.Collections;
using System.Collections.Generic;
using PS.Units;
using PS.Units.Player;
using PS.Units.Enemy;
using UnityEngine;
using UnityEngine.UI;

public class SpawnerUnit : MonoBehaviour
{
    [SerializeField]
    private GameObject spawnWorker;
    [SerializeField]
    private int numberMax = 5;

    [SerializeField]
    private UnitStatDisplay caserneDisplay;
    
    public Vector3 spawnPoint;
    public float timeTilNextSpawn = 5f;
    public float enemyDetectionRadius = 3f;
    
    public string unitToSpawn;
    public UnitConfigManager unitConfigManager;
    public Transform myparent;
    
    private int currentCount = 0;
    private bool _running = true;
    
    private float spawnProgress = 0f;

    private ResourceManager resourceManager;
    
    private int playerLayer;
    private int enemyLayer;

    void Start()
    {
        _running = true;
        var t = transform.Find("Spawn");
        if (t)
        {
            spawnPoint = t.position;
        }
        
        resourceManager = FindObjectOfType<ResourceManager>(); 
        playerLayer = LayerMask.NameToLayer("PlayerUnits");
        enemyLayer = LayerMask.NameToLayer("EnemyUnits");
        StartCoroutine(Spawner());
    }

    private IEnumerator Spawner()
    {
        while (_running) // Change the condition to always run the coroutine
        {
            if (unitToSpawn == "")
            {
                caserneDisplay.ShowProductionDisplay(false);
                yield return new WaitForSeconds(timeTilNextSpawn);
                continue;
            }
            
            if (currentCount != 0)
            {
                caserneDisplay.UpdateUnitsInQueueText(currentCount, numberMax);
            }
            
            // Vérifiez si des unités ennemies sont à proximité
            if (IsEnemyNearby())
            {
                caserneDisplay.ShowProductionDisplay(false);
                yield return new WaitForSeconds(1f); // Attendez un peu avant de vérifier à nouveau
                continue;
            }
            
            if (unitToSpawn == "Worker" && currentCount < numberMax)
            {
                //yield return new WaitForSeconds(timeTilNextSpawn);
                // spawn worker sans le display de production
                caserneDisplay.ShowProductionDisplay(true);
                caserneDisplay.ChangeUnitIcon(unitToSpawn);
                caserneDisplay.UpdateUnitsInQueueText(currentCount, numberMax);
                
                spawnProgress = 0f;

                while (spawnProgress < 1f)
                {
                    spawnProgress += Time.deltaTime / timeTilNextSpawn;
                    caserneDisplay.UpdateProgressBar(spawnProgress);
                    yield return null;
                }
                
                SpawnUnit();
            }
            else if (currentCount < numberMax)
            {
                caserneDisplay.ShowProductionDisplay(true);
                caserneDisplay.ChangeUnitIcon(unitToSpawn);

                if (gameObject.layer == playerLayer)
                {
                    // vérifie si le jouer a assez de resource, si c'est le cas, les resource sont déduit du stock et on return true
                    // sinon un message est afficher pendant quelque seconde et on return false
                    if (!resourceManager.HasEnoughResourcesForUnit(unitToSpawn))
                    {
                        caserneDisplay.ShowResourceWarning(true);
                        yield return new WaitForSeconds(timeTilNextSpawn);
                        continue;
                    }
                    else
                    {
                        caserneDisplay.ShowResourceWarning(false);
                    }
                }
                
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
        GO.name = unitToSpawn;

        if (gameObject.layer == playerLayer)
        {
            PlayerUnit pu = GO.GetComponent<PlayerUnit>();
            pu.unitConfig = unitConfigManager;
                
            // place l'unité dans la bonne catégorie (un objet de coordonnée 0,0,0 de pref)
            if(myparent)
                GO.transform.parent = myparent;
            
            PlayerUnit pus = GetComponent<PlayerUnit>();
            if (pus)
            {
                var GOpu = GO.GetComponent<PlayerUnit>();
                GOpu.unitConfig = pus.unitConfig;
                GOpu.unitHandler = pus.unitHandler;
                GOpu.baseStats = GOpu.unitHandler.GetUnitStats(unitToSpawn.ToLower());
            }
        }
        else if (gameObject.layer == enemyLayer)
        {
            EnemyUnit eu = GO.GetComponent<EnemyUnit>();
            eu.unitConfig = unitConfigManager;

            if (myparent)
                GO.transform.parent = myparent;

            EnemyUnit eus = GetComponent<EnemyUnit>();
            if (eus)
            {
                var GOeu = GO.GetComponent<EnemyUnit>();
                GOeu.unitConfig = eus.unitConfig;
                GOeu.unitHandler = eus.unitHandler;
                GOeu.baseStats = GOeu.unitHandler.GetUnitStats(unitToSpawn.ToLower());
            }
        }
        
                
        GO.GetComponent<UnitSpawnCount>().SetSpawner(this); // Set the spawner reference
        currentCount++;
        Debug.Log("Spawned unit. Current count: " + currentCount);
    }
    
    private bool IsEnemyNearby()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, enemyDetectionRadius);
        foreach (var hitCollider in hitColliders)
        {
            if (gameObject.layer == playerLayer && hitCollider.gameObject.layer == enemyLayer)
            {
                return true;
            }
            else if (gameObject.layer == enemyLayer && hitCollider.gameObject.layer == playerLayer)
            {
                return true;
            }
        }
        return false;
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
