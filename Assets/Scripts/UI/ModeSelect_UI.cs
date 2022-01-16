using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ModeSelect_UI : UI
{
    public void SetMode(int index)
    {
        Gamemode.mode = (Gamemode.Mode)index;

        StartCoroutine(ClickWithFade(
            () =>
            {
                SceneManager.LoadScene("Main", LoadSceneMode.Single);
            }, fadeTime, true));
    }
}
