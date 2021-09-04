using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class Death_UI : MonoBehaviour
{
    [SerializeField] private TMP_Text scoreText;

    public void OnLoad(int score)
    {
        scoreText.text = $"Final Score: {score}";
    }

    public void ClickRespawn()
    {
        //Refrence.adManager.ShowAd();
        OnRespawn();
    }

    public void OnRespawn()
    {
        Refrence.player.Respawn();
        Refrence.gen.Respawn();
        gameObject.SetActive(false);
    }

    public void OnRestart()
    {
        SceneManager.LoadScene("Main", LoadSceneMode.Single);
    }

    public void OnMainMenu()
    {
        SceneManager.LoadScene("Start", LoadSceneMode.Single);
    }
}
