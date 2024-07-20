using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CaserneDisplay : MonoBehaviour
{
    // Dictionnaire pour stocker les icônes chargées
    private Dictionary<string, Sprite> iconDictionary = new Dictionary<string, Sprite>();
    
    [SerializeField] private Image progressBarAmount;
    [SerializeField] private GameObject warningResourceObject;
    [SerializeField] private TextMeshProUGUI unitsInQueueText;
    [SerializeField] private Image unitIconImage;
    
    // Dossier contenant les icônes
    private string iconFolder = "IconUnit";
    
    private Camera _camera;
    
    // Start is called before the first frame update
    void Start()
    {
        LoadIcons();
        _camera = Camera.main;
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
}
