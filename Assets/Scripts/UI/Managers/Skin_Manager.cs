using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

//Class to control the UI of each skin
public class Skin_Manager : BaseBehaviour
{
    //Refrences to UI elements
    [SerializeField] private TextMeshProUGUI title, desc;
    [SerializeField] private Image frontImage, segmentImage, gearImage;

    //Refrence to the associated skin
    private Skin skin;
    private string lockedTitle = "LOCKED";
    [SerializeField] private Sprite lockedSprite;
    private string lockedDesc =>
        "UNLOCK AT LEVEL " + skin.levelRequirement;
    private int index = 0;

    //Method called when the UI is to update
    public void UpdateData()
    {
        //Assigns the title text
        title.text = skin.locked ? lockedTitle : skin.title;

        //Resizes the title if the length is too long
        if (title.text.Length >= 7)
        {
            title.fontSize = 90;
        }
        else
        {
            title.fontSize = 128;
        }

        //Gets the description and assigns it
        desc.text = skin.locked ?
            lockedDesc : !skin.purchased ?
                skin.price.ToString() : skin.equipped ?
                    "EQUIPPED" : "EQUIP";

        gearImage.gameObject.SetActive(!skin.locked && !skin.purchased);

        //Assigns the two sprites
        frontImage.sprite = skin.locked ? lockedSprite : skin.frontSprite;
        segmentImage.sprite = skin.locked ? lockedSprite : skin.segmentSprite;
    }

    //Method called to assign the skin data associated with this object
    public void FromSkinObject(Skin skin)
    {
        //Assigns the skin refrence
        this.skin = skin;
        index = skin.index;

        //Updates the UI
        UpdateData();
    }

    //Method called when the users clicks on the skin card
    public void OnClick()
    {
        //Triggers if this card is the current card in the middle
        if (Shop_UI.activeSkinCard == index)
        {
            //Triggers if the skin has been unlocked
            if (!skin.locked)
            {
                //Triggers if the skin hasn't been purchased and is purchasable
                if (Serializer.Instance.activeData.gearCount >= skin.price && !skin.purchased)
                {
                    //Purchases the skin
                    Serializer.Instance.activeData.PurchaseSkin(index, skin.price);
                }

                //Triggers if the user has purchased the skin
                if (skin.purchased)
                {
                    //Equips the skin and updates all of the other skin cards
                    Serializer.Instance.activeData.SetActiveSkin(index);
                    Shop_UI.Instance.UpdateAllCards();
                }
            }
        }
        //Else animate so that this card is in the middle
        else
        {
            Shop_UI.activeSkinCard = index;
        }
    }
}
