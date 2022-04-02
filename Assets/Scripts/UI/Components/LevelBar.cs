using UnityEngine.UI;
using TMPro;

//Serializable class to store a refrence to a level bar
[System.Serializable]
public class LevelBar
{
    public TMP_Text lowerLevel, higherLevel;
    public Slider levelBar;
}