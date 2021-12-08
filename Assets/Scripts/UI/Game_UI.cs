using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

//Class to control the game UI
public class Game_UI : UI
{
    //Object refrence to the controls
    [SerializeField] private GameObject controls;

    //Refrence to the score text
    [SerializeField] private TextMeshProUGUI scoreText;

    //Refrence to the gear count text
    [SerializeField] private TextMeshProUGUI gearText;

    //Refrence to the shield image and count
    [SerializeField] private Image shieldImage;
    [SerializeField] private TextMeshProUGUI shieldText;

    //Called when the scene is loaded
    private void Start()
    {
        //Deactivates the on screen controls if the user is playing the windows version of the game
        controls.SetActive(Gamemode.platform != Gamemode.Platform.Windows);

        //Sets the gear text
        gearText.text = Serializer.activeData.gearCount.ToString();

        //Hides shield UI
        shieldImage.gameObject.SetActive(false);
        shieldText.gameObject.SetActive(false);
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
        if(shieldCount == 0)
        {
            shieldText.text = "0";

            //Animates the dissapearance of the shield image and text
            StartCoroutine(AnimationPlus.FadeToColor(shieldImage, new Color(0, 0, 0, 0), 1f, false));
            StartCoroutine(AnimationPlus.FadeText(shieldText, 1f));
            return;
        }

        //Animates the showing of the shield image and text if they are hidden
        if (!shieldImage.gameObject.activeInHierarchy)
        {
            shieldImage.gameObject.SetActive(true);
            shieldText.gameObject.SetActive(true);

            StartCoroutine(AnimationPlus.FadeToColor(shieldImage, new Color(1, 1, 1, 1), 1f));
            StartCoroutine(AnimationPlus.FadeText(shieldText, 1f, false));
        }

        shieldText.text = shieldCount.ToString();
    }
}
