using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PS.Units;

public class ResourceManager : MonoBehaviour
{
    public Dictionary<ResourceType, int> resourceStock = new Dictionary<ResourceType, int>();
    public Canvas resourceOverlay;
    private ResourceOverlay resourceOverlayScript;
    
    
    // Ressources de départ
    public int startingWood = 0;
    public int startingStone = 0;
    public int startingGold = 0;
    
    private void Awake()
    {
        
        
    }
    
    // Start is called before the first frame update
    void Start()
    {
        resourceOverlayScript = resourceOverlay.GetComponent<ResourceOverlay>();
        InitializeResourceStock();
        //UpdateResourceUI();
    }
    
    private void InitializeResourceStock()
    {
        // Initialiser les stocks de ressources
        foreach (ResourceType type in System.Enum.GetValues(typeof(ResourceType)))
        {
            resourceStock[type] = 0;
        }

        // Ajouter les ressources de départ
        AddResource(ResourceType.Wood, startingWood);
        AddResource(ResourceType.Stone, startingStone);
        AddResource(ResourceType.Gold, startingGold);
    }
    
    public void AddResource(ResourceType type, int amount)
    {
        if (resourceStock.ContainsKey(type))
        {
            resourceStock[type] += amount;
            UpdateResourceUI();
            // on peu déclencher un événement ici pour notifier le joueur de la mise à jour
        }
    }
    
    public int GetResourceAmount(ResourceType type)
    {
        if (resourceStock.TryGetValue(type, out var amount))
        {
            return amount;
        }
        return 0;
    }
    
    public bool HasEnoughResourcesForBuilding(string unitName)
    {
        Dictionary<ResourceType, int> cost = GetConstructionCost(unitName);
        foreach (var kvp in cost)
        {
            if (GetResourceAmount(kvp.Key) < kvp.Value)
            {
                StartCoroutine(resourceOverlayScript.ShowResourceCostErrorMessage());
                return false;
            }
        }

        DeductResources(cost);
        return true;
    }
    
    public bool HasEnoughResourcesForUnit(string unitName)
    {
        Dictionary<ResourceType, int> cost = GetConstructionCost(unitName);
        foreach (var kvp in cost)
        {
            if (GetResourceAmount(kvp.Key) < kvp.Value)
            {
                return false;
            }
        }

        DeductResources(cost);
        return true;
    }
    
    private void DeductResources(Dictionary<ResourceType, int> cost)
    {
        foreach (var kvp in cost)
        {
            resourceStock[kvp.Key] -= kvp.Value;
        }
        UpdateResourceUI();
    }

    public Dictionary<ResourceType, int> GetConstructionCost(string unitName)
    {
        // Charger les statistiques de l'entrepôt à partir des assets
        Unit unit = Resources.Load<Unit>($"Units/{unitName}");
                        
        // Obtenir les coûts de construction
        Dictionary<ResourceType, int> constructionCost = unit.GetCost();

        return constructionCost;
    }
    
    private void UpdateResourceUI()
    {
        resourceOverlayScript.UpdateResourceUI();
    }
}
