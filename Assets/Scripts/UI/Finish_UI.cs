using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Class to control the UI for each finish object
public class Finish_UI : UI
{
    //Property to store the prefab of the card
    [SerializeField] private GameObject cardPrefab;

    //Arrays to store the instantiated objects
    private GameObject[] cardObjects = new GameObject[3];

    //Method called to set the data of the cards
    public void SetCardData(Card[] cards)
    {
        //Returns if the card array is not long enough
        if (cards.Length < 3)
            return;

        //Loops through each object and assings it associated card data
        for (int i = 0; i < 3; i++)
        {
            cardObjects[i].GetComponent<Card_Manager>().DataFromCard(cards[i]);
        }
    }

    //Method called when the UI is to be shown
    public override void Show()
    {

        //Activates the UI Elements
        UIContainer.SetActive(true);

        //Loops for each card data object
        for (int i = 0; i < 3; i++)
        {
            //Instantiates the card prefab, sets the position of the card object, and sets its data to the current data 
            cardObjects[i] = Instantiate(cardPrefab, UIContainer.transform);
            cardObjects[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(630 - (i * 630), 0);
        }

        transform.parent.gameObject.GetComponent<GraphicRaycaster>().enabled = true;
    }

    //Method call when the UI is to be hidden
    public override void Hide()
    {
        //Destroys each of the current cardobjects
        foreach (GameObject cardObejct in cardObjects)
        {
            Destroy(cardObejct);
        }

        //Sets the player to not be at the finish and activates the Game UI
        Player.isAtFinish = false;
        Refrence.gameUI.gameObject.SetActive(true);

        transform.parent.gameObject.GetComponent<GraphicRaycaster>().enabled = false;

        //Decativates the gameobject hiding the UI
        UIContainer.SetActive(false);
    }
}
