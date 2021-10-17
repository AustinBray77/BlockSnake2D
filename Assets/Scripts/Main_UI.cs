using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Main_UI : UI
{
    [SerializeField] private GameObject startUI;
    [SerializeField] private Shop_UI _shopUI;
    [SerializeField] private Image fadePanel;

    public static Shop_UI shopUI;
    public static Skin baseSkin;

    public void Start()
    {
        StartCoroutine(AnimationPlus.FadeToColor(fadePanel, new Color(0, 0, 0, 0), 1f, false));

        shopUI = _shopUI;

        if (Serializer.activeData == null)
            Serializer.LoadData();

        shopUI.Start();
    }

    public void Click_Start()
    {
        StartCoroutine(ClickWithFade(
            () => {
                SceneManager.LoadScene("Main", LoadSceneMode.Single);
            }, 1f, fadePanel));
    }

    public void Click_Shop()
    {
        StartCoroutine(ClickWithFade(
            () => {
                startUI.SetActive(false);
                _shopUI.gameObject.SetActive(true);
                _shopUI.Show();
            }, 1f, fadePanel));
    }

    public void Click_StartMenu()
    {
        StartCoroutine(ClickWithFade(
            () => {
                _shopUI.Hide();
                startUI.gameObject.SetActive(true);
            }, 1f, fadePanel));
    }

    public void Click_Quit()
    {
        Serializer.SaveData();
        Application.Quit();
    }
}
