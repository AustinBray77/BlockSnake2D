using UnityEngine.UI;
using TMPro;

//Serializable class to store a refrence to a level bar
[System.Serializable]
public class LevelBar
{
    //Stores the references to the Lower and Upper level text boxes
    public TMP_Text lowerLevel, higherLevel;

    //Stores the reference to the slider
    public Slider levelBar;
}