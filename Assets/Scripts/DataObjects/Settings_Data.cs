using UnityEngine;

//Class to store the users settings data
[System.Serializable]
public class Settings_Data
{
    //Variables to store the users settings
    public QualityController.QualityLevel qualityLevel { get; private set; }
    public Player.MovementType movementType { get; private set; }
    public Gamemode.Mode gamemode { get; private set; }
    public bool vsyncEnabled { get; private set; }
    public bool soundEnabled { get; private set; }
    public bool leftHandedControls { get; private set; }

    //Default contructor
    public Settings_Data(QualityController.QualityLevel _qualityLevel, Player.MovementType _movementType, bool _vsyncEnabled, bool _soundEnabled, bool _leftHandedControls, Gamemode.Mode _gamemode)
    {
        //Assigns the instance values
        qualityLevel = _qualityLevel;
        movementType = _movementType;
        vsyncEnabled = _vsyncEnabled;
        soundEnabled = _soundEnabled;
        leftHandedControls = _leftHandedControls;
        gamemode = _gamemode;
    }

    //Constructor from string
    public Settings_Data(string data)
    {
        //Seperates data
        string[] vals = data.Split(' ');

        //If statements are to prevent index out of bounds error, if the data is too short the data is set to a defualt value

        if (vals.Length >= 1)
        {
            //Converts quality level
            int.TryParse(vals[0], out int _qualityLevel);
            qualityLevel = (QualityController.QualityLevel)_qualityLevel;
        }
        else
        {
            //Set quality level to default value
            qualityLevel = QualityController.DefaultQualityLevel(Gamemode.platform);
        }

        if (vals.Length >= 2)
        {
            //Converts vsync enabled
            vsyncEnabled = vals[1].ToLower() == "true";
        }
        else
        {
            //Set vsync enabled to defualt value
            vsyncEnabled = false;
        }

        if (vals.Length >= 3)
        {
            //Converts sound enabled
            soundEnabled = vals[2].ToLower() == "true";
        }
        else
        {
            //Set sound enabled to default value
            soundEnabled = true;
        }

        if (vals.Length >= 4)
        {
            //Converts movement type
            int.TryParse(vals[3], out int _movementType);
            movementType = (Player.MovementType)_movementType;
        }
        else
        {
            //Set movement type to base movement type
            movementType = Player.DefaultMovementType(Gamemode.platform);
        }

        if (vals.Length >= 5)
        {
            //Converts left handed controls
            leftHandedControls = vals[4].ToLower() == "true";
        }
        else
        {
            //Set left handed controls to base value
            leftHandedControls = false;
        }

        if (vals.Length >= 6)
        {
            //Converts movement type
            int.TryParse(vals[5], out int _gamemode);
            gamemode = (Gamemode.Mode)_gamemode;
        }
        else
        {
            //Set movement type to base movement type
            gamemode = Gamemode.Mode.Normal;
        }
    }

    //Public setters for instance variables
    public void SetQualityLevel(QualityController.QualityLevel _qualityLevel) =>
        qualityLevel = _qualityLevel;

    public void SetMovementType(Player.MovementType _movementType) =>
        movementType = _movementType;

    public void EnableVsync(bool enable)
    {
        //Enables vsync in unity and saves the value
        QualitySettings.vSyncCount = enable ? 1 : 0;
        vsyncEnabled = enable;
    }

    public void EnableSound(bool enable) =>
        soundEnabled = enable;

    public void EnableLeftHanded(bool enable) =>
        leftHandedControls = enable;

    //ToString Method
    public override string ToString()
    {
        return (int)qualityLevel + " " + vsyncEnabled + " " + soundEnabled + " " + (int)movementType + " " + leftHandedControls;
    }
}