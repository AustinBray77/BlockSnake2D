using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//Class to control the UI for choosing a gamemode
public class ModeSelect_UI : UI<ModeSelect_UI>
{
    //Method to set the mode
    public void SetMode(int index)
    {
        //Converts the mode index into Mode enum
        Gamemanager.Instance.CurrentMode = (Gamemanager.Mode)index;

        //Activates the following method with a fade
        StartCoroutine(ClickWithFade(
            () =>
            {
                //Load main scene
                SceneManager.LoadScene("Main", LoadSceneMode.Single);
            },
            //Fade for fade time and flag that the method switches scenes
            fadeTime, true));
    }
}
