using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LoadPrefs : MonoBehaviour
{
    [Header("General Setting")] 
    [SerializeField] private bool canUse = false;
    [SerializeField] private menuController menuController;

    [Header("Volume Setting")] 
    [SerializeField] private TMP_Text volumeTextValue = null;
    [SerializeField] private Slider volumeSlider = null;
    
    [Header("Quality Level Setting")] 
    [SerializeField] private TMP_Dropdown qualityDropdown;
    
    [Header("Fullscreen Setting")]
    [SerializeField] private Toggle fullScreenToggle;
    
    [Header("Sensitivity Setting")]
    [SerializeField] private TMP_Text ControllerSenTextValue = null;
    [SerializeField] private Slider controllerSenSlider = null;

    [Header("Invert Y Setting")]
    [SerializeField] private Toggle InvertYToggle = null;

    private void Awake()
    {
        if (canUse)
        {
            if (PlayerPrefs.HasKey("masterVolume"))
            {
                float localVolume = PlayerPrefs.GetFloat("masterVolume");
                
                volumeTextValue.text = localVolume.ToString("0.0");
                volumeSlider.value = localVolume;
                AudioListener.volume = localVolume;
            }
            else
            {
                menuController.ResetButton("Audio");
            }

            if (PlayerPrefs.HasKey("masterQuality"))
            {
                int localQuality = PlayerPrefs.GetInt("masterQuality");
                qualityDropdown.value = localQuality;
                QualitySettings.SetQualityLevel(localQuality);
            }

            if (PlayerPrefs.HasKey("masterFullscreen"))
            {
                int localFullscreen = PlayerPrefs.GetInt("masterFullscreen");

                if (localFullscreen == 1)
                {
                    Screen.fullScreen = true;
                    fullScreenToggle.isOn = true;
                }
                else
                {
                    Screen.fullScreen = false;
                    fullScreenToggle.isOn = false;
                }
            }

            if (PlayerPrefs.HasKey("masterSen"))
            {
                float localSensitivity = PlayerPrefs.GetFloat("masterSen");

                ControllerSenTextValue.text = localSensitivity.ToString("0");
                controllerSenSlider.value = localSensitivity;
                menuController.mainControllerSen = Mathf.RoundToInt(localSensitivity);
            }

            if (PlayerPrefs.HasKey("masterInvert"))
            {
                if (PlayerPrefs.GetInt("masterInvertY") == 1)
                {
                    InvertYToggle.isOn = true;
                }
                else
                {
                    InvertYToggle.isOn = false;
                }
            }
        }
        
    }
}
