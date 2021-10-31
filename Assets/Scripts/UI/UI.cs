using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

//Class inherited by UI objects
public abstract class UI : MonoBehaviour
{
    //Stores the normal fade time between screens
    public static float fadeTime = 1f;

    //Internal refrence to the screens fade panel
    [SerializeField] protected Image fadePanel;

    //Refrence to the container storing all the UI of this screen
    [SerializeField] protected GameObject UIContainer;

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

    //Internal method called to invoke an action with a screen fade
    internal IEnumerator ClickWithFade(System.Action action, float seconds, bool changesScenes = false)
    {
        //Fades the screen and waits until the screen has finished fading 
        fadePanel.gameObject.SetActive(true);
        yield return StartCoroutine(AnimationPlus.FadeToColor(fadePanel, new Color(0, 0, 0, 1), seconds));

        //Invokes the action
        action.Invoke();

        //Triggers another screen fade if the action does not switch scenes
        if(!changesScenes)
            yield return StartCoroutine(AnimationPlus.FadeToColor(fadePanel, new Color(0, 0, 0, 0), seconds, false));
    }
}
