using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using PS.InputHandlers; // Inclut l'espace de noms où se trouve InputManager pour y accéder.

namespace PS.Player
{
    // Déclare la classe PlayerManager comme un composant MonoBehaviour qui peut être attaché à un GameObject.
    public class PlayerManager : MonoBehaviour
    {
        // Déclare une variable statique 'instance' de type PlayerManager. Cela permet d'accéder à l'instance de PlayerManager
        // depuis d'autres classes, en utilisant le motif Singleton pour s'assurer qu'une seule instance de cette classe est utilisée.
        public static PlayerManager instance;

        // Variable publique pour stocker une référence au Transform qui contient les unités du joueur.
        public Transform playerUnits;

        // La méthode Start est appelée avant la première frame update. C'est ici que l'instance statique est initialisée.
        void Start()
        {
            instance = this; // Initialise l'instance statique avec cette instance du script.
        }
        
        // Update est appelée à chaque frame. C'est ici que les entrées utilisateur sont gérées.
        void Update()
        {
            // Appelle la méthode HandleUnitMovement de l'instance d'InputManager pour gérer les mouvements des unités basés sur les entrées utilisateur.
            // Cela suppose que InputManager est déjà instancié et accessible via son propre pattern Singleton.
            InputManager.instance.HandleUnitMovement();
        }
    }
}

