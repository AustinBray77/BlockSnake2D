using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//Class to control the gamemode of the game
public class Gamemanager : SingletonDD<Gamemanager>
{
    //Enum to store the different the platforms
    [System.Serializable]
    public enum Platform
    {
        Debug,
        Windows,
        Android,
        IOS
    }

    [System.Serializable]
    public enum Mode
    {
        Normal,
        Fast
    }

    //Properties to store the current mode
    public Mode CurrentMode;
    public Skin[] Skins;
    public List<Level.LevelUpTrigger> LevelUpTriggers = null;

#if UNITY_EDITOR
    public Platform CurrentPlatform = Platform.Debug;
#elif UNITY_STANDALONE_WIN
    public Platform CurrentPlatform = Platform.Windows;
#elif UNITY_IOS
    public Platform CurrentPlatform = Platform.IOS;
#else
    public Platform CurrentPlatform = Platform.Android;
#endif

    //Returns if in the specified level
    public static bool InLevel(string levelName) =>
        SceneManager.GetActiveScene().name == levelName;

    //Returns the multiplier for the current mode
    public float ModeMultiplier()
    {
        return ModeMultiplier(CurrentMode);
    }

    //Returns the multiplier for each mode
    public static float ModeMultiplier(Mode mode)
    {
        switch (mode)
        {
            case Mode.Fast:
                return 2f;
            default:
                return 1f;
        }
    }

    private new void Awake()
    {
        Log("Call Awake");
        base.Awake();
        Log("Finished Call");

        if (LevelUpTriggers == null)
        {
            List<Level.LevelUpTrigger> LevelUpTriggers = new List<Level.LevelUpTrigger>();
        }

        //Generates the level triggers
        if (LevelUpTriggers.Count != 100)
        {
            int gears = 3, skinIndex = 0;

            for (int i = 1; i <= 100; i++)
            {
                if (i % 5 == 0)
                {
                    gears++;
                }

                bool unlocksSkin = false;

                if (skinIndex < Skins.Length)
                {
                    while (Skins[skinIndex].levelRequirement <= i)
                    {
                        skinIndex++;
                        unlocksSkin = true;

                        if (skinIndex >= Skins.Length)
                        {
                            break;
                        }
                    }
                }

                LevelUpTriggers.Add(new Level.LevelUpTrigger(i, gears, unlocksSkin));
            }
        }
    }
}
