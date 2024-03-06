using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace PS.Units.Enemy
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class EnemyUnits : MonoBehaviour
    {
        private NavMeshAgent navAgent1;
        
        public UnitStatTypes.Base baseStats;
        private Collider[] rangeColliders;
        private Transform aggroTarget;
        private bool hasAggro = false;
        private float distance;

        
        private void Start()
        {
            navAgent1 = gameObject.GetComponent<NavMeshAgent>();
        }

        private void Update()
        {
            if (!hasAggro)
            {
                CheckForEnemyTarget();
            }
            else
            {
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
                    hasAggro = true;
                    break;
                }
            }
        }

        private void MoveToAggroTarget()
        {
            distance = Vector3.Distance(aggroTarget.position, transform.position);
            navAgent1.stoppingDistance = baseStats.attackRange + 1;

            if (distance <= baseStats.aggroRange)
            {
                navAgent1.SetDestination(aggroTarget.position);
            }
        }
    }
}

