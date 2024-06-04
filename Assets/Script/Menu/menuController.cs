using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class menuController : MonoBehaviour
{
    // Volume Setting
    [Header("Volume Setting")]
    [SerializeField] private TMP_Text volumeTextValue = null;
    [SerializeField] private Slider volumeSlider = null;
    
    [Header("Confirmation")]
    [SerializeField] private GameObject confirmationPrompt = null;
    
    // Level to load
    [Header("Levels To Load")]
    public string newGameLevel;
    private string levelToLoad;
    [SerializeField] private GameObject noSavedGameDialog = null;
    
    // Méthode pour charcher la scene de base
    public void NewGameDialogYes()
    {
        SceneManager.LoadScene(newGameLevel);
    }
    
    // méthode pour chaquer la scene sauvegarder si il y en a une
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
    
    // méthode pour quitter l'apli appeller lors d'un clic sur le bouton quiter du menu (gerer sur unity)
    public void Quit()
    {
        Application.Quit();
    }

    public void SetVolume(float volume)
    {
        AudioListener.volume = volume;
        volumeTextValue.text = volume.ToString("0.0");
    }

    public void VolumeApply()
    {
        PlayerPrefs.SetFloat("masterVolume", AudioListener.volume);
        StartCoroutine(ConfirmationBox());
    }

    public IEnumerator ConfirmationBox()
    {
        confirmationPrompt.SetActive(true);
        yield return new WaitForSeconds(2);
        confirmationPrompt.SetActive(false);
    }
}   
