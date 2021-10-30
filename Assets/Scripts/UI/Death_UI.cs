using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

//Class to handle the Death Screen UI
public class Death_UI : UI
{
    //Properties to store refrences to the aspects of the UI
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private LevelBar levelBar;

    private bool levelBarAnimationFinished = false;

    //Method called when the UI is to be shown
    public override void Show()
    {
        //Activates the UI elements
        UIContainer.SetActive(true);

        //Triggers if not in the tutorial
        if (!Gamemode.inLevel("Tutorial"))
        {
            //Displays the players final score and highscore
            scoreText.text = $"FINAL SCORE: {Player.score}\nHIGHSCORE: {Serializer.activeData.highScore}";

            //Sets the values for the level bar
            levelBar.lowerLevel.text = Serializer.activeData.level.level.ToString();
            levelBar.higherLevel.text = (Serializer.activeData.level.level + 1).ToString();
            levelBar.levelBar.minValue = Level.LevelToXP(Serializer.activeData.level.level);
            levelBar.levelBar.maxValue = Level.LevelToXP(Serializer.activeData.level.level + 1);
            levelBar.levelBar.value = Serializer.activeData.level.xp;

            //Animates the level bar
            StartCoroutine(AnimateLevelBar());
        } 
        //Else flags that the level bar has finished animating and deactivates it
        else
        {
            levelBarAnimationFinished = true;
        }
    }

    //Method called when teh level bar is to be animated
    private IEnumerator AnimateLevelBar()
    {
        levelBarAnimationFinished = false;

        //Animates the level bar
        yield return StartCoroutine(AnimationPlus.AnimateLevelBar(levelBar, Serializer.activeData.level.level, Player.level.xp, 5f));

        //Saves the players level
        Serializer.activeData.SetLevel(Player.level);
        Serializer.SaveData();

        //Flags that the level bar animation has finished
        levelBarAnimationFinished = true;
    }

    //Functions which are called on botton clicks

    //Called when the user clicks to respawn
    public void Click_Respawn()
    {
        //Returns if the level bar hasn't finished animating
        if (!levelBarAnimationFinished)
            return;

        //Shows an ad if the user is on phone, respawns if the game is in debug mode
        if ((Gamemode.platform == Gamemode.Platform.Android || Gamemode.platform == Gamemode.Platform.IOS) && !Gamemode.inLevel("Tutorial"))
        {
            Refrence.adManager.ShowAd();
        }
        else if (Gamemode.platform == Gamemode.Platform.Debug || Gamemode.inLevel("Tutorial"))
        {
            OnRespawn();
        }
    }

    //Called when the user clicks to restart
    public void Click_Restart()
    {
        //Returns if the level bar hasn't finished animating
        if (!levelBarAnimationFinished)
            return;

        //Reloads main scene with screen fade
        StartCoroutine(ClickWithFade(() =>
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
        }, fadeTime, true));
    }

    //Called when the user clicks to go to the main menu
    public void Click_MainMenu()
    {
        //Returns if the level bar hasn't finished animating
        if (!levelBarAnimationFinished)
            return;

        //Reloads the start scene with screen fade
        StartCoroutine(ClickWithFade(() =>
        {
            SceneManager.LoadScene("Start", LoadSceneMode.Single);
        }, fadeTime, true));
    }

    //Method called when the player is to respawn
    public void OnRespawn()
    {
        //Tells the Player and Generator that the player is respawning
        Refrence.player.Respawn();
        Refrence.gen.Respawn();

        //Hides the UI
        Hide();
    }
}
