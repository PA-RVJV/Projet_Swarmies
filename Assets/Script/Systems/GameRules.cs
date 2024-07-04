using System;
using System.Data;
using PS.Units;
using PS.Units.Player;
using UnityEngine;
using Script;
using UnityEngine.AI;

#nullable enable

namespace Script.Systems
{
    public class GameRules : MonoBehaviour
    {
        //public UnitConfigManager ucm;
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

                        PlayerUnit pus = go.GetComponent<PlayerUnit>();
                        pus.unitConfig = transform.Find("UnitConfigManager").GetComponent<UnitConfigManager>();
                        pus.unitHandler = GetComponent<UnitHandler>();

                        // pour pouvoir etre cliqué
                        go.layer = LayerMask.NameToLayer("PlayerUnits");
                        go.name = "Alibaba";
                        
                        // bloqueuer de pqthfinding
                        var nvo = go.AddComponent<NavMeshObstacle>();
                        nvo.carving = true;

                        // cercle de selectiom
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


