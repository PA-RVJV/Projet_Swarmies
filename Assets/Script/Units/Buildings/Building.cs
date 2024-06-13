using System.Collections;
using System.Collections.Generic;
using PS.Units;
using UnityEngine;

public class Building : MonoBehaviour
{
        public UnitHandler unitHandler;
        
        public UnitStatTypes.Base baseStats;
        
        private Collider[] rangeColliders;
        
        private Transform aggroTarget;

        private UnitStatDisplay aggroUnit;
        
        private bool hasAggro = false;
        
        private float distance;

        public float attackCooldown;

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
        }
        
        private void CheckForEnemyTarget()
        {
            rangeColliders = Physics.OverlapSphere(transform.position, baseStats.aggroRange);

            for (int i = 0; i < rangeColliders.Length; i++)
            {
                if (rangeColliders[i]?.gameObject.layer == unitHandler.pUnitLayer)
                {
                    aggroTarget = rangeColliders[i].gameObject.transform;
                    aggroUnit = aggroTarget.gameObject.GetComponentInChildren<UnitStatDisplay>();
                    hasAggro = true;
                    break;
                }
            }
        }

        private void Attack()
        {
            if (attackCooldown <= 0 && distance < baseStats.attackRange)
            {
                aggroUnit.TakeDamage(baseStats.attack);
                attackCooldown = baseStats.attackCooldown;
            }
        }
}
