using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//Class to control the gamemode of the game
public class Gamemode : BaseBehaviour
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
    [SerializeField] private Mode _mode;

#if UNITY_EDITOR
    public static Platform platform = Platform.Debug;
#elif UNITY_STANDALONE_WIN
    public static Platform platform = Platform.Windows;
#elif UNITY_IOS
    public static Platform platform = Platform.IOS;
#else
    public static Platform platform = Platform.Android;
#endif

    public static Mode mode;

    //Method called on scene load
    private void Start()
    {
        //Assigns default values
        mode = _mode;
    }

    //Returns if in the specified level
    public static bool inLevel(string levelName) =>
        SceneManager.GetActiveScene().name == levelName;

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
}
