using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceCamp : MonoBehaviour
{
    public float collectionInterval = 5.0f; // Intervalle de collecte en secondes
    public int collectionAmount = 10; // Quantité de ressources collectées à chaque intervalle
    private float collectionTimer = 0.0f;

    private ResourceType resourceType;
    private ResourceZone resourceZone; // Référence à la zone de ressources
    public ResourceOverlay resourceUI; // Référence à l'UI pour mettre à jour les ressources

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
            resourceUI.UpdateResource(resourceType, collected);
        }
    }

    public void SetResourceType(ResourceType typeResource)
    {
        resourceType = typeResource;
    }

    public void SetResourceZone(ResourceZone zoneResource)
    {
        resourceZone = zoneResource;
    }
}
