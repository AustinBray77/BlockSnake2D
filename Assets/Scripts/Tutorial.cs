using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

//Class to control the tutorial scene
public class Tutorial : MonoBehaviour
{
    //List to store the keys the user should press during the tutorial and the associated text elements
    [SerializeField] private List<KeyCode> nextKey;
    [SerializeField] private List<TextMeshProUGUI> nextText;
    //Refrence to Top and Bottom walls
    [SerializeField] private GameObject topWall, bottomWall;

    //Refrence to the generator
    [SerializeField] private Generator gen;

    //Stores the total keycount at the start
    private int startKeyCount;

    //Stores the current fade in and out coroutines.
    private Coroutine curFadeIn, curFadeOut;

    //Method called on scene load
    private void Start()
    {
        //Assigns start key count
        startKeyCount = nextKey.Count;

        //Increases animation capacity
        LeanTween.init(1600);

        //Changes the tutorial text based on the platform
        if (Serializer.activeData.settings.movementType == Player.MovementType.Keyboard)
        {
            nextText[0].text = "Press W to Move Upward";
            nextText[1].text = "Press S to Move Downward";
        }
        else if (Serializer.activeData.settings.movementType == Player.MovementType.Buttons)
        {
            nextText[0].text = "Press the Up Arrow to Move Upward";
            nextText[1].text = "Press the Down Arrow to Move Downward";
        }
        else
        {
            nextText[0].text = "Press Above the Snake to Move Upward";
            nextText[1].text = "Press Below the Snake to Move Downward";
        }
    }

    //Method called once per frame
    private void FixedUpdate()
    {
        //Triggers if there are user inputs to check for
        if (nextKey.Count >= 1)
        {
            //Triggers if the user inputs the current key
            if (Input.GetKeyDown(nextKey[0]) || KeyToButtonPressed(nextKey[0]))
            {
                //Removes it and iterates and animates to the next text
                nextKey.RemoveAt(0);

                if (curFadeIn != null)
                {
                    StopCoroutine(curFadeIn);
                }

                curFadeOut = StartCoroutine(AnimationPlus.FadeText(nextText[0], UI.fadeTime));
                nextText.RemoveAt(0);

                //Checks that there is a next text, fades in the next text if there is
                if (nextText.Count >= 1)
                {
                    curFadeIn = StartCoroutine(AnimationPlus.FadeText(nextText[0], UI.fadeTime, false));
                }

                //Triggers after the W and S key have been hit
                if (nextKey.Count == startKeyCount - 2)
                {
                    //Moves the walls into position
                    DropWalls();

                    //Animates the UI
                    Reference.gameUI.FadeInElements();

                    //Starts the generator
                    gen.Initialize();

                    //Animates and removes the next text
                    StartCoroutine(WaitFadeCurrentText(3f, UI.fadeTime));
                }
            }
        }
    }

    //Method called to drop the walls into the players view
    private void DropWalls()
    {
        //Moves the top and bottom wall into position
        topWall.LeanMove(new Vector3(0, 11.5f), UI.fadeTime);
        bottomWall.LeanMove(new Vector3(0, -11.5f), UI.fadeTime);
    }
    //Functions to wait then fade the current text element
    private IEnumerator WaitFadeCurrentText(float waitTime, float fadeTime, bool fadeOut = true)
    {
        //Waits the desired amount of time
        yield return new WaitForSeconds(waitTime);

        //Animates and removes the next text
        StartCoroutine(AnimationPlus.FadeText(nextText[0], fadeTime));
        nextText.RemoveAt(0);
    }

    //Returns if the current key's associated button 
    private bool KeyToButtonPressed(KeyCode keyCode)
    {
        //Returns the up button if the keycode is W
        if (keyCode == KeyCode.W)
            return Reference.player.GetControls()[0];

        //Returns the down button if the keycode is S
        if (keyCode == KeyCode.S)
            return Reference.player.GetControls()[1];

        //Returns false if the key doesn't have an associated button
        return false;
    }

    //Method to show popup pertaining to the popup
    public IEnumerator ShowFinishInfo()
    {
        GameObject infoObject = Instantiate(Reference.infoObject, Reference.canvas.transform);
        TextMeshProUGUI text = infoObject.GetComponentInChildren<TextMeshProUGUI>();
        text.text = "Congrats you just reached a checkpoint.\nPick one of the following upgrades to be applied to your snake.\n(Click anywhere to continue)";

        yield return new WaitUntil(() => Functions.UserIsClicking());

        Destroy(infoObject);
    }
}
