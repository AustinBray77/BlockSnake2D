using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

//Class to contorl the start UI
public class Start_UI : UI
{
    [SerializeField] private GameObject loadingText;

    //Called on object instation, before start
    public void Awake()
    {
        //Loads the data if no save data is currently present
        if (Serializer.activeData == null)
        {
            //Shows then hides the loading text
            loadingText.SetActive(true);
            Serializer.LoadData();
            loadingText.SetActive(false);
        }

        //Fades in
        fadePanel.gameObject.SetActive(true);
        fadePanel.color = new Color(0, 0, 0, 1);
        StartCoroutine(AnimationPlus.FadeToColor(fadePanel, new Color(0, 0, 0, 0), fadeTime, false));
    }

    //Functions which are called on botton clicks

    //Called when the user clicks to start
    public void Click_Start()
    {
        //Loads the main scene with fade
        StartCoroutine(ClickWithFade(
            () =>
            {
                SceneManager.LoadScene("Main", LoadSceneMode.Single);
            }, fadeTime, true));
    }

    //Called when the user clicks to the shop
    public void Click_Shop()
    {
        //Loads the shop UI with fade
        StartCoroutine(ClickWithFade(
            () =>
            {
                Refrence.shopUI.Show();
                Hide();
            }, fadeTime));
    }

    //Called when the user clicks to enter the tutorial
    public void Click_Tutorial()
    {
        //Loads the tutorial scene with fade
        StartCoroutine(ClickWithFade(
             () =>
             {
                 SceneManager.LoadScene("Tutorial", LoadSceneMode.Single);
             }, fadeTime, true));
    }

    //Called when the user clicks to enter the settings
    public void Click_Settings()
    {
        //Loads the settings scree with fade
        StartCoroutine(ClickWithFade(
            () =>
            {
                Refrence.settingsUI.Show();
                Hide();
            }, fadeTime));
    }

    //Called when the user clicks to quit
    public void Click_Quit()
    {
        //Saves the data and exits the application
        Serializer.SaveData();
        Application.Quit();
    }
}
