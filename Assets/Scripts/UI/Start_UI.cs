using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

//Class to contorl the start UI
public class Start_UI : UI
{
    [SerializeField] private GameObject loadingText, animatedSnake;

    //Called on scene load
    private IEnumerator Start()
    {
        bool firstLogin = !System.IO.File.Exists(Serializer.fileName);

        loadingText.SetActive(true);
        yield return StartCoroutine(Functions.UpdateCurrentTime());
        loadingText.SetActive(false);

        if (Functions.currentTime == new System.DateTimeOffset().DateTime)
        {
            Prompt p = Instantiate(Reference.smallPrompt, transform.parent).GetComponent<Prompt>();
            p.SetTitleText("Connection Error");
            p.SetImages(new List<Sprite> { skinBaseImg });
            p.SetDescriptions(new string[] { "Check Wifi Connection" });
            p.AddButtonListener(new UnityEngine.Events.UnityAction(() => Application.Quit()));
            p.ForceInteraction();
            yield break;
        }

        //Loads the data if no save data is currently present
        if (Serializer.Instance.activeData == null)
        {
            Serializer.Instance.LoadData();

            for (int i = 0; i < Serializer.Instance.activeData.activatedLevelTriggers.Length; i++)
            {
                if (!Serializer.Instance.activeData.activatedLevelTriggers[i] && Reference.levelUpTriggers[i].levelTrigger <= Serializer.Instance.activeData.level.level)
                {
                    TriggerLevelPrompt(i);
                }
            }
        }

        long dayDiff = Functions.DaysSinceUnixFromMillis(Functions.CurrentMillisInTimeZone()) - Functions.DaysSinceUnixFromMillis(Serializer.Instance.activeData.lastRewardTime);

        if (dayDiff > 0 || firstLogin)
        {
            Prompt p = Instantiate(Reference.smallPrompt, transform.parent).GetComponent<Prompt>();
            p.SetTitleText("Daily Reward");
            p.SetImages(new List<Sprite> { gearImg });
            p.SetDescriptions(new string[] { "Check The Shop To Collect" }, new int[] { 48 });
            p.ToCloseOnClick();

            if (dayDiff >= 2)
            {
                Serializer.Instance.activeData.SetLastReward(5);
            }
        }

        //Fades in
        fadePanel.gameObject.SetActive(true);
        fadePanel.color = new Color(0, 0, 0, 1);
        StartCoroutine(AnimationPlus.FadeToColor(fadePanel, new Color(0, 0, 0, 0), fadeTime, false));

        //Saves the data every time the user goes to the main menu
        Serializer.Instance.SaveData();
    }

    //Functions which are called on botton clicks

    //Called when the user clicks to start
    public void Click_Start()
    {
        //Loads the main scene with fade
        StartCoroutine(ClickWithFade(
            () =>
            {
                Reference.modeSelectUI.Show();
                Hide();
            }, fadeTime));
    }

    //Called when the user clicks to the shop
    public void Click_Shop()
    {
        //Loads the shop UI with fade
        StartCoroutine(ClickWithFade(
            () =>
            {
                Reference.shopUI.Show();
                Hide();
            }, fadeTime));
    }

    //Called when the user clicks to enter the tutorial
    public void Click_Tutorial()
    {
        //Loads the tutorial scene with fade
        StartCoroutine(ClickWithFade(
             () =>
             {
                 SceneManager.LoadScene("Tutorial", LoadSceneMode.Single);
             }, fadeTime, true));
    }

    //Called when the user clicks to enter the settings
    public void Click_Settings()
    {
        //Loads the settings screen with fade
        StartCoroutine(ClickWithFade(
            () =>
            {
                Reference.settingsUI.Show();
                Hide();
            }, fadeTime));
    }

    //Called when the user clicks to enter the credits
    public void Click_Credits()
    {
        //Loads credits screen with fade
        StartCoroutine(ClickWithFade(
            () =>
            {
                Reference.creditsUI.Show();
                Hide();
            }, fadeTime));
    }

    //Called when the user clicks to quit
    public void Click_Quit()
    {
        //Saves the data and exits the application
        Serializer.Instance.SaveData();
        Application.Quit();
    }

    //Overidable method called to show the UI object
    public override void Show()
    {
        animatedSnake.SetActive(true);
        base.Show();
    }

    //Method called to hide the UI object
    public override void Hide()
    {
        animatedSnake.SetActive(false);
        base.Hide();
    }
}
