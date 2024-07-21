using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ResourceOverlay : MonoBehaviour
{
    public TextMeshProUGUI woodText;
    public TextMeshProUGUI stoneText;
    public TextMeshProUGUI goldText;

    private ResourceManager resourceManager;
    public GameObject resourceZoneErrorMessageImage;
    public GameObject resourceCostErrorMessageImage;

    private void Start()
    {
        resourceManager = FindObjectOfType<ResourceManager>();
        if (resourceManager == null)
        {
            Debug.LogError("ResourceManager not found in the scene.");
        }

        // Mise à jour initiale des ressources
        UpdateResourceUI();
    }

    public void UpdateResourceUI()
    {
        woodText.text = "" + resourceManager.GetResourceAmount(ResourceType.Wood);
        stoneText.text = "" + resourceManager.GetResourceAmount(ResourceType.Stone);
        goldText.text = "" + resourceManager.GetResourceAmount(ResourceType.Gold);
    }
    
    public IEnumerator ShowResourceZoneErrorMessage()
    {
        resourceZoneErrorMessageImage.SetActive(true);
        yield return new WaitForSeconds(2);
        resourceZoneErrorMessageImage.SetActive(false);
    }
    
    public IEnumerator ShowResourceCostErrorMessage()
    {
        resourceCostErrorMessageImage.SetActive(true);
        yield return new WaitForSeconds(2);
        resourceCostErrorMessageImage.SetActive(false);
    }
}
