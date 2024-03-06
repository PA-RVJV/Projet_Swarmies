using System.Collections;
using System.Collections.Generic;
using PS.InputHandlers;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

namespace PS.Units.Player
{
    // Assure que le GameObject auquel ce script est attaché possède un composant NavMeshAgent.
    // Le déplacement est assuré par le package AI Navigation 
    [RequireComponent(typeof(NavMeshAgent))]
    public class PlayerUnit : MonoBehaviour
    {
        // Variable privée pour stocker la référence au composant NavMeshAgent de l'unité.
        private NavMeshAgent navAgent;

        public UnitStatTypes.Base baseStats;

        public GameObject unitStatDisplay;
        
        public Image healthBarAmount;

        public float currentHealth;
            
        // OnEnable est appelé quand le script est activé.
        public void OnEnable()
        {
            // Initialise la référence au composant NavMeshAgent.
            navAgent = GetComponent<NavMeshAgent>();
        }

        private void Start()
        {
            currentHealth = baseStats.health;
        }
        private void Update()
        {
            HandleHealth();
        }
        
        // Méthode pour déplacer l'unité vers une destination spécifique.
        public void MoveUnit(Vector3 _destination)
        {
            // Utilise le NavMeshAgent pour définir la destination de l'unité.
            navAgent.SetDestination(_destination);
        }

        public void TakeDamage(float damage)
        {
            float totalDamage = damage - baseStats.armor;
            currentHealth -= totalDamage;
        }
        
        private void HandleHealth()
        {
            Camera camera = Camera.main;
            unitStatDisplay.transform.LookAt(unitStatDisplay.transform.position + 
                                             camera.transform.rotation * Vector3.forward, 
                                    camera.transform.rotation * Vector3.up);
            
            healthBarAmount.fillAmount = currentHealth / baseStats.health;

            if (currentHealth <= 0)
            {
                Die();
            }
        }

        private void Die()
        {
            InputHandlers.InputManager.instance.selectedUnits.Remove(gameObject.transform);
            Destroy(gameObject);
        }
    }
}