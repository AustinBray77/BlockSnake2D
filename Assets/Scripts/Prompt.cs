using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

//Class to control UI prompts
public class Prompt : BaseBehaviour
{
    //Stores the title UI element
    [SerializeField] private TextMeshProUGUI title;

    //Stores the action and exit button UI elements
    [SerializeField] private Button actionBtn, exitBtn;

    //Stores the list of description UI elements
    [SerializeField] private List<TextMeshProUGUI> descs;

    //Stores the list of image UI elements
    [SerializeField] private List<Image> images;

    //Method called when the object is instantiation
    public void OnAwake()
    {
        //Sets the size to 0
        transform.localScale = new Vector3(0, 0);

        //Animates the prompt to full size
        gameObject.LeanScale(new Vector3(1, 1), UI.fadeTime);
    }

    //Method to add an action for when the prompt is clicked
    public void AddButtonListener(UnityEngine.Events.UnityAction action)
    {
        //Adds the action to the action button
        actionBtn.onClick.AddListener(action);
    }

    //Method to set the images 
    public void SetImages(List<Sprite> sprites)
    {
        //Loops for each sprite in the list or total image elements on the prompt
        for (int i = 0; i < images.Count && i < sprites.Count; i++)
        {
            //Assigns the sprite of each image to the passed sprite
            images[i].sprite = sprites[i];
        }
    }

    //Method to set the title text
    public void SetTitleText(string text, int size = 0)
    {
        //Sets the title text
        title.text = text;

        //If the provided size is not 0
        if (size != 0)
        {
            //Sets the font size
            title.fontSize = size;
        }
    }

    //Method to set the descriptions text
    public void SetDescriptions(string[] texts, int[] sizes = null)
    {
        //Loops for each string in the texts or total descriptions elements on the prompt
        for (int i = 0; i < descs.Count && i < texts.Length; i++)
        {
            //Sets the text of the description to the passed string
            descs[i].text = texts[i];

            //Trigger if the sizes array is passed
            if (sizes != null)
            {
                //Trigger if the current index has an associated size
                if (sizes.Length > i)
                {
                    //Sets the font size to the passed size
                    descs[i].fontSize = sizes[i];
                }
            }
        }
    }

    //Method called to cause the prompt to close on click
    public void ToCloseOnClick()
    {
        //Sets the action button on click to the exit buttons on click event
        actionBtn.onClick = exitBtn.onClick;
    }

    //Method called when the user clicks to close the prompt
    public void OnClickClose()
    {
        //Closes the prompt
        StartCoroutine(ClosePrompt());
    }

    //Method to close the prompt
    public IEnumerator ClosePrompt()
    {
        //Animations the prompts dissappearance
        gameObject.LeanScale(new Vector3(0, 0), UI.fadeTime);

        //Waits for the prompt to dissappear
        yield return new WaitForSeconds(UI.fadeTime);

        //Destoys the prompt
        Destroy(gameObject);
    }

    //Method alled to force the player to interact with the prompt
    public void ForceInteraction()
    {
        //Deactivates the close button
        exitBtn.gameObject.SetActive(false);
    }
}
