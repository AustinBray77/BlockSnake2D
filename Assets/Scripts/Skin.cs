using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Serializable class used to contol the data for skins
[System.Serializable]
public class Skin
{
    //Properties to store the data of the skin
    public string title;
    public Sprite frontSprite, segmentSprite;
    public int levelRequirement, price, index;

    public bool locked =>
        levelRequirement > Serializer.Instance.activeData.level.level;

    public bool equipped =>
        index == Serializer.Instance.activeData.activeSkin;

    public bool purchased
    {
        get
        {
            Debug.Log($"Serializer Instance is null?{Serializer.Instance == null}");
            Debug.Log($"Active Data is null?{Serializer.Instance.activeData == null}");
            Debug.Log($"Purchased Skins is null?{Serializer.Instance.activeData.purchasedSkins == null}");
            Debug.Log($"Title is null?{title == null}");
            return Serializer.Instance.activeData.purchasedSkins.Contains(title);
        }
    }
}