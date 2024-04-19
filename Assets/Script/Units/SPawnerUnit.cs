using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SPawnerUnit : MonoBehaviour
{
    [SerializeField]
    private GameObject spawnWorker;
    [SerializeField]
    private int NumberMax = 5;
    public Vector3 spawnPiont;
    public int MaxN = 20;
    public int timeTilNextSpawn = 5;
    int x = 0;

    float timer = 0;

    // Start is called before the first frame update
    void Start()
    {
        timer = 0;
        int i = 0;
        if (i < NumberMax)
        {
            StartCoroutine(Spawner(timeTilNextSpawn, spawnWorker, NumberMax));
            i++;
        }
        
    }


    // Update is called once per frame
  ///  void Update()
  ///  {
  /// <summary>
  ///  void Update()
  /// </summary>Time.deltaTime;
  ///      Spawn();
  ///  }

    private IEnumerator Spawner(float interval, GameObject unit, int numberUnit)
    {
        yield return new WaitForSeconds(interval);
        GameObject newUnit = Instantiate(unit, spawnPiont, Quaternion.identity);
        StartCoroutine(Spawner(interval, newUnit, numberUnit));
    }

  ///      void Spawn()
   /// {
    ///    if (timer >= timeTilNextSpawn)
   ///     {
   ///         x = Random.Range(0,MaxN);
   /// <summary>
  ///      void Spawn()
  /// </summary>
  /// <param name=""></param>
  /// <param name=""></param>
  /// <param name=""></param>t.x = x;
  ////          Instantiate(spawnWorker,spawnPiont,Quaternion.identity);
  ///          timer = 0;
   ////     }
  ///  }
}
