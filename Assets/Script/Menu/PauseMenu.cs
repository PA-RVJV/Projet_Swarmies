using System.Collections;
using System.Collections.Generic;
using System.Transactions;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.UIElements;
using Slider = UnityEngine.UI.Slider;
using Toggle = UnityEngine.UI.Toggle;

public class PauseMenu : MonoBehaviour
{
    public static bool isPause = false;
    public GameObject pauseMenuUI;
    public GameObject mainPauseMenu;
    public GameObject optionPauseMenu;
    public GameObject popoutLoadGameMenu;
    public GameObject popoutNoGameMenu;
    public GameObject soundGameMenu;
    public GameObject gameplayGameMenu;
    public GameObject graphicGameMenu;
    
    // TODO Lors de l'appuis sur echap : verification si on se trouve dans les popout option. SI c'est le cas : popout de confirmation si on doit appliquer ou annuler les modif en cours
    
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPause)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void ResumeGame()
    {
        // on remet à default le menu
        mainPauseMenu.SetActive(true);
        optionPauseMenu.SetActive(false);
        popoutLoadGameMenu.SetActive(false);
        popoutNoGameMenu.SetActive(false);
        soundGameMenu.SetActive(false);
        gameplayGameMenu.SetActive(false);
        graphicGameMenu.SetActive(false);
        
        // on desactive le menu Pause et on remet le temps en route
        pauseMenuUI.SetActive(false);
        isPause = false;
        Time.timeScale = 1f;
    }

    void PauseGame()
    {
        // on active le menu pause et on met le temps en pause
        pauseMenuUI.SetActive(true);
        isPause = true;
        Time.timeScale = 0f;
    }
    // Volume Setting
    [Header("Volume Settings")]
    [SerializeField] private TMP_Text volumeTextValue = null;
    [SerializeField] private Slider volumeSlider = null;
    [SerializeField] private float defaultVolume = 1.0f;

    [Header("Gameplay Settings")] 
    [SerializeField] private TMP_Text ControllerSenTextValue = null;
    [SerializeField] private Slider controllerSenSlider = null;
    [SerializeField] private int defaultSen = 5;
    public int mainControllerSen = 5;

    [Header("Toggle Settings")] 
    [SerializeField] private Toggle InvertYToggle = null;

    [Header("Graphics Settings")] 
    [SerializeField] private TMP_Dropdown qualityDropdown;
    [SerializeField] private Toggle fullScreenToggle;
        
    private int _qualityLevel;
    private bool _isFullScreen;
    
    [Header("Confirmation")]
    [SerializeField] private GameObject confirmationPrompt = null;
    
    // Level to load
    [Header("Levels To Load")]
    public string newGameLevel;
    public string menuScene;
    private string levelToLoad;
    [SerializeField] private GameObject noSavedGameDialog = null;
    
    // récupération des resolution
    void Start()
    {

    }
    
    // Méthode pour charger la scene de base
    public void NewGameDialogYes()
    {
        SceneManager.LoadScene(newGameLevel);
    }

    public void ReturnMenuButton()
    {
        SceneManager.LoadScene(menuScene);
    }
    // méthode pour charguer la scene sauvegarder si il y en a une
    public void LoadGameDialogYes()
    {
        if (PlayerPrefs.HasKey("SavedLevel"))
        {
            levelToLoad = PlayerPrefs.GetString("SavecLevel");
            SceneManager.LoadScene(levelToLoad);
        }
        else
        {
            noSavedGameDialog.SetActive(true);
        }
    }
    
    // méthode pour quitter l'application
    public void Quit()
    {
        Application.Quit();
    }

    // méthode pour set le volume en fonction du float définis par un slider
    public void SetVolume(float volume)
    {
        AudioListener.volume = volume;
        volumeTextValue.text = volume.ToString("0.0");
    }

    // appliquer les modification faite au paramètre de son
    public void VolumeApply()
    {
        PlayerPrefs.SetFloat("masterVolume", AudioListener.volume);
        StartCoroutine(ConfirmationBox());
    }

    // méthode pour set le sensibilité en fonction du float définis par un slider
    public void SetControllerSen(float sensitivity)
    {
        mainControllerSen = Mathf.RoundToInt(sensitivity);
        ControllerSenTextValue.text = sensitivity.ToString("0");
    }

    // appliquer les modification faite au paramètre de gameplay
    public void GameplayApply()
    {
        if (InvertYToggle.isOn)
        {
            PlayerPrefs.SetInt("masterInvertY", 1);
        }
        else
        {
            PlayerPrefs.SetInt("masterInvertY", 0);
        }
        
        PlayerPrefs.SetFloat("masterSen", mainControllerSen);
        StartCoroutine(ConfirmationBox());
    }

    // méthode pour set le paramètre fullscreen en fonction du bool définis par un toggle
    public void SetFullScreen(bool isFullscreen)
    {
        _isFullScreen = isFullscreen;
    }

    // méthode pour set le paramètre de qualité définis par un int dans un dropdown (liste de choix)
    public void SetQuality(int qualityIndex)
    {
        _qualityLevel = qualityIndex;
    }

    // applique les modification faite au paramètre graphique
    public void GraphicsApply()
    {
        PlayerPrefs.SetInt("masterQuality", _qualityLevel);
        QualitySettings.SetQualityLevel(_qualityLevel);

        PlayerPrefs.SetInt("masterFullscreen", (_isFullScreen ? 1 : 0));
        Screen.fullScreen = _isFullScreen;

        StartCoroutine(ConfirmationBox());
    }
    
    // permet de reset les paramètre par défault en fonction du menu d'option dans lequel on se trouve
    public void ResetButton(string MenuType)
    {
        if (MenuType == "Audio")
        {
            AudioListener.volume = defaultVolume;
            volumeSlider.value = defaultVolume;
            volumeTextValue.text = defaultVolume.ToString("0.0");
            VolumeApply();
        }

        if (MenuType == "Gameplay")
        {
            ControllerSenTextValue.text = defaultSen.ToString("0");
            controllerSenSlider.value = defaultSen;
            mainControllerSen = defaultSen;
            InvertYToggle.isOn = false;
            GameplayApply();
        }

        if (MenuType == "Graphics")
        {
            qualityDropdown.value = 1;
            QualitySettings.SetQualityLevel(1);

            fullScreenToggle.isOn = false;
            Screen.fullScreen = false;
            
            GraphicsApply();
        }
    }

    public IEnumerator ConfirmationBox()
    {
        confirmationPrompt.SetActive(true);
        yield return new WaitForSeconds(2);
        confirmationPrompt.SetActive(false);
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
