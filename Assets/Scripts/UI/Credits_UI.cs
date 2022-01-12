using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Credits_UI : UI
{
    //Called when the user clicks to go to the start menu
    public void Click_StartMenu()
    {
        //Switches to the Start UI with fade
        StartCoroutine(ClickWithFade(
            () =>
            {
                Refrence.startUI.Show();
                Hide();
            }, fadeTime));
    }
}
