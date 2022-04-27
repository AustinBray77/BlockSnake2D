using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

//Class to contorl the shop UI
public class Shop_UI : UI<Shop_UI>
{
    //Static variable storing the number of skins in the game, CHANGE EVERY TIME A SKIN IS ADDED
    public static int skinCount = 10;

    //Stores the active skin cards and skin managers
    private List<GameObject> skinCards;
    private List<Skin_Manager> skin_Managers;

    //Stores the prefab for skin cards and the daily gear image
    [SerializeField] private GameObject skinCardPrefab, dailyGearImage;

    //Stores references to the gear text and daily reward text
    [SerializeField] private TMP_Text gearText, dailyRewardText;

    //Stores a reference to the content group where the skin cards are put
    [SerializeField] private RectTransform contentGroup;

    //Stores the index active skin card
    public static int activeSkinCard = 0;

    //Method called after the scene loads
    public IEnumerator Start()
    {
        //Waits until the data is loaded from save
        yield return new WaitUntil(() => Serializer.Instance.activeData != null);

        //Assigns the static refrence for skins and sets the gear text to the current gear count
        UpdateGearText();
    }

    //Method called to update the gear text
    public void UpdateGearText()
    {
        //Sets gear text value to the gear count from the loaded save
        gearText.text = Serializer.Instance.activeData.gearCount.ToString();
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

        //Loops for each skin
        for (int i = 0; i < Gamemanager.Instance.Skins.Length; i++)
        {
            //Creates a skin card with the associated skin
            skinCards.Add(CreateSkinCard(new Vector2(i * 625, 0), new Vector3(0.9f, 0.9f), Gamemanager.Instance.Skins[i]));

            //Adds the skin card manager to the managers list
            skin_Managers.Add(skinCards[i].GetComponent<Skin_Manager>());
        }

        //Updates the daily reward UI
        UpdateDailyReward();
    }

    //Method called when the UI is to hide
    public override void Hide()
    {
        //Loops for each skin card
        while (skinCards.Count > 0)
        {
            //Destroys the card
            Destroy(skinCards[0]);

            //Removes it from the list
            skinCards.RemoveAt(0);
        }

        //Sets the gameobject to inactive, hiding it
        base.Hide();
    }

    //Method to update the data on all of the skin cards
    public void UpdateAllCards()
    {
        //Updates gear text
        UpdateGearText();

        //Loops for each skin card manager
        foreach (Skin_Manager sm in skin_Managers)
        {
            //Updates the UI on the manager
            sm.UpdateData();
        }
    }

    //Method called to create a skin card
    private GameObject CreateSkinCard(Vector2 location, Vector3 scale, Skin skin)
    {
        //Instantiates skincard prefab with this as its parent
        GameObject skinCard = Instantiate(skinCardPrefab, transform);

        //Gets the rect transfrom from the instantiation
        RectTransform rt = skinCard.GetComponent<RectTransform>();

        //Sets the location
        rt.anchoredPosition = location;

        //Sets the scale
        rt.localScale = scale;

        //Sets the parent to the content group
        rt.SetParent(contentGroup);

        //Sets the data
        skinCard.GetComponent<Skin_Manager>().FromSkinObject(skin);

        //Returns the new skin card
        return skinCard;
    }

    //Method alled when the user clicks to go to the start menu
    public void Click_StartMenu()
    {
        //Activates the following method with a fade
        StartCoroutine(ClickWithFade(
            () =>
            {
                //Shows the start UI
                Start_UI.Instance.Show();

                //Hides this UI
                Hide();
            },
            //Fades over fadetime time
            fadeTime));
    }

    //Method called when the user clicks to watch an ad for gears
    public void WatchAdForGears(int reward)
    {
        //Shows a rewarded ad then calls the callback method
        UnityAdsService.Instance.ShowRewardedAdThenCall(() =>
        {
            //Adds five gears to the players gear count
            Serializer.Instance.activeData.SetGearCount(Serializer.Instance.activeData.gearCount + 5);

            //Updates the gear text
            Shop_UI.Instance.UpdateGearText();
        });
    }

    //Method called when the user clicks to claim the daily reward
    public void ClaimDailyReward()
    {
        //Trigger if a day has passed since the reward was last claimed
        if (Functions.DaysSinceUnixFromMillis(Functions.CurrentMillisInTimeZone()) - Functions.DaysSinceUnixFromMillis(Serializer.Instance.activeData.lastRewardTime) > 0)
        {
            //Add rewarded gears to the player
            Serializer.Instance.activeData.SetGearCount(Serializer.Instance.activeData.gearCount + Serializer.Instance.activeData.lastReward);

            //Set last reward time to the current time
            Serializer.Instance.activeData.SetLastRewardTime(Functions.CurrentMillisInTimeZone());

            //Set last reward amount to the current amount + 1
            Serializer.Instance.activeData.SetLastReward(Serializer.Instance.activeData.lastReward + 1);

            //Updates UI elements
            UpdateGearText();
            UpdateDailyReward();

            //Saves the data
            Serializer.Instance.SaveData();
        }
    }

    //Method to update the daily reward UI
    private void UpdateDailyReward()
    {
        //Triggers if the daily reward is currently collectible
        if (Functions.DaysSinceUnixFromMillis(Functions.CurrentMillisInTimeZone()) -
            Functions.DaysSinceUnixFromMillis(Serializer.Instance.activeData.lastRewardTime) > 0)
        {
            //Sets the reward text to +(reward amount)
            dailyRewardText.text = "+" + Serializer.Instance.activeData.lastReward.ToString();

            //Activates the gear image
            dailyGearImage.SetActive(true);
        }
        else
        {
            //Tells the user the reward is available tomorrow
            dailyRewardText.text = "Available Tomorrow";

            //Changes the font size
            dailyRewardText.fontSize = 70;

            //Deactivates the gear image
            dailyGearImage.SetActive(false);
        }
    }
}