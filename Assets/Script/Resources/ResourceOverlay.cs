using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResourceOverlay : MonoBehaviour
{
    public TextMeshProUGUI woodText;
    public TextMeshProUGUI stoneText;

    private int woodAmount;
    private int stoneAmount;

    public void UpdateResource(ResourceType type, int amount)
    {
        switch (type)
        {
            case ResourceType.Wood:
                woodAmount += amount;
                woodText.text = "" + woodAmount;
                break;
            case ResourceType.Stone:
                stoneAmount += amount;
                stoneText.text = "" + stoneAmount;
                break;
        }
    }
}
