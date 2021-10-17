using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Skin
{
    public string title;
    public Sprite frontSprite, segmentSprite;
    public int levelRequirement, price, index;

    public bool locked =>
        levelRequirement > Serializer.activeData.level.level;

    public bool equipped =>
        index == Serializer.activeData.activeSkin;

    public bool purchased =>
        Serializer.activeData.purchasedSkins[index];
}