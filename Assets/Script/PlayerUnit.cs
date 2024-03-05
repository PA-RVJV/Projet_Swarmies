using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace PS.Player
{
    // Assure que le GameObject auquel ce script est attaché possède un composant NavMeshAgent.
    // Le déplacement est assuré par le package AI Navigation 
    [RequireComponent(typeof(NavMeshAgent))]
    public class PlayerUnit : MonoBehaviour
    {
        // Variable privée pour stocker la référence au composant NavMeshAgent de l'unité.
        private NavMeshAgent navAgent;

        // OnEnable est appelé quand le script est activé.
        public void OnEnable()
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
    }
}