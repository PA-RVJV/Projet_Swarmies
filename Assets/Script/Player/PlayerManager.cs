using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using PS.InputHandlers;
using PS.Units;

namespace PS.Player
{
    // Déclare la classe PlayerManager comme un composant MonoBehaviour qui peut être attaché à un GameObject.
    public class PlayerManager : MonoBehaviour
    {
        public UnitHandler unitHandler;
        public InputManager inputManager;

        public GameOverScreen gameOverScreen;
        // Déclare une variable statique 'instance' de type PlayerManager

        // Variable publique pour stocker les transform contenant les unité amis et ennemies
        public Transform playerUnits;
        public Transform enemyUnits;

        public void GameOver(string title, string underTitle, int stat)
        {
            gameOverScreen.SetupEndGame(title, underTitle, stat);
        }
        private void Awake()
        {
            unitHandler.SetUnitStats(playerUnits);
            unitHandler.SetUnitStats(enemyUnits);
        }

        
        private void Update()
        {
            // Appelle la méthode HandleUnitMovement de l'instance d'InputManager pour gérer le contrôle des unités basés sur les entrées utilisateur.
            inputManager.HandleUnitMovement();
        }
    }
}

