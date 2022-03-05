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
        levelRequirement > Serializer.activeData.level.level;

    public bool equipped =>
        index == Serializer.activeData.activeSkin;

    public bool purchased =>
        Serializer.activeData.purchasedSkins.Contains(title);
}