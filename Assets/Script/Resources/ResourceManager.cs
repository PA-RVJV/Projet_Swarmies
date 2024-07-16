using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    public Dictionary<ResourceType, int> resourceStock = new Dictionary<ResourceType, int>();
    
    // Ressources de départ
    public int startingWood = 0;
    public int startingStone = 0;
    public int startingGold = 0;
    
    private void Awake()
    {
        InitializeResourceStock();
    }
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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
    
    public bool HasEnoughResources(Dictionary<ResourceType, int> cost)
    {
        foreach (var kvp in cost)
        {
            if (GetResourceAmount(kvp.Key) < kvp.Value)
            {
                return false;
            }
        }
        return true;
    }
    
    public bool DeductResources(Dictionary<ResourceType, int> cost)
    {
        if (HasEnoughResources(cost))
        {
            foreach (var kvp in cost)
            {
                resourceStock[kvp.Key] -= kvp.Value;
            }
            return true;
        }
        return false;
    }
}
