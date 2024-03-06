using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class menuController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    // méthode pour quitter l'apli appeller lors d'un clic sur le bouton quiter du menu (gerer sur unity)
    public void Quit()
    {
        Application.Quit();
    }
    
    // méthode de changement de scène appeler lors d'un clic sur le bouton jouer du menu (gerer sur unity)
    public void SwitchScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}   
