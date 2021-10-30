using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

//Class to contorl the start UI
public class Start_UI : UI
{
    //Called when the scene is loaded
    public void Start()
    {
        //Fades in
        StartCoroutine(AnimationPlus.FadeToColor(fadePanel, new Color(0, 0, 0, 0), fadeTime, false));

        //Loads the data if no save data is currently present
        if (Serializer.activeData == null)
            Serializer.LoadData();

        //Starts the shop UI
        Refrence.shopUI.DelayedStart();
    }

    //Functions which are called on botton clicks

    //Called when the user clicks to start
    public void Click_Start()
    {
        //Loads the main scene with fade
        StartCoroutine(ClickWithFade(
            () => {
                SceneManager.LoadScene("Main", LoadSceneMode.Single);
            }, fadeTime, true));
    }

    //Called when the user clicks to the shop
    public void Click_Shop()
    {
        //Loads the shop UI with fade
        StartCoroutine(ClickWithFade(
            () => {
                Refrence.shopUI.Show();
                Hide();
            }, fadeTime));
    }

    //Called when the user clicks to enter the tutorial
    public void Click_Tutorial()
    {
        //Loads the tutorial scene with fade
        StartCoroutine(ClickWithFade(
             () => {
                 SceneManager.LoadScene("Tutorial", LoadSceneMode.Single);
             }, fadeTime, true));
    }

    //Called when the user clicks to quit
    public void Click_Quit()
    {
        //Saves the data and exits the application
        Serializer.SaveData();
        Application.Quit();
    }
}
