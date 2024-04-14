using System.Collections;
using System.Collections.Generic;
using PS.InputHandlers;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

namespace PS.Units.Player
{
    // Le déplacement est assuré par le package AI Navigation 
    [RequireComponent(typeof(NavMeshAgent))]
    public class PlayerUnit : MonoBehaviour
    {
        // Variable privée pour stocker la référence au composant NavMeshAgent de l'unité.
        private NavMeshAgent navAgent;

        public UnitStatTypes.Base baseStats;
        
        private Collider[] rangeColliders;
        
        private Transform aggroTarget;
        
        private UnitStatDisplay aggroUnit;
        
        private bool hasAggro = false;
        
        private float distance;

        public float attackCooldown;
        
        
        // OnEnable est appelé quand le script est activé.
        private void OnEnable()
        {
            // Initialise la référence au composant NavMeshAgent.
            navAgent = GetComponent<NavMeshAgent>();
        }
        
        // Méthode pour déplacer l'unité vers une destination spécifique.
        public void MoveUnit(Vector3 _destination)
        {
            // Utilise le NavMeshAgent pour définir la destination de l'unité.
            navAgent.SetDestination(_destination);
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
                Debug.Log(attackCooldown);
                MoveToAggroTarget();
            }
        }

        private void CheckForEnemyTarget()
        {
            rangeColliders = Physics.OverlapSphere(transform.position, baseStats.aggroRange);

            for (int i = 0; i < rangeColliders.Length; i++)
            {
                if (rangeColliders[i].gameObject.layer == UnitHandler.instance.eUnitLayer)
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
                navAgent.SetDestination(transform.position);
                hasAggro = false;
            }
            else
            {
                distance = Vector3.Distance(aggroTarget.position, transform.position);
                navAgent.stoppingDistance = baseStats.attackRange;

                if (distance <= baseStats.aggroRange)
                {
                    navAgent.SetDestination(aggroTarget.position);
                }
            }
        }
    }
}