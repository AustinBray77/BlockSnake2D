using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class Death_UI : UI
{
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private LevelBar levelBar;
    [SerializeField] private Image fadePanel;

    [System.Serializable]
    private class LevelBar
    {
        public TMP_Text lowerLevel, higherLevel;
        public Slider levelBar;
    }

    public void OnLoad()
    {
        scoreText.text = $"FINAL SCORE: {Player.score}\nHIGHSCORE: {Serializer.activeData.highScore}";
        levelBar.lowerLevel.text = Serializer.activeData.level.level.ToString();
        levelBar.higherLevel.text = (Serializer.activeData.level.level + 1).ToString();
        levelBar.levelBar.minValue = Level.levelToXP(Serializer.activeData.level.level);
        levelBar.levelBar.maxValue = Level.levelToXP(Serializer.activeData.level.level + 1);
        levelBar.levelBar.value = Serializer.activeData.level.xp;

        Serializer.SaveData();
    }

    public void ClickRespawn()
    {
        Refrence.adManager.ShowAd();
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
        StartCoroutine(ClickWithFade(() =>
        {
            SceneManager.LoadScene("Main", LoadSceneMode.Single);
        }, 1f, fadePanel));
    }

    public void OnMainMenu()
    {
        StartCoroutine(ClickWithFade(() =>
        {
            SceneManager.LoadScene("Start", LoadSceneMode.Single);
        }, 1f, fadePanel));
    }
}
