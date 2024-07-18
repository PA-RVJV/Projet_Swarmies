using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CaserneDisplay : MonoBehaviour
{
    // Dictionnaire pour stocker les icônes chargées
    private Dictionary<string, Sprite> iconDictionary = new Dictionary<string, Sprite>();
    
    // Dossier contenant les icônes
    private string iconFolder = "IconUnit";
    
    private Camera _camera;
    private Image progressBarValueImage;
    
    private TextMeshProUGUI unitsInQueueText;
    private GameObject warningResourceImage;
    private Image unitIconImage;
    
    // Start is called before the first frame update
    void Start()
    {
        LoadIcons();
        
        _camera = Camera.main;
        
        // Trouver le GameObject ProductionBar dans la hiérarchie du Canvas
        Transform productionBarTransform = transform.Find("ProductionBar");
        
        // Trouver l'image Background dans ProductionBar
        Transform backgroundTransform = productionBarTransform.Find("Background");

        // Trouver l'image ProductionBarValue dans Background
        Transform productionBarValueTransform = backgroundTransform.Find("ProductionBarValue");

        // Obtenir le composant Image de ProductionBarValue
        progressBarValueImage = productionBarValueTransform.GetComponent<Image>();
        
        // Trouver le GameObject UnitQueue dans la hiérarchie du Canvas
        Transform UnitQueueTransform = transform.Find("UnitQueue");
        
        unitsInQueueText = UnitQueueTransform.GetComponentInChildren<TextMeshProUGUI>();
        unitIconImage = UnitQueueTransform.GetComponentInChildren<Image>();
        warningResourceImage = UnitQueueTransform.Find("WarningResourceImage").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        HandleDisplayRotation();
    }

    private void HandleDisplayRotation()
    {
        gameObject.transform.LookAt(gameObject.transform.position + 
                                    _camera.transform.rotation * Vector3.forward, 
            _camera.transform.rotation * Vector3.up);
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
        if (progressBarValueImage != null)
        {
            progress = Mathf.Clamp01(progress);
            progressBarValueImage.fillAmount = progress;
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
        warningResourceImage.SetActive(activate);
    }
}
