using UnityEngine;

namespace PS.Units
{
    [CreateAssetMenu(fileName = "New Unit", menuName="New Unit")]
    public class Unit : ScriptableObject
    {
        // class unité
        public enum unitType
        {
            Warrior,
            Shooter,
            Healer
        };
        
        public bool isPlayerUnit;

        public unitType type;
        
        public new string name;

        public GameObject unitPrefab;

        public int cost;
        public int attack;
        public int health;
        public int armor;
    
        void Start()
        {
            
        }

        private void OnDestroy()
        {
            // on supprime l'unité de la liste des unité active lors de sa destruction
            //unitSelectionManager.Instance.allUnitsList.Remove(gameObject);
        }
    
        void Update()
        {
        
        }
    }
}

