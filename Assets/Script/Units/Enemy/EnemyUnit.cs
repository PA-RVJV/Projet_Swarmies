using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

namespace PS.Units.Enemy
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class EnemyUnits : MonoBehaviour
    {
        private NavMeshAgent navAgent1;
        
        public UnitStatTypes.Base baseStats;
        
        private Collider[] rangeColliders;
        
        private Transform aggroTarget;

        private UnitStatDisplay aggroUnit;
        
        private bool hasAggro = false;
        
        private float distance;

        public float attackCooldown;
        
        private void Start()
        {
            navAgent1 = gameObject.GetComponent<NavMeshAgent>();
        }

        private void Update()
        {
            attackCooldown -= Time.deltaTime;
            
            if (!hasAggro)
            {
                CheckForEnemyTarget();
            }
            else
            {
                Attack();
                MoveToAggroTarget();
            }
        }
        
        private void CheckForEnemyTarget()
        {
            rangeColliders = Physics.OverlapSphere(transform.position, baseStats.aggroRange);

            for (int i = 0; i < rangeColliders.Length; i++)
            {
                if (rangeColliders[i].gameObject.layer == UnitHandler.instance.pUnitLayer)
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
            if (attackCooldown <= 0 && distance <= baseStats.attackRange)
            {
                aggroUnit.TakeDamage(baseStats.attack);
                attackCooldown = baseStats.attackSpeed;
            }
        }
        
        
        private void MoveToAggroTarget()
        {
            if (aggroTarget == null)
            {
                navAgent1.SetDestination(transform.position);
                hasAggro = false;
            }
            else
            {
                distance = Vector3.Distance(aggroTarget.position, transform.position);
                navAgent1.stoppingDistance = baseStats.attackRange;

                if (distance <= baseStats.aggroRange)
                {
                    navAgent1.SetDestination(aggroTarget.position);
                }
            }
        }
    }
}

