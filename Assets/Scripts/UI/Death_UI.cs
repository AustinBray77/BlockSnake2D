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
    [SerializeField] private Sprite adImg;

    private bool levelBarAnimationFinished = false;

    public float multiplier = 1f;

    //Method called when the UI is to be shown
    public override void Show()
    {
        //Activates the UI elements
        UIContainer.SetActive(true);

        //Triggers if not in the tutorial
        if (!Gamemanager.InLevel("Tutorial"))
        {
            //Displays the players final score and highscore
            scoreText.text = $"FINAL SCORE: {Player.score}\nHIGHSCORE: {Serializer.Instance.activeData.highScore}";

            //Sets the values for the level bar
            levelBar.lowerLevel.text = Serializer.Instance.activeData.level.level.ToString();
            levelBar.higherLevel.text = (Serializer.Instance.activeData.level.level + 1).ToString();
            levelBar.levelBar.minValue = Level.LevelToXP(Serializer.Instance.activeData.level.level);
            levelBar.levelBar.maxValue = Level.LevelToXP(Serializer.Instance.activeData.level.level + 1);
            levelBar.levelBar.value = Serializer.Instance.activeData.level.xp;

            //Prompt ad
            multiplier = 0f;

            Prompt prompt = Instantiate(Reference.smallPrompt, Reference.canvas).GetComponent<Prompt>();
            prompt.SetTitleText("Watch an Ad for 4x Rewards", 48);
            prompt.SetImages(new List<Sprite>() { adImg });

            prompt.AddButtonListener(new UnityEngine.Events.UnityAction(() =>
            {
                Reference.adManager.ShowRewardedAdThenCall(new System.Action(() =>
                {
                    Reference.deathUI.multiplier = 3f;
                    prompt.OnClickClose();
                }));
            }));

            //Animates the level bar
            StartCoroutine(AnimateLevelBar(prompt.gameObject));
        }
        //Else flags that the level bar has finished animating and deactivates it
        else
        {
            levelBarAnimationFinished = true;
        }
    }

    //Method called when teh level bar is to be animated
    private IEnumerator AnimateLevelBar(GameObject prompt)
    {
        yield return new WaitUntil(() => prompt == null);

        Log((Player.level.xp - Serializer.Instance.activeData.level.xp));

        levelBarAnimationFinished = false;
        Player.level.AddXP((Player.level.xp - Serializer.Instance.activeData.level.xp) * multiplier);

        //Calculates the length of the animation
        float time = Level.XPToLevel(Player.level.xp) - Serializer.Instance.activeData.level.level > 0 ?
            Level.XPToLevel(Player.level.xp) - Serializer.Instance.activeData.level.level : Serializer.Instance.activeData.level.xp / Player.level.xp;

        //Animates the level bar
        yield return StartCoroutine(AnimationPlus.AnimateLevelBar(levelBar, Serializer.Instance.activeData.level.level, Player.level.xp, time));

        //Saves the players level
        Serializer.Instance.activeData.SetLevel(Player.level);
        Serializer.Instance.SaveData();

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
        if ((Gamemanager.Instance.CurrentPlatform != Gamemanager.Platform.Windows) && !Gamemanager.InLevel("Tutorial"))
        {
            Reference.adManager.ShowRewardedAdThenCall(() => Reference.deathUI.OnRespawn());
        }
        else if (Gamemanager.InLevel("Tutorial"))
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
        //If the player never hit a check point reload the scene
        if (Reference.gen.lastFinish == null)
        {
            Click_Restart();
            return;
        }

        StartCoroutine(ClickWithFade(() =>
        {
            //Tells the Player and Generator that the player is respawning
            Reference.player.Respawn();
            Reference.gen.Respawn();
            //Hides the UI
            Hide();
        }, fadeTime, false));
    }
}
