using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class unit1 : MonoBehaviour
{
    // class de guerrier 1 (unité de base)
    
    
    void Start()
    {
        // on ajoute l'unité a la liste des unité active au start
        unitSelectionManager.Instance.allUnitsList.Add(gameObject);
    }

    private void OnDestroy()
    {
        // on supprime l'unité de la liste des unité active lors de sa destruction
        unitSelectionManager.Instance.allUnitsList.Remove(gameObject);
    }
    
    void Update()
    {
        
    }
}
