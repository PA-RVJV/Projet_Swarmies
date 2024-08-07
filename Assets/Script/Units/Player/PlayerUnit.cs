using UnityEngine;
using System.Collections;
using UnityEngine.AI;
using PS.Units.Enemy;
using UnityEditor;


namespace PS.Units.Player
{
    // Le déplacement est assuré par le package AI Navigation 
    [RequireComponent(typeof(NavMeshAgent))]
    public class PlayerUnit : MonoBehaviour
    {
        private AudioManager audioManager;
        
        public UnitHandler unitHandler;
        public UnitStatTypes.Base baseStats;
        public UnitConfigManager unitConfig;
        public float attackCooldown;
        
        private bool isPlayerUnit;
        
        // Variable privée pour stocker la référence au composant NavMeshAgent de l'unité.
        private NavMeshAgent navAgent;
        private Collider[] rangeColliders = {};
        private Transform aggroTarget;
        private UnitStatDisplay unitStatDisplay;
        
        private bool hasAggro;
        private bool isDeplaced;
        private float distance;
        private bool isUnit;
        public float currentHealth;
        private bool isHealer;
        
        private Coroutine attackCoroutine;
        private PlayerUnit allyUnit; // Variable pour stocker l'unité alliée actuelle
        
        public float interval = 0.5f;
        private float timer = 0f;


        private void Awake()
        {
            audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
        }
        // OnEnable est appelé quand le script est activé.
        private void OnEnable()
        {
            // Initialise la référence au composant NavMeshAgent.
            isUnit = GetComponent<NavMeshAgent>() && GetComponent<NavMeshAgent>().enabled;
        }

        private void Start()
        {
            navAgent = GetComponent<NavMeshAgent>();
            navAgent.speed = baseStats.speed;
            navAgent.angularSpeed = 120f;
            navAgent.avoidancePriority = 50;
            navAgent.stoppingDistance = 0.6f;
            ApplyConfig(transform.parent);
            isHealer = baseStats.isHealer;
            attackCooldown = baseStats.attackCooldown;
            currentHealth = baseStats.health;
            unitStatDisplay = gameObject.GetComponentInChildren<UnitStatDisplay>();
            DetectAndAvoidOverlap();
        }
        
        private void OnDestroy()
        {
            Debug.Log("destroyed");
            if (attackCoroutine != null)
            {
                StopCoroutine(attackCoroutine);
            }
        }
        
        private void Update()
        {
            unitStatDisplay.HandleHealth(currentHealth);
            if (!isUnit)
                return;

            if (isHealer)
            {
                if (!hasAggro)
                {
                    timer += Time.deltaTime;
                    if (timer >= interval)
                    {
                        CheckForAlliedTarget();
                        timer = 0f;
                    }
                }
                else
                {
                    MoveToAggroTarget();
                    Heal();
                    attackCooldown -= Time.deltaTime;
                
                    // Vérifie si l'unité ciblée est complètement guérie
                    if (allyUnit != null && allyUnit.currentHealth >= allyUnit.baseStats.health)
                    {
                        hasAggro = false;
                        aggroTarget = null;
                        allyUnit = null; // Réinitialise l'unité alliée
                    }
                }

                
            }
            else
            {
                if (!hasAggro)
                {
                    CheckForEnemyTarget();
                }
                else
                {
                    MoveToAggroTarget();
                    timer += Time.deltaTime;
                    if (timer >= interval)
                    {
                        Attack();
                        timer = 0f;
                    }

                    
                    CheckAggroDistance();
                    attackCooldown -= Time.deltaTime;
                }
            }
            
            if (isDeplaced == true)
            {
                if (navAgent.remainingDistance <= navAgent.stoppingDistance) // Vérifie si l'agent est assez proche de la destination
                {
                    if (!navAgent.hasPath || navAgent.velocity.sqrMagnitude == 0f) // Vérifie si l'agent est arrêté
                    {
                        isDeplaced = false;
                    }
                }
            }

            if (attackCooldown <= -0.3)
            {
                attackCooldown = baseStats.attackCooldown;
            }

            // Interpoler les positions pour un mouvement plus fluide
            transform.position = Vector3.Lerp(transform.position, navAgent.nextPosition, Time.deltaTime * navAgent.speed);
        }

        // Méthode pour déplacer l'unité vers une destination spécifique.
        public void MoveUnit(Vector3 destination)
        {
            navAgent.stoppingDistance = 0.1f;
            if (!isUnit)
                return;
            if (!navAgent)
                return;
            // Utilise le NavMeshAgent pour définir la destination de l'unité.
            isDeplaced = true;
            navAgent.SetDestination(destination);
        }
        
        public void TargetEnemy(Transform TargetedEnemy)
        {
            aggroTarget = TargetedEnemy;
            hasAggro = true;
        }
        
        private void ApplyConfig(Transform parent)
        {
            isPlayerUnit = parent.parent.name == "Player Units";
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
                if (rangeColliders[i]?.gameObject.layer == unitHandler.eUnitLayer)
                {
                    aggroTarget = rangeColliders[i].gameObject.transform;
                    hasAggro = true;
                    break; 
                }
            }
        }

        private void CheckForAlliedTarget()
        {
            rangeColliders = Physics.OverlapSphere(transform.position, baseStats.aggroRange);

            for (int i = 0; i < rangeColliders.Length; i++)
            {
                var unit = rangeColliders[i].GetComponent<PlayerUnit>();
                if (unit != null && unit != this && unit.currentHealth < unit.baseStats.health)
                {
                    aggroTarget = unit.transform;
                    allyUnit = unit; // Stocke la référence de l'unité alliée
                    hasAggro = true;
                    break;
                }
            }
        }

        private void Heal()
        {
            if (distance <= baseStats.attackRange && attackCooldown <= 0 && aggroTarget)
            {
                attackCooldown = baseStats.attackCooldown;
                if (allyUnit != null)
                {
                    allyUnit.currentHealth = Mathf.Min(allyUnit.baseStats.health, allyUnit.currentHealth + baseStats.attack);
                    // Ajoutez une animation ou un effet de particules pour le soin ici, si nécessaire.
                }
            }
        }
        
        private void CheckAggroDistance()
        {
            if (aggroTarget != null)
            {
                distance = Vector3.Distance(aggroTarget.position, transform.position);
                if (distance >= baseStats.attackRange)
                {
                    hasAggro = false;
                    aggroTarget = null;
                }
            }
        }


        private void Attack()
        {
            if (distance <= baseStats.attackRange && attackCooldown <= 0 & aggroTarget)
            {
                attackCooldown = baseStats.attackCooldown;
                EnemyUnit enemyUnit = aggroTarget.GetComponent<EnemyUnit>();
                if (enemyUnit != null)
                {
                    enemyUnit.TakeDamage(baseStats.attack, transform);
                }
                else
                {
                    Building buildingUnit = aggroTarget.GetComponent<Building>();
                    if (buildingUnit != null)
                    {
                        buildingUnit.TakeDamage(baseStats.attack);
                    }
                }
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
                navAgent.stoppingDistance = baseStats.attackRange - 0.5f;
                distance = Vector3.Distance(aggroTarget.position, transform.position);
                
                if (isDeplaced != true)
                {
                    //if (_distance <= baseStats.aggroRange)
                    //{
                    if (navAgent)
                    {
                        navAgent.SetDestination(aggroTarget.position);
                    }
                    //}
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

        public void Die()
        {
            if (!isUnit)
            {
                audioManager.PlaySFX(audioManager.destroyUnit);
            }
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
        
        // peut ettre utilisé dans un manager avec une interface pour des déplacement en attaque ou en défense etfc ...
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
        
        // peut ettre utilisé dans un manager avec une interface pour des déplacement en attaque ou en défense etfc ...
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
            while (this != null && Vector3.Distance(transform.position, targetPosition) > navAgent.stoppingDistance)
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
