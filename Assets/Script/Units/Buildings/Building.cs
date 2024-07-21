using System.Collections;
using System.Collections.Generic;
using PS.Units;
using UnityEngine;
using PS.Units.Enemy;
using PS.Units.Player;


public class Building : MonoBehaviour
{
        public UnitHandler unitHandler;
        
        public UnitStatTypes.Base baseStats;
        
        private Collider[] rangeColliders;
        
        private Transform aggroTarget;

        private UnitStatDisplay unitStatDisplay;
        
        private bool hasAggro = false;
        
        private float distance;

        public float attackCooldown;
        
        public float currentHealth;

        private void Start()
        {
            currentHealth = baseStats.health;
            unitStatDisplay = gameObject.GetComponentInChildren<UnitStatDisplay>();
        }
        
        private void Update()
        {
            if (!hasAggro)
            {
                CheckForEnemyTarget();
            }
            else
            {
                Attack();
                attackCooldown -= Time.deltaTime;
            }
            unitStatDisplay.HandleHealth(currentHealth);
        }
        
        private void CheckForEnemyTarget()
        {
            rangeColliders = Physics.OverlapSphere(transform.position, baseStats.aggroRange);

            for (int i = 0; i < rangeColliders.Length; i++)
            {
                if (rangeColliders[i]?.gameObject.layer == unitHandler.pUnitLayer)
                {
                    aggroTarget = rangeColliders[i].gameObject.transform;
                    hasAggro = true;
                    break;
                }
            }
        }

        private void Attack()
        {
            if (distance <= baseStats.attackRange && attackCooldown <= 0)
            {
                attackCooldown = baseStats.attackCooldown;
                EnemyUnit enemyUnit = aggroTarget.GetComponent<EnemyUnit>();
                if (enemyUnit is not null)
                {
                    enemyUnit.TakeDamage(baseStats.attack, transform);
                }
                else
                {
                    PlayerUnit playerUnit = aggroTarget.GetComponent<PlayerUnit>();
                    if (playerUnit is not null)
                    {
                        playerUnit.TakeDamage(baseStats.attack, transform);
                    }
                }
            }
        }
        
        public void TakeDamage(float damage)
        {
            float totalDamage = damage - baseStats.armor;
            currentHealth -= totalDamage;
            if (currentHealth <= 0)
            {
                Die();
            }
        }

        private void Die()
        {
            Destroy(gameObject);
        }
}
