using System.Collections;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

//Class to manage the UI elements associated with a card
[RequireComponent(typeof(Button))]
public class Card_Manager : MonoBehaviour
{
    //Refrences to UI elements of the cards
    [SerializeField] private TMP_Text title;
    [SerializeField] private TMP_Text description;
    [SerializeField] private Image image;

    private Button btn;
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
        //Assigns the values for the UI elements
        title.text = data.title.ToUpper() + " - LEVEL " + (data.level + 1).ToString();
        description.text = data.GetDescription();
        image.sprite = data.image;
        desc = data.desc;

        //Assigns the listener to the button
        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(new UnityAction(data.Call));
    }

    public void ShowInfo_Click() => StartCoroutine(ShowInfo());

    private IEnumerator ShowInfo()
    {
        GameObject infoObject = Instantiate(Reference.infoObject, Reference.canvas.transform);

        TextMeshProUGUI text = infoObject.GetComponentInChildren<TextMeshProUGUI>();
        text.text = desc + "\n(Press anywhere to continue)";

        yield return new WaitUntil(() => Functions.UserIsClicking());

        Destroy(infoObject);
    }
}
