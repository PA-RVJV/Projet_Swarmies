using UnityEngine;
using UnityEngine.AI;


namespace PS.Units.Player
{
    // Le déplacement est assuré par le package AI Navigation 
    [RequireComponent(typeof(NavMeshAgent))]
    public class PlayerUnit : MonoBehaviour
    {
        public UnitHandler unitHandler;
        // Variable privée pour stocker la référence au composant NavMeshAgent de l'unité.
        private NavMeshAgent navAgent;

        public UnitStatTypes.Base baseStats;
        
        private Collider[] rangeColliders;
        
        private Transform aggroTarget;
        
        private UnitStatDisplay aggroUnit;
        
        private bool hasAggro = false;

        private bool isDeplaced = false;
        
        private bool isPlayerUnit;
        
        private float distance;

        public float attackCooldown;

        public UnitConfigManager unitConfig;
        
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
        
        private void OnDestroy()
        {
            Debug.Log("destroyed");
        }
        
        // Méthode pour déplacer l'unité vers une destination spécifique.
        public void MoveUnit(Vector3 _destination)
        {
            // Utilise le NavMeshAgent pour définir la destination de l'unité.
            isDeplaced = true;
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
                MoveToAggroTarget();
                Attack();
                CheckAggroDistance();
                attackCooldown -= Time.deltaTime;
            }

            if (isDeplaced)
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

            // introduire une condition avec les batiment de production d'unité pour le changement d'apparence
            //if ()
            //{
            //    unitConfig.ApplyConfig(gameObject, currentParentName);
            //
            //}
        }

        public void ApplyConfig(Transform parent)
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
                    aggroUnit = aggroTarget.gameObject.GetComponentInChildren<UnitStatDisplay>();
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
                    aggroUnit = null;
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
                if (isDeplaced != true)
                {
                    navAgent.stoppingDistance = baseStats.attackRange;

                    if (distance <= baseStats.aggroRange)
                    {
                        navAgent.SetDestination(aggroTarget.position);
                    }
                }
            }
        }
    }
}