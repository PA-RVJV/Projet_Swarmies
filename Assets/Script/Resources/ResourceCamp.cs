using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceCamp : MonoBehaviour
{
    public float collectionInterval; // Intervalle de collecte en secondes
    public int collectionAmount; // Quantité de ressources collectées à chaque intervalle
    private float collectionTimer = 0.0f;

    private ResourceType resourceType;
    private ResourceZone resourceZone; // Référence à la zone de ressources
    public ResourceManager resourceManager; // Référence à l'UI pour mettre à jour les ressources

    private void Start()
    {
        if (resourceZone != null)
        {
            // Récupére les valeurs spécifiques à la zone de ressources
            collectionInterval = resourceZone.collectionInterval;
            collectionAmount = resourceZone.collectionAmount;
        }
    }
    
    private void Update()
    {
        collectionTimer += Time.deltaTime;
        if (collectionTimer >= collectionInterval)
        {
            CollectResources();
            collectionTimer = 0.0f;
        }
    }

    private void CollectResources()
    {
        if (resourceZone != null && resourceZone.resourceType == resourceType)
        {
            int collected = resourceZone.CollectResources(collectionAmount);
            resourceManager.AddResource(resourceType, collected);
        }
    }

    public void SetResourceType(ResourceType typeResource)
    {
        resourceType = typeResource;
    }

    public void SetResourceZone(ResourceZone zoneResource)
    {
        resourceZone = zoneResource;
        
        // Récupérer les valeurs spécifiques à la zone de ressources
        collectionInterval = resourceZone.collectionInterval;
        collectionAmount = resourceZone.collectionAmount;
    }
}
