using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class unit1 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        unitSelectionManager.Instance.allUnitsList.Add(gameObject);
    }

    private void OnDestroy()
    {
        unitSelectionManager.Instance.allUnitsList.Remove(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
