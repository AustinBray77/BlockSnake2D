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

    //Properties to store the current platform
    [SerializeField] private Platform _platform;

    public static Platform platform;

    //Method called on scene load
    private void Start()
    {
        //Assigns the static refrence of Platform
        platform = _platform;
    }

    //Returns if in the specified level
    public static bool inLevel(string levelName) =>
        SceneManager.GetActiveScene().name == levelName;
}
