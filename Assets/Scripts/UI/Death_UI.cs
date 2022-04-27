using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

//Class to handle the Death Screen UI
public class Death_UI : UI<Death_UI>
{
    //Stores score text UI element
    [SerializeField] private TMP_Text scoreText;

    //Stores level bar UI element
    [SerializeField] private LevelBar levelBar;

    //Reference to the image to be used when signaling an ad would be played
    [SerializeField] private Sprite adImg;

    //Stores if the level bar has finished animating
    private bool levelBarAnimationFinished = false;

    //Stores the rewards multiplier
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

            //Sets higher and lower level text values to the current level (+1)
            levelBar.lowerLevel.text = Serializer.Instance.activeData.level.level.ToString();
            levelBar.higherLevel.text = (Serializer.Instance.activeData.level.level + 1).ToString();

            //Sets the min value to the current level in xp and max in the next level in xp
            levelBar.levelBar.minValue = Level.LevelToXP(Serializer.Instance.activeData.level.level);
            levelBar.levelBar.maxValue = Level.LevelToXP(Serializer.Instance.activeData.level.level + 1);

            //Sets the value of the slider to the current xp
            levelBar.levelBar.value = Serializer.Instance.activeData.level.xp;

            //Set multiplier to 0f (base value if user does not watch ad)
            multiplier = 0f;

            //Instantiates a prompt for watching an ad for 4x rewards
            Prompt prompt = Instantiate(Reference.smallPrompt, Reference.canvas).GetComponent<Prompt>();
            prompt.SetTitleText("Watch an Ad for 4x Rewards", 48);
            prompt.SetImages(new List<Sprite>() { adImg });

            //Add ad show method (lambda) for when the prompt is clicked
            prompt.AddButtonListener(new UnityEngine.Events.UnityAction(() =>
            {
                //Shows a rewarded ad call, with nested callback
                UnityAdsService.Instance.ShowRewardedAdThenCall(new System.Action(() =>
                {
                    //Set the multiplier to 3 (3+1=4x rewards)
                    Death_UI.Instance.multiplier = 3f;

                    //Closes the prompt
                    prompt.OnClickClose();
                }));
            }));

            //Animates the level bar
            StartCoroutine(AnimateLevelBar(prompt.gameObject));
        }
        else
        {
            //Else flag that the level bar is not animating
            levelBarAnimationFinished = true;
        }
    }

    //Method called when teh level bar is to be animated
    private IEnumerator AnimateLevelBar(GameObject prompt)
    {
        //Wait until the given prompt has been closed
        yield return new WaitUntil(() => prompt == null);

        //Flag that the level bar is animating
        levelBarAnimationFinished = false;

        //Add extra xp to the player based on the rewards multiplier - 1 (4-1=3)
        Player.level.AddXP((Player.level.xp - Serializer.Instance.activeData.level.xp) * multiplier);

        //Calculates the length of the animation
        float time = Level.XPToLevel(Player.level.xp) - Serializer.Instance.activeData.level.level > 0 ?
            Level.XPToLevel(Player.level.xp) - Serializer.Instance.activeData.level.level : Serializer.Instance.activeData.level.xp / Player.level.xp;

        //Animates the level bar and waits for it to finish
        yield return StartCoroutine(AnimationPlus.AnimateLevelBar(levelBar, Serializer.Instance.activeData.level.level, Player.level.xp, time));

        //Saves the players level and then saves all data
        Serializer.Instance.activeData.SetLevel(Player.level);
        Serializer.Instance.SaveData();

        //Flags that the level bar animation has finished
        levelBarAnimationFinished = true;
    }

    #region On Button Clicks
    //Called when the user clicks to respawn
    public void Click_Respawn()
    {
        //Returns if the level bar hasn't finished animating
        if (!levelBarAnimationFinished)
            return;

        //Trigger if the user is not in the tutorial mode or on windows
        if ((Gamemanager.Instance.CurrentPlatform != Gamemanager.Platform.Windows) && !Gamemanager.InLevel("Tutorial"))
        {
            //Shows an ad then respawns after
            UnityAdsService.Instance.ShowRewardedAdThenCall(() => Death_UI.Instance.OnRespawn());
        }
        //Trigger if the user is in the tutorial mode
        else if (Gamemanager.InLevel("Tutorial"))
        {
            //Respawn
            OnRespawn();
        }
    }

    //Called when the user clicks to restart
    public void Click_Restart()
    {
        //Returns if the level bar hasn't finished animating
        if (!levelBarAnimationFinished)
            return;

        //Activates the following method with a fade
        StartCoroutine(ClickWithFade(() =>
        {
            //Reloads the current scene
            SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
        },
        //Lasts standard fade time and flags that the action does change scenes 
        fadeTime, true));
    }

    //Called when the user clicks to go to the main menu
    public void Click_MainMenu()
    {
        //Returns if the level bar hasn't finished animating
        if (!levelBarAnimationFinished)
            return;

        //Activates the following method with a fade
        StartCoroutine(ClickWithFade(() =>
        {
            //Loads the start scene
            SceneManager.LoadScene("Start", LoadSceneMode.Single);
        },
        //Lasts standard fade time and flags that the action does change scenes
        fadeTime, true));
    }
    #endregion

    //Method called when the player is to respawn
    public void OnRespawn()
    {
        //Trigger if the player did not hit a finish
        if (Reference.gen.lastFinish == null)
        {
            //Restart
            Click_Restart();
            return;
        }

        //Activates the following method with a fade
        StartCoroutine(ClickWithFade(() =>
        {
            //Tells the Player and Generator that the player is respawning
            Reference.player.Respawn();
            Reference.gen.Respawn();

            //Hides the death screen
            Hide();
        },
        //Lasts standard fade time and flags that the action does not change scenes
        fadeTime, false));
    }
}
