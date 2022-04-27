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

    //Enum to store the game modes that the game has
    [System.Serializable]
    public enum Mode
    {
        Normal,
        Fast
    }

    //Stores the current gamemode
    public Mode CurrentMode;

    //Stores the skins
    public Skin[] Skins;

    //Stores the level up triggers
    public List<Level.LevelUpTrigger> LevelUpTriggers = null;

    //Sets the platform variable based on the unity platform
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

    //Method to get the rewards multiplier for the current mode
    public float ModeMultiplier() =>
        ModeMultiplier(CurrentMode);

    //Method to get the rewards multiplier for a given mode
    public static float ModeMultiplier(Mode mode)
    {
        //Switches the given mode
        switch (mode)
        {
            //Trigger if the current mode is fast
            case Mode.Fast:
                return 2f;
            //Trigger if the current mode is normal
            default:
                return 1f;
        }
    }

    //Method called on object instantiation
    private new void Awake()
    {
        base.Awake();

        //Triggers if the level up triggers is null
        if (LevelUpTriggers == null)
        {
            //Initializes the level up triggers
            List<Level.LevelUpTrigger> LevelUpTriggers = new List<Level.LevelUpTrigger>();
        }

        //Triggers if the level up triggers is not at 100
        if (LevelUpTriggers.Count != 100)
        {
            //Starts gears rewards at 3, skin index at 0
            int gears = 3, skinIndex = 0;

            //Loops 100 times to create level up triggers
            for (int i = 1; i <= 100; i++)
            {
                //Triggers if the trigger is a multiple of 5
                if (i % 5 == 0)
                {
                    //Increase the gear reward
                    gears++;
                }

                //Stores if the current skin is unlocked
                bool unlocksSkin = false;

                //Triggers if skin index is less than amount of skins
                if (skinIndex < Skins.Length)
                {
                    //Loops while the skin's level requirement is less than the current index (level of level up trigger)
                    while (Skins[skinIndex].levelRequirement <= i)
                    {
                        //Increment level up trigger and flag that a skin is unlocked
                        skinIndex++;
                        unlocksSkin = true;

                        //If the skins index is out of bounds, break
                        if (skinIndex >= Skins.Length)
                        {
                            break;
                        }
                    }
                }

                //Creates the trigger and adds it to the list
                LevelUpTriggers.Add(new Level.LevelUpTrigger(i, gears, unlocksSkin));
            }
        }
    }
}
