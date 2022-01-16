using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//Class to control the gamemode of the game
public class Gamemode : MonoBehaviour
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

    //Properties to store the current platform
    [SerializeField] private Platform _platform;
    [SerializeField] private Mode _mode;

    public static Platform platform;
    public static Mode mode;

    //Method called on scene load
    private void Start()
    {
        //Assigns the static refrence of Platform
        platform = _platform;
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
