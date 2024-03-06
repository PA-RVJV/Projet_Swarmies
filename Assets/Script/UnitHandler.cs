using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

using PS.Units;

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

        public (float cost, float attack, float attackRange, float aggroRange, float health, float armor) GetUnitStats(string type)
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
                    return (0, 0, 0, 0, 0, 0);
            }

            return (unit.baseStats.cost, unit.baseStats.attack, unit.baseStats.attackRange, unit.baseStats.aggroRange, unit.baseStats.health, unit.baseStats.armor);
        }

        public void SetUnitStats(Transform type)
        {
            Transform pUnits = Player.PlayerManager.instance.playerUnits;
            Transform eUnits = Player.PlayerManager.instance.enemyUnits;
            
            foreach (Transform child in type)
            {
                foreach (Transform unit in child)
                {
                    string unitName = child.name.Substring(0, child.name.Length - 1).ToLower();
                    var stats = GetUnitStats(unitName);
                    
                    if (type == pUnits)
                    {
                        Player.PlayerUnit pU = unit.GetComponent<Player.PlayerUnit>();
                        // set unit stats in each unit
                        pU.baseStats.cost = stats.cost;
                        pU.baseStats.attack = stats.attack;
                        pU.baseStats.attackRange = stats.attackRange;
                        pU.baseStats.aggroRange = stats.aggroRange;
                        pU.baseStats.health = stats.health;
                        pU.baseStats.armor = stats.armor;
                    }
                    else if (type == eUnits)
                    {
                        Enemy.EnemyUnits eU = unit.GetComponent<Enemy.EnemyUnits>();
                        // set unit stats in each unit
                        eU.baseStats.cost = stats.cost;
                        eU.baseStats.attack = stats.attack;
                        eU.baseStats.attackRange = stats.attackRange;
                        eU.baseStats.aggroRange = stats.aggroRange;
                        eU.baseStats.health = stats.health;
                        eU.baseStats.armor = stats.armor;
                    }
                    
                    // if we have any upgrade add them now
                    // add upgrades to unit stats
                }
            }
        }
    }
}

