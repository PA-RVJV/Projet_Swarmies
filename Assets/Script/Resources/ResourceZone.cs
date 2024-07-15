using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ResourceType
{
    Wood,
    Stone,
    //Iron,
    //Or
}

public class ResourceZone : MonoBehaviour
{
    public ResourceType resourceType;
    public int resourceAmount;
    
    public int CollectResources(int amount)
    {
        int collected = Mathf.Min(amount, resourceAmount);
        resourceAmount -= collected;
        return collected;
    }
}


