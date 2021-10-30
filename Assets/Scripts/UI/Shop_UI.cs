using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

//Class to contorl the shop UI
public class Shop_UI : UI
{
    //Stores the active skin cards and skin managers
    private List<GameObject> skinCards;
    private List<Skin_Manager> skin_Managers;

    //Stores valid skins, skin card prefab, and gear amount text
    [SerializeField] private Skin[] _skins;
    [SerializeField] private GameObject skinCardPrefab;
    [SerializeField] private TMP_Text gearText;

    //Stores a static refrence to valid skins and active skin card
    public static Skin[] skins;
    public static int activeSkinCard = 0;

    //Method called after the scene loads
    public void DelayedStart()
    {
        //Assigns the static refrence for skins and sets the gear text to the current gear count
        skins = _skins;
        gearText.text = Serializer.activeData.gearCount.ToString();
    }

    //Method called when the UI is to show
    public override void Show()
    {
        //Activates the UI Elements
        UIContainer.SetActive(true);

        //Assigns default values
        activeSkinCard = 0;
        skinCards = new List<GameObject>();
        skin_Managers = new List<Skin_Manager>();

        //Instantiates each skin card ath the correct location and adds its manager
        for(int i = 0; i < _skins.Length; i++)
        {
            skinCards.Add(CreateSkinCard(new Vector2(i * 625, 0), new Vector3(i == 0 ? 1 : 0.9f, i == 0 ? 1 : 0.9f), _skins[i]));
            skin_Managers.Add(skinCards[i].GetComponent<Skin_Manager>());
        }
    }

    //Method called when the UI is to hide
    public override void Hide()
    {
        //Destroys all the skin cards
        while(skinCards.Count > 0)
        {
            Destroy(skinCards[0]);
            skinCards.RemoveAt(0);
        }

        //Sets the gameobject to inactive, hiding it
        UIContainer.SetActive(false);
    }

    //Method called to move all the skin cards
    public void MoveCards(int direction)
    {
        //Increments the active skin card
        activeSkinCard += direction;

        //Moves each of the skin cards
        for(int i = 0; i < skinCards.Count; i++)
        {
            RectTransform rt = skinCards[i].GetComponent<RectTransform>();
            rt.LeanMove(new Vector3((i - activeSkinCard) * 625, 0), 1f);
            rt.LeanScale(new Vector3(i - activeSkinCard == 0 ? 1 : 0.9f, i - activeSkinCard == 0 ? 1 : 0.9f), 1f);
        }   
    }

    //Method to update the data on all of the skin cards
    public void UpdateAllCards()
    {
        //Updates gear text
        gearText.text = Serializer.activeData.gearCount.ToString();

        //Updates each skin card
        foreach (Skin_Manager sm in skin_Managers)
        {
            sm.UpdateData();
        }
    }

    //Method called to create a skin card
    private GameObject CreateSkinCard(Vector2 location, Vector3 scale, Skin skin)
    {
        //Instantiates skincard prefab
        GameObject skinCard = Instantiate(skinCardPrefab, transform);
        RectTransform rt = skinCard.GetComponent<RectTransform>();

        //Sets the location
        rt.anchoredPosition = location;
        rt.localScale = scale;

        //Sets the data
        skinCard.GetComponent<Skin_Manager>().FromSkinObject(skin);

        //Returns the new skin card
        return skinCard;
    }

    //Called when the user clicks to go to the start menu
    public void Click_StartMenu()
    {
        //Switches to the Start UI with fade
        StartCoroutine(ClickWithFade(
            () => {
                Refrence.startUI.Show();
                Hide();
            }, 1f));
    }
}
