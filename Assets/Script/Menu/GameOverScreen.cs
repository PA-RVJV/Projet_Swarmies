using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;


using PS.Player;
using PS.Units;

public class GameOverScreen : MonoBehaviour
{
    public GameObject gameOverTitleEndText;
    public GameObject gameOverUnderTitleEndText;
    
    public GameObject victoryTitleEndText;
    public GameObject victoryUnderTitleEndText;
    
    public TextMeshProUGUI statsEndGame;

    public void SetupEndGame(bool isGameOver, int stat)
    {
        gameObject.SetActive(true);
        Time.timeScale = 0f;
        if (isGameOver)
        {
            gameOverTitleEndText.SetActive(false);
            gameOverUnderTitleEndText.SetActive(false);
            victoryTitleEndText.SetActive(true);
            victoryUnderTitleEndText.SetActive(true);
        }
        else
        {
            gameOverTitleEndText.SetActive(true);
            gameOverUnderTitleEndText.SetActive(true);
            victoryTitleEndText.SetActive(false);
            victoryUnderTitleEndText.SetActive(false); 
        }
        statsEndGame.text = stat.ToString() + " " + statsEndGame.text;
        
    }
    
    // permet de reset le jeu ou revenir au menu suivant le bouton cliquer sur écran gameOver
    public void EndGame(string sceneName)
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneName);
    }
}
