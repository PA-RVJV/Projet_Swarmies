using System;
using System.Data;
using UnityEngine;
using Script;
using UnityEngine.AI;

#nullable enable

namespace Script.Systems
{
    public class GameRules : MonoBehaviour
    {
        public GameObject? casernePrefab;
        public GameObject selectCirclePrefab;
        public GameObject? terrain;
        public GameObject casernesAlliees;

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
                        var go = Instantiate(casernePrefab, unit.transform.position, unit.transform.rotation);
                        go.transform.parent = casernesAlliees.transform;
                        var nvo = go.AddComponent<NavMeshObstacle>();
                        nvo.carving = true;

                        var selectCircle = Instantiate(selectCirclePrefab, go.transform);
                        selectCircle.name = "Hightlight";
                        selectCircle.transform.parent = go.transform;
                        var scpos = selectCircle.transform.position;
                        scpos.y = 0;
                        selectCircle.transform.position = scpos;
                        Destroy(unit);
                    }
                    break;
                default:
                    throw new NotImplementedException(nameof(action));
            }
            

        }
    }
}


