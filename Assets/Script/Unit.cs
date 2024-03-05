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
            Healer
        };
        
        // Indique si l'unité appartient au joueur.
        public bool isPlayerUnit;

        // Type de l'unité.
        public unitType type;
        
        // Nom de l'unité.
        public new string name;

        // Préfabriqué de l'unité pour instanciation.
        public GameObject unitPrefab;

        // Coût de l'unité.
        public int cost;
        // Attaque de l'unité.
        public int attack;
        // Santé de l'unité.
        public int health;
        // Armure de l'unité.
        public int armor;
    
        // Méthode Start - Vide car ScriptableObject n'utilise pas Start ou Update.
        void Start()
        {
            
        }
    
        // Méthode Update - Vide car ScriptableObject n'utilise pas Start ou Update.
        void Update()
        {
        
        }
    }
}

