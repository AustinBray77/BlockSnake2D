using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

//Class inherited by UI objects
public abstract class UI : BaseBehaviour
{
    //Stores the normal fade time between screens
    public static float fadeTime = 0.2f;

    //Internal refrence to the screens fade panel
    [SerializeField] protected Image fadePanel;

    //Refrence to the container storing all the UI of this screen
    [SerializeField] protected GameObject UIContainer;

    //Refrence to some images
    [SerializeField] protected Sprite gearImg, skinBaseImg;

    //Serializable class to store a refrence to a level bar
    [System.Serializable]
    public class LevelBar
    {
        public TMP_Text lowerLevel, higherLevel;
        public Slider levelBar;
    }

    //Overidable method called to show the UI object
    public virtual void Show()
    {
        UIContainer.SetActive(true);
    }

    //Overidable method called to hide the UI object
    public virtual void Hide()
    {
        UIContainer.SetActive(false);
    }

    //Method called to invoke an action with a screen fade
    protected IEnumerator ClickWithFade(System.Action action, float seconds, bool changesScenes = false)
    {
        //Fades the screen and waits until the screen has finished fading 
        fadePanel.gameObject.SetActive(true);
        yield return StartCoroutine(AnimationPlus.FadeToColor(fadePanel, new Color(0, 0, 0, 1), seconds));

        //Invokes the action
        action.Invoke();

        //Triggers another screen fade if the action does not switch scenes
        if (!changesScenes)
            yield return StartCoroutine(AnimationPlus.FadeToColor(fadePanel, new Color(0, 0, 0, 0), seconds, false));
    }

    //Method called to fill a dropdown object with enum data
    protected void FillDropFromEnum<T>(TMP_Dropdown dropdown, int[] ingoreValues = null) where T : System.Enum
    {
        //Adds the items to the quality select dropdown
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

            //Adds the item text
            dropdown.options.Add(new TMP_Dropdown.OptionData(value.ToString()));
        }
    }

    public void TriggerLevelPrompt(int index)
    {
        var trigger = Reference.levelUpTriggers[index];
        Serializer.Instance.activeData.TriggerActivated(index);
        Serializer.Instance.SaveData();

        Prompt p;

        if (!trigger.reward.unlocksSkin)
        {
            p = Instantiate(Reference.smallPrompt, Reference.canvas).GetComponent<Prompt>();
            p.SetDescriptions(new string[] { "+" + trigger.reward.gearReward.ToString() + " Gears" });
            p.SetImages(new List<Sprite>() { gearImg });
        }
        else
        {
            p = Instantiate(Reference.mediumPrompt, Reference.canvas).GetComponent<Prompt>();
            p.SetDescriptions(new string[] { "+" + trigger.reward.gearReward.ToString() + " Gears", "New skin unlocked!" });
            p.SetImages(new List<Sprite>() { gearImg, skinBaseImg });
        }

        p.SetTitleText("Leveled Up To Level " + (index + 1));
        p.ToCloseOnClick();
    }
}
