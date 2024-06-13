using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSpawnCount : MonoBehaviour
{
    private SpawnerUnit spawner;

    public void SetSpawner(SpawnerUnit spawner)
    {
        this.spawner = spawner;
    }

    void OnDestroy()
    {
        if (spawner != null)
        {
            spawner.DecrementCount();
        }
    }
}
