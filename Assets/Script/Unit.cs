using UnityEngine;

namespace PS.Units
{
    [CreateAssetMenu(fileName = "New Unit", menuName="Units/New Unit")]
    public class Unit : ScriptableObject
    {
        // Définition des types d'unités disponibles.
        public enum unitType
        {
            Warrior,
            Shooter,
            Healer,
            Worker
        };
        
        [Header("Unit Settings")]
        [Space(15)]
        
        public unitType type; // Type de l'unité
        public new string name;// Nom de l'unité
        public GameObject unitPrefab; // Préfabriqué de l'unité pour instanciation.
        
        [Space(40)]
        [Header("Unit Stats")]
        [Space(15)]
        
        public int cost; // Coût de l'unité
        public int attack; // Attaque de l'unité
        public int attackRange; // Portée de l'attaque de l'unité
        public int health; // Santé de l'unité.
        public int armor; // Armure de l'unité.
    }
}

