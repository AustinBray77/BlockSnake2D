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

    //Stores valid skins, skin card prefab, and gear amount text
    //public Skin[] Skins;
    [SerializeField] private GameObject skinCardPrefab, dailyGearImage;
    [SerializeField] private TMP_Text gearText, dailyRewardText;
    [SerializeField] private RectTransform contentGroup;

    //Stores a static refrence to valid skins and active skin card
    public static int activeSkinCard = 0;

    //Method called after the scene loads
    public IEnumerator Start()
    {
        yield return new WaitUntil(() => Serializer.Instance.activeData != null);

        //Assigns the static refrence for skins and sets the gear text to the current gear count
        SetGearText();
    }

    //Sets the text after the serializer is assigned
    public void SetGearText()
    {
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

        //Instantiates each skin card ath the correct location and adds its manager
        for (int i = 0; i < Gamemanager.Instance.Skins.Length; i++)
        {
            skinCards.Add(CreateSkinCard(new Vector2(i * 625, 0), new Vector3(0.9f, 0.9f), Gamemanager.Instance.Skins[i]));
            skin_Managers.Add(skinCards[i].GetComponent<Skin_Manager>());
        }

        UpdateDailyReward();
    }

    //Method called when the UI is to hide
    public override void Hide()
    {
        //Destroys all the skin cards
        while (skinCards.Count > 0)
        {
            Destroy(skinCards[0]);
            skinCards.RemoveAt(0);
        }

        //Sets the gameobject to inactive, hiding it
        base.Hide();
    }

    //Method to update the data on all of the skin cards
    public void UpdateAllCards()
    {
        //Updates gear text
        gearText.text = Serializer.Instance.activeData.gearCount.ToString();

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
        rt.SetParent(contentGroup);

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
            () =>
            {
                Start_UI.Instance.Show();
                Hide();
            }, fadeTime));
    }

    //Method called when the user clicks to watch an ad for gears
    public void WatchAdForGears(int reward)
    {
        UnityAdsService.Instance.ShowRewardedAdThenCall(() =>
        {
            Serializer.Instance.activeData.SetGearCount(Serializer.Instance.activeData.gearCount + 5);
            Shop_UI.Instance.SetGearText();
        });
    }

    public void ClaimDailyReward()
    {
        if (Functions.DaysSinceUnixFromMillis(Functions.CurrentMillisInTimeZone()) - Functions.DaysSinceUnixFromMillis(Serializer.Instance.activeData.lastRewardTime) > 0)
        {
            Serializer.Instance.activeData.SetGearCount(Serializer.Instance.activeData.gearCount + Serializer.Instance.activeData.lastReward);
            Serializer.Instance.activeData.SetLastRewardTime(Functions.CurrentMillisInTimeZone());
            Serializer.Instance.activeData.SetLastReward(Serializer.Instance.activeData.lastReward + 1);
            SetGearText();
            UpdateDailyReward();
            Serializer.Instance.SaveData();
        }
    }

    private void UpdateDailyReward()
    {
        if (Functions.DaysSinceUnixFromMillis(Functions.CurrentMillisInTimeZone()) -
            Functions.DaysSinceUnixFromMillis(Serializer.Instance.activeData.lastRewardTime) > 0)
        {
            dailyRewardText.text = "+" + Serializer.Instance.activeData.lastReward.ToString();
            dailyGearImage.SetActive(true);
        }
        else
        {
            dailyRewardText.text = "Available Tomorrow";
            dailyRewardText.fontSize = 70;
            dailyGearImage.SetActive(false);
        }
    }
}