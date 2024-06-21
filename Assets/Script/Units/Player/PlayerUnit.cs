using UnityEngine;
using UnityEngine.AI;


namespace PS.Units.Player
{
    // Le déplacement est assuré par le package AI Navigation 
    [RequireComponent(typeof(NavMeshAgent))]
    public class PlayerUnit : MonoBehaviour
    {
        public UnitHandler unitHandler;
        public UnitStatTypes.Base baseStats;
        public float attackCooldown;
        
        // Variable privée pour stocker la référence au composant NavMeshAgent de l'unité.
        private NavMeshAgent _navAgent;
        private Collider[] _rangeColliders = {};
        private Transform _aggroTarget;
        private UnitStatDisplay _aggroUnit;
        
        private bool _hasAggro;
        private bool _isDeplaced;
        private float _distance;

        
        // OnEnable est appelé quand le script est activé.
        private void OnEnable()
        {
            // Initialise la référence au composant NavMeshAgent.
            _navAgent = GetComponent<NavMeshAgent>();
            attackCooldown = baseStats.attackCooldown;
        }

        private void OnDestroy()
        {
            Debug.Log("destroyed");
        }
        
        // Méthode pour déplacer l'unité vers une destination spécifique.
        public void MoveUnit(Vector3 destination)
        {
            // Utilise le NavMeshAgent pour définir la destination de l'unité.
            _isDeplaced = true;
            _navAgent.SetDestination(destination);
        }
        private void Update()
        {
            if (!_hasAggro)
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

            if (_isDeplaced == true)
            {
                if (_navAgent.remainingDistance <= _navAgent.stoppingDistance) // Vérifie si l'agent est assez proche de la destination
                {
                    if (!_navAgent.hasPath || _navAgent.velocity.sqrMagnitude == 0f) // Vérifie si l'agent est arrêté
                    {
                        _isDeplaced = false;
                    }
                }
            }

            if (attackCooldown <= -0.3)
            {
                attackCooldown = baseStats.attackCooldown;
            }
        }

        private void CheckForEnemyTarget()
        {
            Physics.OverlapSphereNonAlloc(transform.position, baseStats.aggroRange, _rangeColliders);

            for (int i = 0; i < _rangeColliders.Length; i++)
            {
                if (_rangeColliders[i]?.gameObject.layer == unitHandler.eUnitLayer)
                {
                    _aggroTarget = _rangeColliders[i].gameObject.transform;
                    _aggroUnit = _aggroTarget.gameObject.GetComponentInChildren<UnitStatDisplay>();
                    _hasAggro = true;
                    break; 
                }
            }
        }
        
        private void CheckAggroDistance()
        {
            if (_aggroTarget != null)
            {
                _distance = Vector3.Distance(_aggroTarget.position, transform.position);
                if (_distance >= baseStats.attackRange)
                {
                    _hasAggro = false;
                    _aggroTarget = null;
                    _aggroUnit = null;
                }
            }
        }


        private void Attack()
        {
            if (_distance < baseStats.attackRange && attackCooldown <= 0)
            {
                attackCooldown = baseStats.attackCooldown;
                _aggroUnit.TakeDamage(baseStats.attack);
            }
        }

        private void MoveToAggroTarget()
        {
            if (_aggroTarget == null)
            {
                _navAgent.SetDestination(transform.position);
                _hasAggro = false;
            }
            else
            {
                _distance = Vector3.Distance(_aggroTarget.position, transform.position);
                if (_isDeplaced != true)
                {
                    _navAgent.stoppingDistance = baseStats.attackRange;

                    if (_distance <= baseStats.aggroRange)
                    {
                        _navAgent.SetDestination(_aggroTarget.position);
                    }
                }
            }
        }
    }
}