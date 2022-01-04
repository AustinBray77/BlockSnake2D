using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

//Class to control the game UI
public class Game_UI : UI
{
    //Object refrence to the controls
    [SerializeField] private GameObject controls;

    //Refrence to the score text
    [SerializeField] private TextMeshProUGUI scoreText;

    //Refrence to the gear count text and Image
    [SerializeField] private TextMeshProUGUI gearText;
    [SerializeField] private Image gearImage;

    //Refrence to the shield image and count
    [SerializeField] private Image shieldImage;
    [SerializeField] private TextMeshProUGUI shieldText;

    //Refrence to the slowdown image
    [SerializeField] private Image slowDownImage;

    //Refrence to parent canvas
    [SerializeField] private Canvas canvas;

    //Refrence to the camera in the scene
    [SerializeField] private Camera cam;

    //Gets the buttons
    List<RectTransform> buttons;

    Vector2[] buttonBounds = null;

    //Called when the scene is loaded
    private void Start()
    {
        //Deactivates the on screen controls if the user is playing with button or all controls
        controls.SetActive(/*Serializer.activeData.settings.movementType == Player.MovementType.All ||*/ Serializer.activeData.settings.movementType == Player.MovementType.Buttons);

        //Sets the gear text
        gearText.text = Serializer.activeData.gearCount.ToString();

        //Hides shield UI
        shieldImage.gameObject.SetActive(false);
        shieldText.gameObject.SetActive(false);

        buttons = controls.GetComponentsInChildren<RectTransform>().ToList();
        buttons.RemoveAt(0);

        buttonBounds = CalculateButtonBounds();

        LeanTween.init();
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

    //Used to enable the slowdown UI object
    public void EnableSlowDown(bool state, bool useFade = true)
    {
        //Fade in if state is true and current acitivty is false
        if (state && slowDownImage.color == new Color(1, 1, 1, 0))
        {
            if (useFade) StartCoroutine(AnimationPlus.FadeToColor(slowDownImage, new Color(1, 1, 1, 1), fadeTime));
            else slowDownImage.color = new Color(1, 1, 1, 1);
        }
        //Fade out if state is false and current activity is true
        else if (!state && slowDownImage.color == new Color(1, 1, 1, 1))
        {
            if (useFade) StartCoroutine(AnimationPlus.FadeToColor(slowDownImage, new Color(1, 1, 1, 0), fadeTime));
            else slowDownImage.color = new Color(1, 1, 1, 0);
        }
    }

    //Method used to return the bounds of the control buttons
    public Vector2[] CalculateButtonBounds()
    {
        //Initializes return object
        Vector2[] output = new Vector2[6];

        float screenScalingFactor = transform.parent.localScale.x;

        output[0] = (Vector2)buttons[0].position + new Vector2(buttons[0].rect.width, buttons[0].rect.height) * screenScalingFactor / 2;
        output[1] = (Vector2)buttons[0].position - new Vector2(buttons[0].rect.width, buttons[0].rect.height) * screenScalingFactor / 2;
        output[2] = (Vector2)buttons[1].position + new Vector2(buttons[1].rect.width, buttons[1].rect.height) * screenScalingFactor / 2;
        output[3] = (Vector2)buttons[1].position - new Vector2(buttons[1].rect.width, buttons[1].rect.height) * screenScalingFactor / 2;

        RectTransform slowDown = slowDownImage.gameObject.GetComponent<RectTransform>();
        output[4] = (Vector2)slowDown.position + new Vector2(slowDown.rect.width, slowDown.rect.height) * screenScalingFactor / 2;
        output[5] = (Vector2)slowDown.position - new Vector2(slowDown.rect.width, slowDown.rect.height) * screenScalingFactor / 2;

        //Returns the bounds
        return output;
    }

    public Vector2[] GetButtonBounds()
    {
        return buttonBounds;
    }

    //Method to fade in the UI elements
    public void FadeInElements()
    {
        scoreText.color -= new Color(0, 0, 0, 1);
        gearText.color -= new Color(0, 0, 0, 1);
        gearImage.color = new Color(1, 1, 1, 0);

        StartCoroutine(AnimationPlus.FadeText(scoreText, UI.fadeTime, false));
        StartCoroutine(AnimationPlus.FadeText(gearText, UI.fadeTime, false));
        StartCoroutine(AnimationPlus.FadeToColor(gearImage, new Color(1, 1, 1, 1), UI.fadeTime));
    }

    //Property to get scaling UI factor
    public float scalingFactor => transform.parent.localScale.x;
}
