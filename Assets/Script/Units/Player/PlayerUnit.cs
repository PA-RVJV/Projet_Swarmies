using UnityEngine;
using UnityEngine.AI;
using PS.Units.Enemy;


namespace PS.Units.Player
{
    // Le déplacement est assuré par le package AI Navigation 
    [RequireComponent(typeof(NavMeshAgent))]
    public class PlayerUnit : MonoBehaviour
    {
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
        private bool isAttacked;
        public float currentHealth;
        
        // OnEnable est appelé quand le script est activé.
        private void OnEnable()
        {
            // Initialise la référence au composant NavMeshAgent.
            navAgent = GetComponent<NavMeshAgent>();
            navAgent.speed = 3f;
            navAgent.angularSpeed = 120f;
            navAgent.avoidancePriority = 50;
            isAttacked = GetComponent<NavMeshAgent>() && GetComponent<NavMeshAgent>().enabled;
            
            if (unitConfig == null)
            {
                
            }
        }

        private void Start()
        {
            ApplyConfig(transform.parent);
            attackCooldown = baseStats.attackCooldown;
            currentHealth = baseStats.health;
            unitStatDisplay = gameObject.GetComponentInChildren<UnitStatDisplay>();
        }
        
        private void OnDestroy()
        {
            Debug.Log("destroyed");
        }
        
        private void Update()
        {
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
                //CheckAggroDistance();
                attackCooldown -= Time.deltaTime;
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
            unitStatDisplay.HandleHealth(currentHealth);
            transform.position = Vector3.Lerp(transform.position, navAgent.nextPosition, Time.deltaTime * navAgent.speed);
        }

        // Méthode pour déplacer l'unité vers une destination spécifique.
        public void MoveUnit(Vector3 destination)
        {
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
            if (parent.parent.name == "Player Units")
            {
                isPlayerUnit = true;
            }
            else
            {
                isPlayerUnit = false;
            }
            
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
            if (distance <= baseStats.attackRange && attackCooldown <= 0)
            {
                attackCooldown = baseStats.attackCooldown;
                EnemyUnit enemyUnit = aggroTarget.GetComponent<EnemyUnit>();
                if (enemyUnit != null)
                {
                    enemyUnit.TakeDamage(baseStats.attack);
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
                distance = Vector3.Distance(aggroTarget.position, transform.position);
                navAgent.stoppingDistance = baseStats.attackRange;
                
                if (isDeplaced != true)
                {
                    //if (_distance <= baseStats.aggroRange)
                    //{
                    navAgent.SetDestination(aggroTarget.position);
                    //}
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
