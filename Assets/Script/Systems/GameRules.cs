using System;
using System.Data;
using UnityEngine;
using Script;
#nullable enable

namespace Script.Systems
{
    public class GameRules : MonoBehaviour
    {
        public GameObject? casernePrefab;
        public GameObject? terrain;
        

        public void DealWithAction(UnitActionsEnum action, GameObject?[] source)
        {
            if (!terrain || !casernePrefab)
                throw new ConstraintException();

            switch (action)
            {
                case UnitActionsEnum.Construire:
                    foreach (var unit in source)
                    {
                        if(!unit)
                            continue;
                        var go = Instantiate(casernePrefab, unit?.transform);
                        go.transform.parent = terrain.transform;
                        Destroy(unit);
                    }
                    break;
                default:
                    throw new NotImplementedException(nameof(action));
            }
            

        }
    }
}


