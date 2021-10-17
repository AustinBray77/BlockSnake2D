using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Skin_Manager : MonoBehaviour
{ 
    [SerializeField] private TextMeshProUGUI title, desc;
    [SerializeField] private Image frontImage, segmentImage;

    private Skin skin;

    private string lockedTitle = "LOCKED";
    [SerializeField] private Sprite lockedSprite;

    private string lockedDesc =>
        "UNLOCK AT LEVEL " + skin.levelRequirement;

    private int index = 0;

    public void UpdateData()
    {
        title.text = skin.locked ? lockedTitle : skin.title;

        desc.text = skin.locked ?
            lockedDesc : !skin.purchased ?
                skin.price.ToString() + " GEARS" : skin.equipped ?
                    "EQUIPPED" : "EQUIP";

        frontImage.sprite = skin.locked ? lockedSprite : skin.frontSprite;
        segmentImage.sprite = skin.locked ? lockedSprite : skin.segmentSprite;
    }

    public void FromSkinObject(Skin skin)
    {
        this.skin = skin;
        index = skin.index;

        UpdateData();
    }

    public void OnClick()
    {
        if(Shop_UI.activeSkinCard == index)
        {
            if (!skin.locked)
            {
                if (Serializer.activeData.gearCount >= skin.price && !skin.purchased)
                {
                    Serializer.activeData.PurchaseSkin(index, skin.price);
                }

                if (skin.purchased)
                {
                    Serializer.activeData.SetActiveSkin(index);
                    Main_UI.shopUI.UpdateAllCards();
                }
            }
        } else
        {
            Main_UI.shopUI.MoveCards(ExtraMath.NormalizeInt(index - Shop_UI.activeSkinCard));
        }
    }
}
