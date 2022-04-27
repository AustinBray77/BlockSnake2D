using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

//Static class containing members for all UI types
public static class UI
{
    //Stores the normal fade time between screens
    public const float fadeTime = 0.2f;
}

//Interface for UIs to inherit from
public interface IUI
{
    //Base methods
    void Show();
    void Hide();
}

//Class for UI objects to inherit
public abstract class UI<_T> : Singleton<_T>, IUI where _T : Component, IUI
{
    //Stores the normal fade time between screens
    public const float fadeTime = UI.fadeTime;

    //Internal refrence to the screens fade panel
    [SerializeField] protected Image fadePanel;

    //Refrence to the container storing all the UI of this screen
    [SerializeField] protected GameObject UIContainer;

    //Refrence to some images
    [SerializeField] protected Sprite gearImg, skinBaseImg;

    //Overidable method called to show the UI object
    public virtual void Show()
    {
        //Activates all the UI elements
        UIContainer.SetActive(true);
    }

    //Method to show the UI object statically
    public static void ShowInstance()
    {
        //Shows the instance
        Instance.Show();
    }

    //Overidable method called to hide the UI object
    public virtual void Hide()
    {
        //Deactivates all the UI elements
        UIContainer.SetActive(false);
    }

    //Method to hide the UI object statically
    public static void HideInstance()
    {
        //Hides the instance
        Instance.Hide();
    }

    //Method called to invoke an action with a screen fade
    protected IEnumerator ClickWithFade(System.Action action, float seconds, bool changesScenes = false)
    {
        //Activates the fade panel
        fadePanel.gameObject.SetActive(true);

        //Fades to black and waits for it to finish
        yield return StartCoroutine(AnimationPlus.FadeToColor(fadePanel, new Color(0, 0, 0, 1), seconds));

        //Invokes the action
        action.Invoke();

        //Triggers if the action does not switch scenes
        if (!changesScenes)
            //Fades the screen back in and waits for it to finish
            yield return StartCoroutine(AnimationPlus.FadeToColor(fadePanel, new Color(0, 0, 0, 0), seconds, false));
    }

    //Method called to fill a dropdown object with enum data
    protected void FillDropFromEnum<T>(TMP_Dropdown dropdown, int[] ingoreValues = null) where T : System.Enum
    {
        //Loops through each value in the enum
        foreach (T value in System.Enum.GetValues(typeof(T)))
        {
            //Trigger if the some ignore values were passed in
            if (ingoreValues != null)
            {
                //If the value should be ignored, continue
                if (System.Array.IndexOf(ingoreValues, (int)(object)value) != -1)
                {
                    continue;
                }
            }

            //Adds the item text to the passed dropdown
            dropdown.options.Add(new TMP_Dropdown.OptionData(value.ToString()));
        }
    }

    //Method called to trigger a level prompt
    public void TriggerLevelPrompt(int index)
    {
        //Triggers if the index is out of bounds
        if (index >= Gamemanager.Instance.LevelUpTriggers.Count)
        {
            //Log that the index is out of bounds and return
            Log($"Player has unlocked all level achievements already={Gamemanager.Instance.LevelUpTriggers.Count}...", true);
            return;
        }

        //Set the trigger to the trigger at the index
        var trigger = Gamemanager.Instance.LevelUpTriggers[index];

        //Flag that the trigger has been activated and save
        Serializer.Instance.activeData.TriggerActivated(index);
        Serializer.Instance.SaveData();

        //Blank Prompt
        Prompt p;

        //Trigger if the reward does not unlock a skin
        if (!trigger.reward.unlocksSkin)
        {
            //Create a small prompt and save it
            p = Instantiate(Reference.smallPrompt, Reference.canvas).GetComponent<Prompt>();

            //Set the text to the the user how many gears they recieve
            p.SetDescriptions(new string[] { "+" + trigger.reward.gearReward.ToString() + " Gears" });

            //Set image to gear image
            p.SetImages(new List<Sprite>() { gearImg });
        }
        else
        {
            //Else create a medium prompt and save it
            p = Instantiate(Reference.mediumPrompt, Reference.canvas).GetComponent<Prompt>();

            //Set the text to the the user how many gears they recieve and that they unlocked a new skin
            p.SetDescriptions(new string[] { "+" + trigger.reward.gearReward.ToString() + " Gears", "New skin unlocked!" });

            //Set image to gear image and unknown skin image
            p.SetImages(new List<Sprite>() { gearImg, skinBaseImg });
        }

        //Set title to tell the user they levelled up
        p.SetTitleText("Leveled Up To Level " + (index + 1));

        //Close the prompt when the user clicks
        p.ToCloseOnClick();
    }
}
