using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using PS.Player;
using PS.Units;

public class GameOverScreen : MonoBehaviour
{
    public TextMeshProUGUI titleEndText;
    public TextMeshProUGUI underTitleEndText;
    public TextMeshProUGUI statsEndGame;
    
    public void SetupEndGame(string title, string underTitle, int stat)
    {
        gameObject.SetActive(true);
        Time.timeScale = 0f;
        statsEndGame.text = stat.ToString() + " unités ennemies éliminé";
        titleEndText.text = title;
        underTitleEndText.text = underTitle;
    }
    
    // permet de reset le jeu ou revenir au menu suivant le bouton cliquer sur écran gameOver
    public void EndGame(string sceneName)
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneName);
    }
}
