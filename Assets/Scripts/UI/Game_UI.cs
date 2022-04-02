using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

//Class to control the game UI
public class Game_UI : UI<Game_UI>
{
    //Object refrence to the controls
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

    //Refrence to the slowdown image
    private Image slowDownImage;
    public RectTransform SlowDownCover;

    //Refrence to the shock image and count
    private Image shockImage;
    private TextMeshProUGUI shockText;

    //Refrence to parent canvas
    [SerializeField] private Canvas canvas;

    //Refrence to the camera in the scene
    [SerializeField] private Camera cam;

    //Gets the buttons
    List<RectTransform> buttons;

    private Vector2[] _buttonBounds = null;

    float _screenWidth, _screenHeight;

    public Vector2[] ButtonBounds
    {
        get
        {
            if (_screenHeight != Screen.height || _screenWidth != Screen.width)
            {
                _buttonBounds = GetButtonBounds();
                _screenHeight = Screen.height;
                _screenWidth = Screen.width;

            }

            return _buttonBounds;
        }
    }

    //Called when the scene is loaded
    private IEnumerator Start()
    {
        yield return new WaitUntil(() => Serializer.Instance.activeData != null);

        _screenHeight = Screen.height;
        _screenWidth = Screen.width;

        if (Serializer.Instance.activeData.settings.leftHandedControls || Serializer.Instance.activeData.settings.movementType != Player.MovementType.Buttons)
        {
            controls = controlsLeft;
            controlsRight.SetActive(false);
        }
        else
        {
            controls = controlsRight;
            controlsLeft.SetActive(false);
        }

        //Deactivates the on screen controls if the user is playing with button or all controls
        controls.transform.GetChild(0).gameObject.SetActive(/*Serializer.activeData.settings.movementType == Player.MovementType.All ||*/ Serializer.Instance.activeData.settings.movementType == Player.MovementType.Buttons);

        //Sets the gear text
        gearText.text = Serializer.Instance.activeData.gearCount.ToString();

        //Hides shield UI
        shieldImage.gameObject.SetActive(false);
        shieldText.gameObject.SetActive(false);

        buttons = controls.transform.GetChild(0).GetComponentsInChildren<RectTransform>().ToList();
        buttons.RemoveAt(0);

        Log($"Button Count: {buttons.Count}", true);
        Log($"Names: {buttons[0].gameObject.name}, {buttons[1].gameObject.name})", true);

        shockImage = controls.transform.GetChild(1).GetComponentInChildren<Image>();
        shockText = controls.transform.GetChild(1).GetComponentInChildren<TextMeshProUGUI>();

        shockImage.gameObject.SetActive(false);
        shockText.gameObject.SetActive(false);

        slowDownImage = controls.transform.GetChild(2).GetChild(0).GetComponentInChildren<Image>();
        SlowDownCover = controls.transform.GetChild(2).GetChild(0).GetComponent<RectTransform>();


        SlowDownCover.gameObject.SetActive(false);

        LeanTween.init();
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
        //Hides the shield UI if the player has none
        if (shieldCount == 0)
        {
            shieldText.text = "0";

            //Animates the dissapearance of the shield image and text
            StartCoroutine(AnimationPlus.FadeToColor(shieldImage, new Color(0, 0, 0, 0), fadeTime, false));
            StartCoroutine(AnimationPlus.FadeText(shieldText, fadeTime));
            return;
        }

        //Animates the showing of the shield image and text if they are hidden
        if (!shieldImage.gameObject.activeInHierarchy)
        {
            shieldImage.gameObject.SetActive(true);
            shieldText.gameObject.SetActive(true);

            StartCoroutine(AnimationPlus.FadeToColor(shieldImage, new Color(1, 1, 1, 1), fadeTime));
            StartCoroutine(AnimationPlus.FadeText(shieldText, fadeTime, false));
        }

        shieldText.text = shieldCount.ToString();
    }

    //Method called to update the shield count in UI
    public void UpdateShockCount(int shockCount)
    {
        //Hides the shield UI if the player has none
        if (shockCount == 0)
        {
            shockText.text = "0";

            //Animates the dissapearance of the shield image and text
            StartCoroutine(AnimationPlus.FadeToColor(shockImage, new Color(0, 0, 0, 0), fadeTime, false));
            StartCoroutine(AnimationPlus.FadeText(shockText, fadeTime));
            return;
        }

        //Animates the showing of the shield image and text if they are hidden
        if (!shockImage.gameObject.activeInHierarchy)
        {
            shockImage.gameObject.SetActive(true);
            shockText.gameObject.SetActive(true);

            StartCoroutine(AnimationPlus.FadeToColor(shockImage, new Color(1, 1, 1, 1), fadeTime));
            StartCoroutine(AnimationPlus.FadeText(shockText, fadeTime, false));
        }

        shockText.text = shockCount.ToString();
    }

    //Used to enable the slowdown UI object
    public void EnableSlowDown(bool state, bool useFade = true, bool ignoreCurrent = false)
    {
        Log("Slowdown UI?");

        //Fade in if state is true and current acitivty is false
        if (state && (slowDownImage.color == new Color(1, 1, 1, 0) || ignoreCurrent))
        {
            Log("Enabling Slowdown UI...");
            if (useFade) StartCoroutine(AnimationPlus.FadeToColor(slowDownImage, new Color(1, 1, 1, 1), fadeTime));
            else slowDownImage.color = new Color(1, 1, 1, 1);
        }
        //Fade out if state is false and current activity is true
        else if (!state && (slowDownImage.color == new Color(1, 1, 1, 1) || ignoreCurrent))
        {
            Log("Disabling Slowdown UI...");
            if (useFade) StartCoroutine(AnimationPlus.FadeToColor(slowDownImage, new Color(1, 1, 1, 0), fadeTime));
            else slowDownImage.color = new Color(1, 1, 1, 0);
        }
    }

    //Method used to return the bounds of the control buttons
    public Vector2[] CalculateButtonBounds()
    {
        //Initializes return object
        Vector2[] output = new Vector2[8];

        float screenScalingFactor = transform.parent.localScale.x;

        output[0] = (Vector2)buttons[0].position + new Vector2(buttons[0].rect.width, buttons[0].rect.height) * screenScalingFactor / 2;
        output[1] = (Vector2)buttons[0].position - new Vector2(buttons[0].rect.width, buttons[0].rect.height) * screenScalingFactor / 2;
        output[2] = (Vector2)buttons[1].position + new Vector2(buttons[1].rect.width, buttons[1].rect.height) * screenScalingFactor / 2;
        output[3] = (Vector2)buttons[1].position - new Vector2(buttons[1].rect.width, buttons[1].rect.height) * screenScalingFactor / 2;

        RectTransform slowDown = slowDownImage.gameObject.GetComponent<RectTransform>();
        output[4] = (Vector2)slowDown.position + new Vector2(slowDown.rect.width, slowDown.rect.height) * screenScalingFactor / 2;
        output[5] = (Vector2)slowDown.position - new Vector2(slowDown.rect.width, slowDown.rect.height) * screenScalingFactor / 2;

        RectTransform shock = shockImage.gameObject.GetComponent<RectTransform>();
        output[6] = (Vector2)shock.position + new Vector2(shock.rect.width, shock.rect.height) * screenScalingFactor / 2;
        output[7] = (Vector2)shock.position - new Vector2(shock.rect.width, shock.rect.height) * screenScalingFactor / 2;

        //Returns the bounds
        return output;
    }

    public Vector2[] GetButtonBounds()
    {
        return _buttonBounds;
    }

    //Method to fade in the UI elements
    public void FadeInElements()
    {
        scoreText.color -= new Color(0, 0, 0, 1);
        gearText.color -= new Color(0, 0, 0, 1);
        gearImage.color = new Color(1, 1, 1, 0);

        StartCoroutine(AnimationPlus.FadeText(scoreText, fadeTime, false));
        StartCoroutine(AnimationPlus.FadeText(gearText, fadeTime, false));
        StartCoroutine(AnimationPlus.FadeToColor(gearImage, new Color(1, 1, 1, 1), fadeTime));
    }

    //Property to get scaling UI factor
    public float scalingFactor => transform.parent.localScale.x;
}
