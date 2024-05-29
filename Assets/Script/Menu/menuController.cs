using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class menuController : MonoBehaviour
{
    
    // méthode pour quitter l'apli appeller lors d'un clic sur le bouton quiter du menu (gerer sur unity)
    public void Quit()
    {
        Application.Quit();
    }
    
    // méthode de changement de scène appeler lors d'un clic sur le bouton jouer du menu (gerer sur unity)
    public void SwitchScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
        Scene scene = SceneManager.GetSceneByName(sceneName);
        if (!SceneManager.SetActiveScene(scene))
        {
            throw new Exception("cant set active scene");
        }
    }
}   
