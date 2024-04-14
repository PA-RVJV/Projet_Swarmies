using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

using PS.Units;

namespace PS.Player
{
    public class GameOver : MonoBehaviour
    {
        private int playerUnitsCount;
    
        private int enemyUnitsCount;

        public PlayerManager PlayerManager;
        
        // Start is called before the first frame update
        void Start()
        {
            // lance CheckUnits a intervalle régulier
            InvokeRepeating("CheckUnits", 2.0f, 5.0f);
        }
        
        void  CheckUnits() 
        {
            playerUnitsCount = GameObject.FindGameObjectsWithTag("PlayerUnit").Length;
            enemyUnitsCount = GameObject.FindGameObjectsWithTag("EnemyUnit").Length;
            if (playerUnitsCount == 0)
            {
                // affiche Screen GameOver : défaite
                PlayerManager.GameOver("Game Over","Vos unités sont toutes anéantis",2);
            }
            else if (enemyUnitsCount == 0)
            {
                // affichje screen GameOver : Victoire
                PlayerManager.GameOver("Victoire", "Toutes les unités ennemies ont été détruites", 2);
            }
        }
    }
}


