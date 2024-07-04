using System.Collections;
using PS.Units;
using PS.Units.Player;
using UnityEngine;

public class SpawnerUnit : MonoBehaviour
{
    [SerializeField]
    private GameObject spawnWorker;
    [SerializeField]
    private int numberMax = 5;
    public Vector3 spawnPoint;
    public float timeTilNextSpawn = 5f;

    private int currentCount = 0;
    private string _unitToSpawn;

    // Start is called before the first frame update

    void Start()
    {
        var t = transform.Find("Spawn");
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
                
                //GO.name = gameObject.name.Remove(gameObject.name.Length - 1);
                GO.name = _unitToSpawn;
                
                // place l'unité dans la bonne catégorie (un objet de coordonnée 0,0,0 de pref)
                GO.transform.parent = transform;
                
                
                PlayerUnit pus = GetComponent<PlayerUnit>();
                //Transform ucf = transform.Find("UnitConfigManager");
                if (pus)
                {
                    var GOpu = GO.GetComponent<PlayerUnit>();
                    GOpu.unitConfig = pus.unitConfig;
                    GOpu.unitHandler = pus.unitHandler;
                }
                
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

    public void SetUnitToSpawn(string unitName)
    {
        _unitToSpawn = unitName;
    }
}
