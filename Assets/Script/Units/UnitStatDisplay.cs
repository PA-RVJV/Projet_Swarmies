using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PS.Units
{
    public class UnitStatDisplay : MonoBehaviour
    {
        // Dictionnaire pour stocker les icônes chargées
        private Dictionary<string, Sprite> iconDictionary = new Dictionary<string, Sprite>();
        
        [SerializeField] private Image healthBarAmount;
        [SerializeField] private Image progressBarAmount;
        [SerializeField] private GameObject warningResourceObject;
        [SerializeField] private TextMeshProUGUI unitsInQueueText;
        [SerializeField] private Image unitIconImage;
        [SerializeField] private GameObject ProductionDisplay;
        [SerializeField] private Image PausePlayImage;
        [SerializeField] private Sprite pauseSprite;
        [SerializeField] private Sprite playSprite;
    
        private Camera _camera;
        
        public float maxHealth;
        
        // Dossier contenant les icônes
        private string iconFolder = "IconButton";

        private void Start()
        {
            _camera = Camera.main;
            LoadIcons();
            
            try
            {
                var playerUnit = gameObject.GetComponentInParent<Player.PlayerUnit>();
                if (playerUnit != null)
                {
                    maxHealth = playerUnit.baseStats.health;
                }
                else if(gameObject.GetComponentInParent<Enemy.EnemyUnit>())
                {
                    var enemyUnit = gameObject.GetComponentInParent<Enemy.EnemyUnit>();
                    if (enemyUnit != null)
                    {
                        maxHealth = enemyUnit.baseStats.health;
                    }
                }
                else
                {
                    var buildingUnit = gameObject.GetComponentInParent<Building>();
                    if (buildingUnit != null)
                    {
                        maxHealth = buildingUnit.baseStats.health;
                    }
                }
            }
            catch (Exception)
            {
                Debug.Log("No Unit Scripts found!");
            }
        }
        
        private void Update()
        {
            HandleDisplayRotation();
        }
        
        public void HandleHealth(float currentHealth)
        {
            healthBarAmount.fillAmount = currentHealth / maxHealth;
        }

        private void HandleDisplayRotation()
        {
            Quaternion rotationCam = _camera.transform.rotation;
            gameObject.transform.LookAt(gameObject.transform.position + rotationCam * Vector3.forward, 
                rotationCam * Vector3.up);
        }
            
        public void UpdateUnitsInQueueText(int currentCount, int numberMax)
        {
    
            if (unitsInQueueText != null)
            {
                unitsInQueueText.text = $"{currentCount}/{numberMax}";
            }
        }
        
        public void UpdateProgressBar(float progress)
        {
            if (progressBarAmount != null)
            {
                progress = Mathf.Clamp01(progress);
                progressBarAmount.fillAmount = progress;
            }
        }
    
        // Méthode pour charger les icônes
        void LoadIcons()
        {
            // Charge toutes les icônes du dossier spécifié
            Sprite[] icons = Resources.LoadAll<Sprite>(iconFolder);
    
            // Ajoute chaque icône au dictionnaire
            foreach (Sprite icon in icons)
            {
                iconDictionary.Add(icon.name, icon);
            }
        }
    
        // Méthode pour changer l'icône en fonction du type d'unité
        public void ChangeUnitIcon(string unitType)
        {
            if (iconDictionary.ContainsKey(unitType))
            {
                unitIconImage.sprite = iconDictionary[unitType];
            }
        }
    
        public void ShowResourceWarning(bool activate)
        {
            warningResourceObject.SetActive(activate);
        }

        public void ShowProductionDisplay(bool activate)
        {
            ProductionDisplay.SetActive(activate);
        }
        
        // Méthode pour mettre à jour l'icône du bouton de pause/play
        public void UpdatePausePlayButton(bool isPaused)
        {
            PausePlayImage.sprite = isPaused ? playSprite : pauseSprite;
        }
    }

}
