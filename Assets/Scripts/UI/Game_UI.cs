using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

//Class to control the game UI
public class Game_UI : UI<Game_UI>
{
    //Object refrences to the controls
    private GameObject controls;
    [SerializeField] private GameObject controlsLeft, controlsRight;

    //Refrence to the score text
    [SerializeField] private TextMeshProUGUI scoreText;

    //Refrence to the gear count text and Image
    [SerializeField] private TextMeshProUGUI gearText;
    [SerializeField] private Image gearImage;

    //Refrence to the shield image and count
    [SerializeField] private Image shieldImage;
    [SerializeField] private TextMeshProUGUI shieldText;

    //Refrence to the slowdown image and its cover
    private Image slowDownImage;
    public RectTransform SlowDownCover;

    //Refrence to the shock image and count
    private Image shockImage;
    private TextMeshProUGUI shockText;

    //Refrence to parent canvas
    [SerializeField] private Canvas canvas;

    //Refrence to the camera in the scene
    [SerializeField] private Camera cam;

    //Stores the rect transforms of the buttons
    List<RectTransform> buttons;

    //Stores the bounds of the buttons
    private Vector2[] _buttonBounds = null;

    //Stores the screen width and height
    float _screenWidth, _screenHeight;

    //Get only property for the bounds of the buttons
    public Vector2[] ButtonBounds
    {
        get
        {
            //Trigger if the current screen height or width does not match the stored values
            if (_screenHeight != Screen.height || _screenWidth != Screen.width)
            {
                //Recalculate the buttons bounds
                _buttonBounds = CalculateButtonBounds();

                //Save the new screen height and width
                _screenHeight = Screen.height;
                _screenWidth = Screen.width;

            }

            //Returns the bound of the buttons
            return _buttonBounds;
        }
    }

    //Coroutine called when the scene is loaded
    private IEnumerator Start()
    {
        //Wait until the save data is loaded
        yield return new WaitUntil(() => Serializer.Instance.activeData != null);

        //Save the current screen height and width
        _screenHeight = Screen.height;
        _screenWidth = Screen.width;

        //Trigger if the controls are left handed or the buttons are to be disabled
        if (Serializer.Instance.activeData.settings.leftHandedControls || Serializer.Instance.activeData.settings.movementType != Player.MovementType.Buttons)
        {
            //Saves the controls to be the left ones
            controls = controlsLeft;

            //Deactivates the right controls
            controlsRight.SetActive(false);
        }
        //Else trigger if the controls are right
        else
        {
            //Saves the controls to be the right ones
            controls = controlsRight;

            //Deactivates the left contorols
            controlsLeft.SetActive(false);
        }

        //Sets the movement buttons to be active depending on the movement type
        controls.transform.GetChild(0).gameObject.SetActive(Serializer.Instance.activeData.settings.movementType == Player.MovementType.Buttons);

        //Sets the gear text
        gearText.text = Serializer.Instance.activeData.gearCount.ToString();

        //Hides shield UI
        shieldImage.gameObject.SetActive(false);
        shieldText.gameObject.SetActive(false);

        //Gets the rect transforms of the movement buttons
        buttons = controls.transform.GetChild(0).GetComponentsInChildren<RectTransform>().ToList();
        buttons.RemoveAt(0);

        //Gets the shock UI
        shockImage = controls.transform.GetChild(1).GetComponentInChildren<Image>();
        shockText = controls.transform.GetChild(1).GetComponentInChildren<TextMeshProUGUI>();

        //Sets the shock UI to false
        shockImage.gameObject.SetActive(false);
        shockText.gameObject.SetActive(false);

        //Gets the slowdown UI
        slowDownImage = controls.transform.GetChild(2).GetChild(0).GetComponentInChildren<Image>();
        SlowDownCover = controls.transform.GetChild(2).GetChild(0).GetComponent<RectTransform>();

        //Sets the cover of the slowdown UI to false
        SlowDownCover.gameObject.SetActive(false);

        //Initializes lean tween animation
        LeanTween.init();

        //Calculates the button bounds
        _buttonBounds = CalculateButtonBounds();
    }

    //Method called to update the score in UI
    public void UpdateScore(int score) =>
        //Sets the text to the score
        scoreText.text = score.ToString();


    //Method called to update the gear count in UI
    public void UpdateGearCount(int gearCount) =>
        //Sets the text to the gear count
        gearText.text = gearCount.ToString();


    //Method called to update the shield count in UI
    public void UpdateShieldCount(int shieldCount)
    {
        //Triggers if the new shield count is 0
        if (shieldCount == 0)
        {
            //Sets shield text
            shieldText.text = "0";

            //Animates the dissapearance of the shield image and text and deactivates image
            StartCoroutine(AnimationPlus.FadeToColor(shieldImage, new Color(0, 0, 0, 0), fadeTime, false));
            StartCoroutine(AnimationPlus.FadeText(shieldText, fadeTime));

            return;
        }

        //Trigger if the shield image is not active
        if (!shieldImage.gameObject.activeInHierarchy)
        {
            //Activate the shield UI
            shieldImage.gameObject.SetActive(true);
            shieldText.gameObject.SetActive(true);

            //Animate the appearance of the shield UI
            StartCoroutine(AnimationPlus.FadeToColor(shieldImage, new Color(1, 1, 1, 1), fadeTime));
            StartCoroutine(AnimationPlus.FadeText(shieldText, fadeTime, false));
        }

        //Set the shield text to the shield count
        shieldText.text = shieldCount.ToString();
    }

    //Method called to update the shield count in UI
    public void UpdateShockCount(int shockCount)
    {
        //Triggers if the new shock count is 0
        if (shockCount == 0)
        {
            //Sets shock text
            shockText.text = "0";

            //Animates the dissapearance of the shock image and text and deactivates the shock image
            StartCoroutine(AnimationPlus.FadeToColor(shockImage, new Color(0, 0, 0, 0), fadeTime, false));
            StartCoroutine(AnimationPlus.FadeText(shockText, fadeTime));
            return;
        }

        //Trigger if the shock image is not active
        if (!shockImage.gameObject.activeInHierarchy)
        {
            //Activate the shock UI
            shockImage.gameObject.SetActive(true);
            shockText.gameObject.SetActive(true);

            //Animate the appearance of the shock UI
            StartCoroutine(AnimationPlus.FadeToColor(shockImage, new Color(1, 1, 1, 1), fadeTime));
            StartCoroutine(AnimationPlus.FadeText(shockText, fadeTime, false));
        }

        //Set the shock text to the shock count
        shockText.text = shockCount.ToString();
    }

    //Method called to enable slowdown UI
    public void EnableSlowDown(bool state, bool useFade = true, bool ignoreCurrent = false)
    {
        //Trigger if state is true and (the UI is currently transparent (a=0) or is set to ignore the value of the UI)
        if (state && (slowDownImage.color == new Color(1, 1, 1, 0) || ignoreCurrent))
        {
            //Trigger if the it should use fade
            if (useFade)
            {
                //Fade in the UI
                StartCoroutine(AnimationPlus.FadeToColor(slowDownImage, new Color(1, 1, 1, 1), fadeTime));
            }
            else
            {
                //Else just set the UI to be fully opaque (a=1)
                slowDownImage.color = new Color(1, 1, 1, 1);
            }
        }
        //Trigger if state is false and (the UI is currently opaque (a=1) or is set to ignore the value of the UI)
        else if (!state && (slowDownImage.color == new Color(1, 1, 1, 1) || ignoreCurrent))
        {
            //Trigger if the it should use fade
            if (useFade)
            {
                //Fade out the UI
                StartCoroutine(AnimationPlus.FadeToColor(slowDownImage, new Color(1, 1, 1, 0), fadeTime));
            }
            else
            {
                //Else just set the UI to be fully transparent (a=0)
                slowDownImage.color = new Color(1, 1, 1, 0);
            }
        }
    }

    //Method used to return the bounds of the control buttons
    public Vector2[] CalculateButtonBounds()
    {
        //Initializes return object
        Vector2[] output = new Vector2[8];

        //Top movement button
        output[0] = (Vector2)buttons[0].position + new Vector2(buttons[0].rect.width, buttons[0].rect.height) * scalingFactor / 2;
        output[1] = (Vector2)buttons[0].position - new Vector2(buttons[0].rect.width, buttons[0].rect.height) * scalingFactor / 2;

        //Bottom movement button
        output[2] = (Vector2)buttons[1].position + new Vector2(buttons[1].rect.width, buttons[1].rect.height) * scalingFactor / 2;
        output[3] = (Vector2)buttons[1].position - new Vector2(buttons[1].rect.width, buttons[1].rect.height) * scalingFactor / 2;

        //Slowdown powerup button
        RectTransform slowDown = slowDownImage.gameObject.GetComponent<RectTransform>();
        output[4] = (Vector2)slowDown.position + new Vector2(slowDown.rect.width, slowDown.rect.height) * scalingFactor / 2;
        output[5] = (Vector2)slowDown.position - new Vector2(slowDown.rect.width, slowDown.rect.height) * scalingFactor / 2;

        //Shock powerup button
        RectTransform shock = shockImage.gameObject.GetComponent<RectTransform>();
        output[6] = (Vector2)shock.position + new Vector2(shock.rect.width, shock.rect.height) * scalingFactor / 2;
        output[7] = (Vector2)shock.position - new Vector2(shock.rect.width, shock.rect.height) * scalingFactor / 2;

        //Returns the bounds
        return output;
    }

    //Method to fade in the main UI elements
    public void FadeInElements()
    {
        //Sets all the elements to transparent (a=0)
        scoreText.color -= new Color(0, 0, 0, 1);
        gearText.color -= new Color(0, 0, 0, 1);
        gearImage.color = new Color(1, 1, 1, 0);

        //Fade in the elements
        StartCoroutine(AnimationPlus.FadeText(scoreText, fadeTime, false));
        StartCoroutine(AnimationPlus.FadeText(gearText, fadeTime, false));
        StartCoroutine(AnimationPlus.FadeToColor(gearImage, new Color(1, 1, 1, 1), fadeTime));
    }

    //Property to get scaling UI factor
    public float scalingFactor => canvas.transform.localScale.x;
}
