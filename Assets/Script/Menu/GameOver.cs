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
        
        private int StartEnemyUnitsCount;

        public PlayerManager PlayerManager;
        
        // Start is called before the first frame update
        void Start()
        {
            // lance CheckUnits a intervalle régulier
            InvokeRepeating("CheckUnits", 2.0f, 5.0f);
            StartEnemyUnitsCount = GameObject.FindGameObjectsWithTag("EnemyUnit").Length;
        }
        
        void CheckUnits()
        {
            playerUnitsCount = GameObject.FindGameObjectsWithTag("PlayerUnit").Length;
            enemyUnitsCount = GameObject.FindGameObjectsWithTag("EnemyUnit").Length;
            if (playerUnitsCount == 0)
            {
                // affiche Screen GameOver : défaite
                PlayerManager.GameOver(false,StartEnemyUnitsCount);
            }
            else if (enemyUnitsCount == 0)
            {
                // affichje screen GameOver : Victoire
                PlayerManager.GameOver(true, StartEnemyUnitsCount);
            }
        }
    }
}


