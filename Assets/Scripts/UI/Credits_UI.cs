using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Class to control the UI on the credits screen
public class Credits_UI : UI<Credits_UI>
{
    //Called when the user clicks to go to the start menu
    public void Click_StartMenu()
    {
        //Activates the following method with a fade
        StartCoroutine(ClickWithFade(
            () =>
            {
                //Shows the start UI
                Start_UI.Instance.Show();

                //Hides current UI
                Hide();
            },
            //Lasts standard fade time and flags that the action does not change scenes
            fadeTime, false));
    }
}
