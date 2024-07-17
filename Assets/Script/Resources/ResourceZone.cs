using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ResourceType
{
    Wood,
    Stone,
    Gold
}

public class ResourceZone : MonoBehaviour
{
    public ResourceType resourceType;
    public int resourceAmount;

    public float collectionInterval = 5.0f;
    public int collectionAmount = 10;
    
    public int CollectResources(int amount)
    {
        int collected = Mathf.Min(amount, resourceAmount);
        resourceAmount -= collected;
        return collected;
    }
}


