using System.Collections.Generic;
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
            Worker,
            Mairie,
            Porte,
            Entrepot,
        };
        
        [Header("Unit Settings")] [Space(15)]
        
        public unitType type; // Type de l'unité
        public new string name;// Nom de l'unité
        public GameObject unitPrefab; // Préfabriqué de l'unité pour instanciation.

        
        [Space(40)] [Header("Unit Stats")] [Space(15)]
        
        public UnitStatTypes.Base baseStats;
        
        public Dictionary<ResourceType, int> GetCost()
        {
            return new Dictionary<ResourceType, int>
            {
                { ResourceType.Wood, Mathf.RoundToInt(baseStats.woodCost) },
                { ResourceType.Stone, Mathf.RoundToInt(baseStats.stoneCost) },
                { ResourceType.Gold, Mathf.RoundToInt(baseStats.goldCost) }
            };
        }
    }
}

