using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

namespace PS.Units.Enemy
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class EnemyUnit : MonoBehaviour
    {
        public UnitHandler unitHandler;
        private NavMeshAgent navAgent;
        
        public UnitStatTypes.Base baseStats;
        
        private Collider[] rangeColliders;
        
        private Transform aggroTarget;

        private UnitStatDisplay aggroUnit;
        
        private bool hasAggro = false;
        
        private float distance;

        public float attackCooldown;
        
        public UnitConfigManager unitConfig;
        
        private bool isPlayerUnit;
        
        // OnEnable est appelé quand le script est activé.
        private void OnEnable()
        {
            // Initialise la référence au composant NavMeshAgent.
            navAgent = GetComponent<NavMeshAgent>();
            attackCooldown = baseStats.attackCooldown;
        }

        private void Start()
        {
            ApplyConfig(transform.parent);
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
                Attack();
                attackCooldown -= Time.deltaTime;
            }
            
            if (attackCooldown <= -0.3)
            {
                attackCooldown = baseStats.attackCooldown;
            }
        }
        
        public void ApplyConfig(Transform parent)
        {
            isPlayerUnit = false;
            UnitConfig config = unitConfig.GetConfig(parent.name);
            if (config != null)
            {
                transform.localScale = config.scale;
                Renderer renderer = GetComponent<Renderer>();
                if (renderer != null)
                {
                    renderer.material = isPlayerUnit ? config.playerMaterial : config.enemyMaterial;
                }
            }
            else
            {
                Debug.LogWarning("No config found for unit type: " + parent.name);
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
            if (distance < baseStats.attackRange && attackCooldown <= 0)
            {
                attackCooldown = baseStats.attackCooldown;
                aggroUnit.TakeDamage(baseStats.attack);
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

