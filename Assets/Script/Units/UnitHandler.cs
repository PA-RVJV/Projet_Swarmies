using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

using PS.Player;

namespace PS.Units
{
    public class UnitHandler : MonoBehaviour
    {
        public static UnitHandler instance;
        
        [SerializeField]
        private Unit warrior, shooter, healer, worker;

        public LayerMask pUnitLayer;
        public LayerMask eUnitLayer;

        private void Awake()
        {
            instance = this;
        }
        void Start()
        {
            pUnitLayer = LayerMask.NameToLayer("PlayerUnits");
            eUnitLayer = LayerMask.NameToLayer("EnemyUnits");
        }

        public UnitStatTypes.Base GetUnitStats(string type)
        {
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
                default:
                    Debug.Log($"Unit Type : {type} not found");
                    return null;
            }

            return unit.baseStats;
        }

        public void SetUnitStats(Transform type)
        {
            Transform pUnits = PlayerManager.instance.playerUnits;
            Transform eUnits = PlayerManager.instance.enemyUnits;
            
            foreach (Transform child in type)
            {
                foreach (Transform unit in child)
                {
                    string unitName = child.name.Substring(0, child.name.Length - 1).ToLower();
                    var stats = GetUnitStats(unitName);
                    
                    if (type == pUnits)
                    {
                        Player.PlayerUnit pU = unit.GetComponent<Player.PlayerUnit>();
                        pU.baseStats = GetUnitStats(unitName);
                    }
                    else if (type == eUnits)
                    {
                        Enemy.EnemyUnits eU = unit.GetComponent<Enemy.EnemyUnits>();
                        // set unit stats in each unit
                        eU.baseStats = GetUnitStats(unitName);
                    }
                }
            }
        }
    }
}

