using System;
using System.Collections.Generic;
using System.Collections;
using System.Data;
using PS.Units;
using PS.Units.Player;
using UnityEngine;
using UnityEngine.AI;

namespace Script.Systems
{
    public class GameRules : MonoBehaviour
    {
        public GameObject casernePrefab;
        public GameObject entrepotPrefab;
        public GameObject selectCirclePrefab;
        public GameObject terrain;
        public GameObject casernesAlliees;
        public GameObject entrepotsAllies;
        public GameObject pastilleMinimap;
        public GameObject unitStatsDisplay;
        
        public GameObject warriorsAllies;
        public GameObject shootersAllies;
        public GameObject healersAllies;
        public GameObject workersAllies;

        public float closeRange; 
        
        private TreeInstance[] initialTreeInstances;
        private Terrain activeTerrain;
        private TerrainData activeTerrainData;
        
        private void OnEnable()
        {
            // Stocker l'état initial des arbres au démarrage du jeu
            activeTerrain  = Terrain.activeTerrain;
            activeTerrainData = activeTerrain.terrainData;
            initialTreeInstances = activeTerrainData.treeInstances.Clone() as TreeInstance[];
        }

        
        // TODO PROBLEME les arbres ne réaparaisse pas toujours .... a cause de initialTreeInstances ?
        private void OnDisable()
        {
            // Restaurer l'état initial des arbres lorsque le jeu s'arrête
            if (initialTreeInstances != null)
            {
                if (activeTerrain != null)
                {
                    if (activeTerrainData != null)
                    {
                        activeTerrainData.treeInstances = initialTreeInstances;
                        activeTerrain.Flush();
                    }
                }
            }
        }
        
        public void DealWithAction(UnitActionsEnum action, GameObject?[] source)
        {
            if (!terrain || !casernePrefab)
                throw new ConstraintException();
            foreach (var unit in source)
            {
                switch (action)
                {
                    case UnitActionsEnum.Construire:
                    {
                        if (!unit)
                            continue;
                        var go = Instantiate(casernePrefab, unit.transform.position, unit.transform.rotation);
                        go.transform.parent = casernesAlliees.transform;
                        go.name = casernePrefab.name;

                        PlayerUnit pus = go.GetComponent<PlayerUnit>();
                        var ucf = transform.Find("UnitConfigManager").GetComponent<UnitConfigManager>();
                        pus.unitConfig = ucf;
                        pus.unitHandler = GetComponent<UnitHandler>();
                        pus.baseStats = pus.unitHandler.GetUnitStats("caserne");

                        // Le script de spawn attachée a la caserne
                        var spaner = go.GetComponent<SpawnerUnit>();
                        spaner.unitConfigManager = ucf;

                        checkForTreesIntersecting(go);

                        // pour pouvoir etre cliqué
                        go.layer = LayerMask.NameToLayer("PlayerUnits");

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
                        pastille.transform.SetParent(go.transform, false);

                        // Barre de vie
                        var usd = Instantiate(unitStatsDisplay, go.transform);
                        usd.transform.SetParent(go.transform, false);

                        Destroy(unit);
                }

                break;
                    case UnitActionsEnum.ConstruireEntrepot:
                    {
                        if (!unit)
                            continue;
                        var go = Instantiate(entrepotPrefab, unit.transform.position, unit.transform.rotation);
                        go.transform.parent = entrepotsAllies.transform;
                        go.name = entrepotPrefab.name;

                        PlayerUnit pus = go.GetComponent<PlayerUnit>();
                        var ucf = transform.Find("UnitConfigManager").GetComponent<UnitConfigManager>();
                        pus.unitConfig = ucf;
                        pus.unitHandler = GetComponent<UnitHandler>();
                        pus.baseStats = pus.unitHandler.GetUnitStats("entrepot");
                        
                        checkForTreesIntersecting(go);

                        // pour pouvoir etre cliqué
                        go.layer = LayerMask.NameToLayer("PlayerUnits");

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
                        pastille.transform.SetParent(go.transform, false);

                        // Barre de vie
                        var usd = Instantiate(unitStatsDisplay, go.transform);
                        usd.transform.SetParent(go.transform, false);

                        Destroy(unit);
                    }
                        break;

                    case UnitActionsEnum.ConvertirEnWarriors:
                    {
                        var pu = unit.GetComponent<SpawnerUnit>();
                        pu.unitToSpawn = "Warrior";
                        pu.myparent = warriorsAllies.transform;

                        break;
                    }
                    case UnitActionsEnum.ConvertirEnShooters:
                    {
                        var pu = unit.GetComponent<SpawnerUnit>();
                        pu.unitToSpawn = "Shooter";
                        pu.myparent = shootersAllies.transform;
                        
                        break;
                    }
                    case UnitActionsEnum.ConvertirEnHealers:
                    {
                        var pu = unit.GetComponent<SpawnerUnit>();
                        pu.unitToSpawn = "Healer";
                        pu.myparent = healersAllies.transform;

                        break;
                    }
                    default:
                        throw new NotImplementedException(nameof(action));
                }
            }
        }
        
        /**
         * A partir d'une unité sur le terrain, sort toutes les actions
         * que cette unité peut faire
         */
        public IEnumerable<UnitActionsEnum> yieldActions(Transform unit)
        {
            switch (unit.parent.name)
            {
                case "Workers":
                    yield return UnitActionsEnum.Construire;
                    yield return UnitActionsEnum.ConstruireEntrepot;
                    break;
                case "Casernes":
                {
                    yield return UnitActionsEnum.ConvertirEnWarriors;
                    yield return UnitActionsEnum.ConvertirEnShooters;
                    yield return UnitActionsEnum.ConvertirEnHealers;
                    break;
                }
            }
        }

        private void checkForTreesIntersecting(GameObject building)
        {
            TreeInstance[] treeInstances = activeTerrainData.treeInstances;
            
            Vector3 buildingPosition = building.transform.position;

            for (int i = 0; i < treeInstances.Length; i++)
            {
                TreeInstance tree = treeInstances[i];
                Vector3 treeWorldPosition = Vector3.Scale(tree.position, activeTerrainData.size) + activeTerrain.transform.position;
                float distance = Vector3.Distance(treeWorldPosition, buildingPosition);

                if (distance < closeRange)
                {
                    // L'arbre est à portée - le masquer en réduisant son échelle
                    tree.widthScale = 0;
                    tree.heightScale = 0;
                    treeInstances[i] = tree; // Mettre à jour l'instance de l'arbre modifiée dans le tableau
                }
            }

            // Mettre à jour les instances d'arbres du terrain
            activeTerrainData.treeInstances = treeInstances;
            activeTerrain.Flush();

        }
    }
}


