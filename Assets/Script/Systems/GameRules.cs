using System;
using System.Data;
using UnityEngine;
using Script;
using UnityEngine.AI;

namespace Script.Systems
{
    public class GameRules : MonoBehaviour
    {
        public GameObject casernePrefab;
        public GameObject selectCirclePrefab;
        public GameObject terrain;
        public GameObject casernesAlliees;
        public GameObject pastilleMinimap;
        public GameObject unitStatsDisplay;

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

                        // pour pouvoir etre cliqué
                        go.layer = LayerMask.NameToLayer("PlayerUnits");
                        go.name = "Titikaka";
                        
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
                        
                        // pastille minimap
                        var pastille = Instantiate(pastilleMinimap, go.transform);
                        pastille.transform.parent = go.transform;
                        
                        // Barre de vie
                        var usd = Instantiate(unitStatsDisplay, go.transform);
                        usd.transform.parent = go.transform;
                        
                        Destroy(unit);
                    }
                    break;
                default:
                    throw new NotImplementedException(nameof(action));
            }
        }
    }
}


