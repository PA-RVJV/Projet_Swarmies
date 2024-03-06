using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using PS.InputHandlers;

namespace PS.Player
{
    // Déclare la classe PlayerManager comme un composant MonoBehaviour qui peut être attaché à un GameObject.
    public class PlayerManager : MonoBehaviour
    {
        // Déclare une variable statique 'instance' de type PlayerManager
        public static PlayerManager instance;

        // Variable publique pour stocker les transform contenant les unité amis et ennemies
        public Transform playerUnits;
        public Transform enemyUnits;

        private void Awake()
        {
            instance = this;
            Units.UnitHandler.instance.SetUnitStats(playerUnits);
            Units.UnitHandler.instance.SetUnitStats(enemyUnits);
        }
        
        private void Start()
        {
            
        }
        
        private void Update()
        {
            // Appelle la méthode HandleUnitMovement de l'instance d'InputManager pour gérer le contrôle des unités basés sur les entrées utilisateur.
            InputManager.instance.HandleUnitMovement();
        }
    }
}

