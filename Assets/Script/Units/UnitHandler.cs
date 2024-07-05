using UnityEngine;

using PS.Player;
using Script.Traits;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace PS.Units
{
    public class UnitHandler : MonoBehaviour
    {
        public PlayerManager playerManager;
        [SerializeField]
        private Unit warrior, shooter, healer, worker, mairie, caserne;

        public LayerMask pUnitLayer;
        public LayerMask eUnitLayer;

        public GameObject _HealthPrefab;
        public GameObject _MinimapPastillePrefab;
        
        void Start()
        {
            pUnitLayer = LayerMask.NameToLayer("PlayerUnits");
            eUnitLayer = LayerMask.NameToLayer("EnemyUnits");

            Assert.IsNotNull(mairie);
            Assert.IsNotNull(worker);
            Assert.IsNotNull(healer);
            Assert.IsNotNull(shooter);
            Assert.IsNotNull(warrior);
            
            Assert.IsNotNull(playerManager);
        }

        public UnitStatTypes.Base GetUnitStats(string type)
        {
            Assert.IsTrue(type.ToLower() == type, "la chaine demandée doit être en minuscules");
            Assert.IsTrue(type[type.Length - 1] != 's', "enlever le s final de la chainje");
            
            Unit unit;
            switch (type)
            {
                case "warrior":
                    unit = warrior;
                    break;
                case "shooter":
                    unit = shooter;
                    break;
                case "healer":
                    unit = healer;
                    break;
                case "worker":
                    unit = worker;
                    break;
                case "mairie":
                    unit = mairie;
                    break;
                case "caserne":
                    unit = caserne;
                    break;
                default:
                    Debug.Log($"Unit Type : {type} not found");
                    Assert.IsTrue(false);
                    return null;
            }

            return unit.baseStats;
        }

        public void SetUnitStats(Transform side)
        {
            Transform pUnits = playerManager.playerUnits;
            Transform eUnits = playerManager.enemyUnits;
            
            foreach (Transform category in side)
            {
                string unitName = category.name.Substring(0, category.name.Length - 1).ToLower();
                var stats = GetUnitStats(unitName);
                
                Assert.IsNotNull(stats);

                foreach (Transform unit in category)
                {
                    if (side == pUnits)
                    {
                        Player.PlayerUnit pU = unit.GetComponent<Player.PlayerUnit>();
                        
                        if (pU == null)
                        {
                            Building building = unit.GetComponent<Building>();
                            //pU = unit.GetComponent<Building>();
                            building.baseStats = GetUnitStats(unitName);
                        } else
                            pU.baseStats = GetUnitStats(unitName);
                    }
                    else if (side == eUnits)
                    {
                        Enemy.EnemyUnit eU = unit.GetComponent<Enemy.EnemyUnit>();
                        // set unit stats in each unit
                        if (eU == null)
                        {
                            Building building = unit.GetComponent<Building>();
                            //pU = unit.GetComponent<Building>();
                            building.baseStats = GetUnitStats(unitName);
                        }
                        else
                            eU.baseStats = GetUnitStats(unitName);

                    }
                    else
                    {
                        Assert.IsTrue(false);
                    }

                    // ajoute une barre de vie et une pastille de minimap si elle est absente
                    if (unit.GetComponentInChildren<HealthBarTrait>() == null)
                    {
                        GameObject go;
                        go=Instantiate(_HealthPrefab, unit.position, Quaternion.identity);
                        go.transform.SetParent(unit, false);

                        go.transform.localPosition = new Vector3(0, 5, 0);
                    }
                    
                    // ajoute une pastille de minimap si elle est absente
                    if (unit.GetComponentInChildren<MinimapPastilleTrait>() == null)
                    {
                        GameObject go;
                        go=Instantiate(_MinimapPastillePrefab, unit.position, Quaternion.Euler(270, 0, 0));
                        go.transform.SetParent(unit, false);
                        go.transform.localPosition = new Vector3(0, 15, 0);

                        if (side == pUnits)
                        {
                            go.GetComponentInChildren<Image>().color = Color.blue;
                        }
                        else if(side == eUnits)
                        {
                            go.GetComponentInChildren<Image>().color = Color.red;
                        }
                    }
                }
            }
        }
    }
}

