using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace PS.Units
{
    public class UnitHandler : MonoBehaviour
    {
        public static UnitHandler instance;
        
        [SerializeField]
        private Unit warrior, shooter, healer, worker;
        
        void Start()
        {
            instance = this;
        }

        public (int cost, int attack, int attackRange, int health, int armor) GetUnitStats(string type)
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
                    return (0, 0, 0, 0, 0);
            }

            return (unit.cost, unit.attack, unit.attackRange, unit.health, unit.armor);
        }

        public void SetUnitStats(Transform type)
        {
            foreach (Transform child in type)
            {
                foreach (Transform unit in child)
                {
                    string unitName = child.name.Substring(0, child.name.Length - 1).ToLower();
                    var stats = GetUnitStats(unitName);
                    Player.PlayerUnit pU;
                    
                    if (type == PS.Player.PlayerManager.instance.playerUnits)
                    {
                        pU = unit.GetComponent<Player.PlayerUnit>();
                        // set unit stats in each unit
                        pU.cost = stats.cost;
                        pU.attack = stats.attack;
                        pU.attackRange = stats.attackRange;
                        pU.health = stats.health;
                        pU.armor = stats.armor;
                    }
                    else if (type == PS.Player.PlayerManager.instance.enemyUnits)
                    {
                        // set enemy stats
                    }
                    
                    // if we have any upgrade add them now
                    // add upgrades to unit stats
                }
            }
        }
    }
}

