using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ModeSelect_UI : UI<ModeSelect_UI>
{
    public void SetMode(int index)
    {
        Gamemanager.Instance.CurrentMode = (Gamemanager.Mode)index;

        StartCoroutine(ClickWithFade(
            () =>
            {
                SceneManager.LoadScene("Main", LoadSceneMode.Single);
            }, fadeTime, true));
    }
}
