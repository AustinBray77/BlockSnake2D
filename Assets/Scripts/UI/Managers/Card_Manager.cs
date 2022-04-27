using System.Collections;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

//Class to manage the UI elements associated with a card
[RequireComponent(typeof(Button))]
public class Card_Manager : BaseBehaviour
{
    //Refrences to UI elements of the cards
    [SerializeField] private TMP_Text title;
    [SerializeField] private TMP_Text description;
    [SerializeField] private Image image;

    //Reference to the button on the card
    private Button btn;

    //Stores the description the card will have
    private string desc;

    //Method called on instantiation
    private void Awake()
    {
        //Gets the button from the current cam object
        btn = GetComponent<Button>();
    }

    //Method called to load a card data into UI
    public void DataFromCard(Card data)
    {
        //Sets the title to the title of the card + its level
        title.text = data.title.ToUpper() + " - LEVEL " + (data.level + 1).ToString();

        //Sets the card image from the card data
        description.text = data.GetDescription();
        image.sprite = data.image;

        //Sets the description variable from the card data
        desc = data.GetDescription();

        //Adds the card method to the button
        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(new UnityAction(data.Call));
    }

    //Method for when the user clicks to see the card info 
    public void ShowInfo_Click() => StartCoroutine(ShowInfo());

    //Coroutine to show the info screen for the card
    private IEnumerator ShowInfo()
    {
        //Instantiates the info object from the prefab
        GameObject infoObject = Instantiate(Reference.infoObject, Reference.canvas.transform);

        //Gets the text on the info object and sets the text to the description of the card plus continue statement
        TextMeshProUGUI text = infoObject.GetComponentInChildren<TextMeshProUGUI>();
        text.text = desc + "\n(Press anywhere to continue)";

        //Wait until the user clicks
        yield return new WaitUntil(() => Functions.UserIsClicking());

        //Destroys the object after the user clicks
        Destroy(infoObject);
    }
}
