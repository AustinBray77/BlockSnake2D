using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Serializable class used to contol the data for skins
[System.Serializable]
public class Skin
{
    //Stores the title of the skin
    public string title;

    //Stores the front and segment sprites of the skin
    public Sprite frontSprite, segmentSprite;

    //Stores the level unlock requirement, price, and index of the skin
    public int levelRequirement, price, index;

    //Gets if the skin is locked
    public bool locked =>
        levelRequirement > Serializer.Instance.activeData.level.level;

    //Gets if the skin is equipped
    public bool equipped =>
        index == Serializer.Instance.activeData.activeSkin;

    //Gets if the skin is purchased
    public bool purchased =>
        Serializer.Instance.activeData.purchasedSkins.Contains(title);

}