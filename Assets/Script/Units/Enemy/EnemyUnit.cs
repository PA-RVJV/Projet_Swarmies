using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using PS.Units.Player;

namespace PS.Units.Enemy
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class EnemyUnit : MonoBehaviour
    {
        public UnitHandler unitHandler;
        private NavMeshAgent navAgent;
        
        public UnitStatTypes.Base baseStats;
        
        private Collider[] rangeColliders;
        
        public Transform aggroTarget;
        
        private bool hasAggro = false;
        
        private float distance;
        
        private UnitStatDisplay unitStatDisplay;

        public float attackCooldown;
        
        public UnitConfigManager unitConfig;
        
        private bool isPlayerUnit;
        
        public float currentHealth;
        
        // OnEnable est appelé quand le script est activé.
        private void OnEnable()
        {
            // Initialise la référence au composant NavMeshAgent.
            navAgent = GetComponent<NavMeshAgent>();
            navAgent.speed = 3f;
            navAgent.angularSpeed = 120f;
            navAgent.avoidancePriority = 50;
        }

        private void Start()
        {
            ApplyConfig(transform.parent);
            currentHealth = baseStats.health;
            attackCooldown = baseStats.attackCooldown;
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
                MoveToAggroTarget();
                Attack();
                attackCooldown -= Time.deltaTime;
            }
            
            if (attackCooldown <= -0.3)
            {
                attackCooldown = baseStats.attackCooldown;
            }
            unitStatDisplay.HandleHealth(currentHealth);
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
                PlayerUnit playerUnit = aggroTarget.GetComponent<PlayerUnit>();
                if (playerUnit is not null)
                {
                    playerUnit.TakeDamage(baseStats.attack);
                }
                else
                {
                    Building buildingUnit = aggroTarget.GetComponent<Building>();
                    if (buildingUnit is not null)
                    {
                        buildingUnit.TakeDamage(baseStats.attack);
                    }
                }
            }
        }
        
        
        public void MoveToAggroTarget()
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

                if (distance >= baseStats.attackRange)
                {
                    navAgent.SetDestination(aggroTarget.position);
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
}

