using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using PS.Units.Player;
using UnityEditor;

namespace PS.Units.Enemy
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class EnemyUnit : MonoBehaviour
    {
        public UnitHandler unitHandler;
        public UnitStatTypes.Base baseStats;
        public UnitConfigManager unitConfig;
        public float attackCooldown;
        
        private bool isPlayerUnit;
        
        private NavMeshAgent navAgent;
        private Collider[] rangeColliders;
        public Transform aggroTarget;
        private UnitStatDisplay unitStatDisplay;
        
        private bool hasAggro = false;
        private float distance;
        private bool isAttacked;
        public float currentHealth;
        
        private Coroutine attackCoroutine;
        
        
        // OnEnable est appelé quand le script est activé.
        private void OnEnable()
        {
            // Initialise la référence au composant NavMeshAgent.
            navAgent = GetComponent<NavMeshAgent>();
            navAgent.speed = 3f;
            navAgent.angularSpeed = 120f;
            navAgent.avoidancePriority = 50;
            navAgent.stoppingDistance = 0.6f;
            isAttacked = GetComponent<NavMeshAgent>() && GetComponent<NavMeshAgent>().enabled;
        }

        private void Start()
        {
            if (gameObject.name.Contains("Caserne"))
            {
                baseStats = unitHandler.GetUnitStats("caserne");
            }
            
            ApplyConfig(transform.parent);
            currentHealth = baseStats.health;
            attackCooldown = baseStats.attackCooldown;
            unitStatDisplay = gameObject.GetComponentInChildren<UnitStatDisplay>();
            DetectAndAvoidOverlap();
        }
        
        private void OnDestroy()
        {
            if (attackCoroutine != null)
            {
                StopCoroutine(attackCoroutine);
            }
        }
        
        private void Update()
        {
            unitStatDisplay.HandleHealth(currentHealth);
            if (!isAttacked)
                return;
            
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
            
            transform.position = Vector3.Lerp(transform.position, navAgent.nextPosition, Time.deltaTime * navAgent.speed);
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
            if (distance <= baseStats.attackRange && attackCooldown <= 0 && aggroTarget)
            {
                attackCooldown = baseStats.attackCooldown;
                PlayerUnit playerUnit = aggroTarget.GetComponent<PlayerUnit>();
                if (playerUnit is not null)
                {
                    playerUnit.TakeDamage(baseStats.attack, transform);
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
            if (!isAttacked)
                return;
            if (aggroTarget == null)
            {
                navAgent.SetDestination(transform.position);
                hasAggro = false;
            }
            else
            {
                distance = Vector3.Distance(aggroTarget.position, transform.position);

                if (distance >= baseStats.attackRange)
                {
                    if (navAgent)
                    {
                        navAgent.SetDestination(aggroTarget.position);
                    }
                }
            }
        }
        public void TakeDamage(float damage, Transform attacker)
        {
            float totalDamage = damage - baseStats.armor;
            currentHealth -= totalDamage;
            if (currentHealth <= 0)
            {
                Die();
            }
            else
            {
                aggroTarget = attacker;
                hasAggro = true;
            }
        }

        private void Die()
        {
            Destroy(gameObject);
        }
        
        private void DetectAndAvoidOverlap()
        {
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, 0.5f); // Smaller overlap radius
            foreach (var hitCollider in hitColliders)
            {
                if (hitCollider != this.GetComponent<Collider>())
                {
                    Vector3 randomOffset = new Vector3(Random.Range(-0.5f, 0.5f), 0, Random.Range(-0.5f, 0.5f)); // Smaller random offset
                    navAgent.Warp(transform.position + randomOffset);
                }
            }
        }
        
        public void SetAttackTarget(Vector3 targetPosition)
        {
            aggroTarget = null;
            hasAggro = false;
            if (attackCoroutine != null)
            {
                StopCoroutine(attackCoroutine);
            }
            attackCoroutine = StartCoroutine(MoveToTargetWithAggro(targetPosition));
        }

        public void SetDefendTarget(Vector3 defendPosition)
        {
            aggroTarget = null;
            hasAggro = false;
            if (attackCoroutine != null)
            {
                StopCoroutine(attackCoroutine);
            }
            navAgent.SetDestination(defendPosition);
        }

        private IEnumerator MoveToTargetWithAggro(Vector3 targetPosition)
        {
            while (Vector3.Distance(transform.position, targetPosition) > navAgent.stoppingDistance)
            {
                if (!hasAggro)
                {
                    CheckForEnemyTarget();
                }
                if (hasAggro)
                {
                    MoveToAggroTarget();
                    if (aggroTarget == null)
                    {
                        hasAggro = false;
                    }
                }
                else
                {
                    navAgent.SetDestination(targetPosition);
                }
                yield return null;
            }
        }
        
        
    }
}

