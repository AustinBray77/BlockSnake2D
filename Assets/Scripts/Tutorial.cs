using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

//Class to control the tutorial scene
public class Tutorial : BaseBehaviour
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

        //Trigger if the player is using keyboard controls
        if (Serializer.Instance.activeData.settings.movementType == Player.MovementType.Keyboard)
        {
            //Sets the texts to tell the user how to move with the keyboard
            nextText[0].text = "Press W to Move Upward";
            nextText[1].text = "Press S to Move Downward";
        }
        //Trigger if the player is using button controls
        else if (Serializer.Instance.activeData.settings.movementType == Player.MovementType.Buttons)
        {
            //Sets the texts to tell the user how to move with the buttons
            nextText[0].text = "Press the Up Arrow to Move Upward";
            nextText[1].text = "Press the Down Arrow to Move Downward";
        }
        //Trigger if the player is using touch controls
        else
        {
            //Sets the texts to tell the user how to move with touch controls
            nextText[0].text = "Press Above the Snake to Move Upward";
            nextText[1].text = "Press Below the Snake to Move Downward";
        }
    }

    //Method called once per frame
    private void Update()
    {
        //Triggers if there are user inputs to check for
        if (nextKey.Count >= 1)
        {
            //Triggers if the user inputs the current key
            if (Input.GetKeyDown(nextKey[0]) || KeyToButtonPressed(nextKey[0]))
            {
                //Removes the current key as it has been pressed
                nextKey.RemoveAt(0);

                //Trigger if text is currently fading in
                if (curFadeIn != null)
                {
                    //Stop the text from fading in
                    StopCoroutine(curFadeIn);
                }

                //Save that the current text is fading out
                curFadeOut = StartCoroutine(AnimationPlus.FadeText(nextText[0], UI.fadeTime));

                //Removes the text being faded out from the text list
                nextText.RemoveAt(0);

                //Trigger if there are still texts to be faded in
                if (nextText.Count >= 1)
                {
                    //Fade in the next text
                    curFadeIn = StartCoroutine(AnimationPlus.FadeText(nextText[0], UI.fadeTime, false));
                }

                //Triggers after the W and S key have been hit
                if (nextKey.Count == startKeyCount - 2)
                {
                    //Moves the walls into position
                    DropWalls();

                    //Animates the UI
                    Game_UI.Instance.FadeInElements();

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
        //Animates the top and bottom wall into position
        topWall.LeanMove(new Vector3(0, 11.5f), UI.fadeTime);
        bottomWall.LeanMove(new Vector3(0, -11.5f), UI.fadeTime);
    }

    //Method to wait then fade the current text element
    private IEnumerator WaitFadeCurrentText(float waitTime, float fadeTime, bool fadeOut = true)
    {
        //Waits the desired amount of time
        yield return new WaitForSeconds(waitTime);

        //Animates the next text
        StartCoroutine(AnimationPlus.FadeText(nextText[0], fadeTime));

        //Removes it from the list
        nextText.RemoveAt(0);
    }

    //Method to convert a key to its associated button and check if that button is pressed
    private bool KeyToButtonPressed(KeyCode keyCode)
    {
        //Returns the up button if the keycode is W
        if (keyCode == KeyCode.W)
            return Reference.player.ButtonsPressed[0];

        //Returns the down button if the keycode is S
        if (keyCode == KeyCode.S)
            return Reference.player.ButtonsPressed[1];

        //Returns false if the key doesn't have an associated button
        return false;
    }

    //Method to show popup pertaining to the popup
    public IEnumerator ShowFinishInfo()
    {
        //Creates the Info Object that covers the screen
        GameObject infoObject = Instantiate(Reference.infoObject, Reference.canvas.transform);

        //Gets the text element from the info object
        TextMeshProUGUI text = infoObject.GetComponentInChildren<TextMeshProUGUI>();

        //Tells the user how to use the upgrade screen
        text.text = "Congrats you just reached a checkpoint.\nPick one of the following upgrades to be applied to your snake.\n(Click anywhere to continue)";

        //Waits until the user clicks
        yield return new WaitUntil(() => Functions.UserIsClicking());

        //Destroys the info object
        Destroy(infoObject);
    }
}
