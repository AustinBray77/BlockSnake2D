using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

//Class to contorl the start UI
public class Start_UI : UI<Start_UI>
{
    //Stores a reference to the loading text and the animated snake
    [SerializeField] private GameObject loadingText, animatedSnake;

    //Method called on scene load
    private IEnumerator Start()
    {
        //Trigger if the platform is andriod
        if (Gamemanager.Instance.CurrentPlatform == Gamemanager.Platform.Android)
        {
            //Wait until both the play games service instance and serializer instance are not null
            yield return new WaitUntil(() => Serializer.Instance != null && PlayGamesService.Instance != null);

            //Wait until login completion from play games service
            yield return StartCoroutine(PlayGamesService.Instance.SignIn(false));
        }
        else
        {
            //Else wait until the the serializer instance is not null
            yield return new WaitUntil(() => Serializer.Instance != null);
        }

        //Shows the loading text
        loadingText.SetActive(true);

        //Updates the current time from the server
        yield return StartCoroutine(Functions.UpdateCurrentTime());

        //Hides the loading text
        loadingText.SetActive(false);

        //Triggers if the current time is equal to 1/1/1970 (connection error)
        if (Functions.CurrentTime == new System.DateTimeOffset().DateTime)
        {
            //Initiates a prompt
            Prompt p = Instantiate(Reference.smallPrompt, transform.parent).GetComponent<Prompt>();

            //Sets title to connection error
            p.SetTitleText("Connection Error");

            //Sets the image to the ? image
            p.SetImages(new List<Sprite> { skinBaseImg });

            //Tells the user to check their wifi connection
            p.SetDescriptions(new string[] { "Check Wifi Connection" });

            //When the user clicks, quit the game
            p.AddButtonListener(new UnityEngine.Events.UnityAction(() => Application.Quit()));

            //Forces the user to interact with the popup
            p.ForceInteraction();

            //Exit
            yield break;
        }

        //Triggers if the save data is not loaded
        if (Serializer.Instance.activeData == null)
        {
            //Loads the saved data
            yield return StartCoroutine(Serializer.Instance.LoadData());

            //Loops trough each level trigger
            for (int i = 0; i < Serializer.Instance.activeData.activatedLevelTriggers.Length; i++)
            {
                //Trigger if the level trigger should have been activated but wasn't
                if (!Serializer.Instance.activeData.activatedLevelTriggers[i] && Gamemanager.Instance.LevelUpTriggers[i].levelTrigger <= Serializer.Instance.activeData.level.level)
                {
                    //Show a prompt for the level trigger
                    TriggerLevelPrompt(i);
                }
            }
        }
        else
        {
            //Else log
            Log("Data is not null.");
            Log($"Active Data:{Serializer.Instance.activeData == null}");
            Log($"Settings:{Serializer.Instance.activeData.settings == null}");
        }

        //Gets the difference since last reward time in days
        long dayDiff = Functions.DaysSinceUnixFromMillis(Functions.CurrentMillisInTimeZone()) - Functions.DaysSinceUnixFromMillis(Serializer.Instance.activeData.lastRewardTime);

        //Trigger if the difference is greater than 0
        if (dayDiff > 0)
        {
            //Create a prompt
            Prompt p = Instantiate(Reference.smallPrompt, transform.parent).GetComponent<Prompt>();

            //Set prompt title to daily reward
            p.SetTitleText("Daily Reward");

            //Sets image on prompt to gear
            p.SetImages(new List<Sprite> { gearImg });

            //Tells the user they can collect the reward in the shop
            p.SetDescriptions(new string[] { "Check The Shop To Collect" }, new int[] { 48 });

            //Flags the prompt to close on click
            p.ToCloseOnClick();

            //Trigger if the user did not collect their reward yesterday
            if (dayDiff >= 2)
            {
                //Resets the reward to the first value
                Serializer.Instance.activeData.SetLastReward(5);
            }
        }

        //Activates the fade panel
        fadePanel.gameObject.SetActive(true);

        //Sets it to be opaque
        fadePanel.color = new Color(0, 0, 0, 1);

        //Fades the fade panel to transparent
        StartCoroutine(AnimationPlus.FadeToColor(fadePanel, new Color(0, 0, 0, 0), fadeTime, false));

        //Saves the data every time the user goes to the main menu
        Serializer.Instance.SaveData();
    }

    //Functions which are called on botton clicks

    //Method called when the user clicks to start
    public void Click_Start()
    {
        //Activates the following method with a fade
        StartCoroutine(ClickWithFade(
            () =>
            {
                //Shows the mode select UI
                ModeSelect_UI.ShowInstance();

                //Hides current UI
                Hide();
            },
            //Animates over fade time time
            fadeTime));
    }

    //Method called when the user clicks to the shop
    public void Click_Shop()
    {
        //Activates the following method with a fade
        StartCoroutine(ClickWithFade(
            () =>
            {
                //Shows the shop UI
                Shop_UI.ShowInstance();

                //Hides current UI
                Hide();
            },
            //Animates over fade time time
            fadeTime));
    }

    //Method called when the user clicks to enter the tutorial
    public void Click_Tutorial()
    {
        //Activates the following method with a fade
        StartCoroutine(ClickWithFade(
            () =>
            {
                //Loads the tutorial scene
                SceneManager.LoadScene("Tutorial", LoadSceneMode.Single);
            },
            //Animates over fade time time and flags that the method switches scenes
            fadeTime, true));
    }

    //Method called when the user clicks to enter the settings
    public void Click_Settings()
    {
        //Activates the following method with a fade
        StartCoroutine(ClickWithFade(
            () =>
            {
                //Shows the settings UI
                Settings_UI.ShowInstance();

                //Hides the current UI
                Hide();
            },
            //Animates over fade time time
            fadeTime));
    }

    //Method called when the user clicks to enter the credits
    public void Click_Credits()
    {
        //Activates the following method with a fade
        StartCoroutine(ClickWithFade(
            () =>
            {
                //Shows the credits UI
                Credits_UI.ShowInstance();

                //Hides the current UI
                Hide();
            },
            //Animates over fade time time
            fadeTime));
    }

    //Method called when the user clicks to quit
    public void Click_Quit()
    {
        //Saves the data
        Serializer.Instance.SaveData();

        //Exits the application
        Application.Quit();
    }

    //Method called when the user clicks to show the leaderboard
    public void ClickLeaderboard()
    {
        //Triggers if the user is on android
        if (Gamemanager.Instance.CurrentPlatform == Gamemanager.Platform.Android)
        {
            //Shows the google play games leaderboard
            PlayGamesService.Instance.ShowLeaderboard(PlayGamesService.HighScoreID);
        }
    }

    //Overidable method called to show the UI object
    public override void Show()
    {
        //Activates the animated snake
        animatedSnake.SetActive(true);

        //Calls base show behaviour
        base.Show();
    }

    //Method called to hide the UI object
    public override void Hide()
    {
        //Deactivates the animated snake
        animatedSnake.SetActive(false);

        //Calls base hide behaviour
        base.Hide();
    }
}
