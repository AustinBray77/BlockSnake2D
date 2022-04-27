using UnityEngine;

//Class to store the users settings data
[System.Serializable]
public class Settings_Data
{
    //Variables to store the users settings
    public QualityController.QualityLevel qualityLevel { get; private set; }
    public Player.MovementType movementType { get; private set; }
    public Gamemanager.Mode gamemode { get; private set; }
    public bool vsyncEnabled { get; private set; }
    public bool soundEnabled { get; private set; }
    public bool leftHandedControls { get; private set; }

    //Default contructor
    public Settings_Data(QualityController.QualityLevel _qualityLevel, Player.MovementType _movementType, bool _vsyncEnabled, bool _soundEnabled, bool _leftHandedControls, Gamemanager.Mode _gamemode)
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

        //Triggers if the quality level is in the data
        if (vals.Length >= 1)
        {
            //Converts quality level
            int.TryParse(vals[0], out int _qualityLevel);
            qualityLevel = (QualityController.QualityLevel)_qualityLevel;
        }
        else
        {
            //Else set quality level to default value
            qualityLevel = QualityController.DefaultQualityLevel(Gamemanager.Instance.CurrentPlatform);
        }

        //Triggers if the vsync enabled is in the data
        if (vals.Length >= 2)
        {
            //Converts vsync enabled
            vsyncEnabled = vals[1].ToLower() == "true";
        }
        else
        {
            //Else set vsync enabled to defualt value
            vsyncEnabled = false;
        }

        //Triggers if the sound enabled is in the data 
        if (vals.Length >= 3)
        {
            //Converts sound enabled
            soundEnabled = vals[2].ToLower() == "true";
        }
        else
        {
            //Else set sound enabled to default value
            soundEnabled = true;
        }

        //Triggers if movement type is in data
        if (vals.Length >= 4)
        {
            //Converts movement type
            int.TryParse(vals[3], out int _movementType);
            movementType = (Player.MovementType)_movementType;
        }
        else
        {
            //Else set movement type to base movement type
            movementType = Player.DefaultMovementType(Gamemanager.Instance.CurrentPlatform);
        }

        //Triggers if the left handed controls is in the data
        if (vals.Length >= 5)
        {
            //Converts left handed controls
            leftHandedControls = vals[4].ToLower() == "true";
        }
        else
        {
            //Else set left handed controls to base value
            leftHandedControls = false;
        }

        //Triggers if the gamemode is in the data
        if (vals.Length >= 6)
        {
            //Converts movement type
            int.TryParse(vals[5], out int _gamemode);
            gamemode = (Gamemanager.Mode)_gamemode;
        }
        else
        {
            //Else set movement type to base movement type
            gamemode = Gamemanager.Mode.Normal;
        }
    }

    //Public setters for instance variables
    #region Setters
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
    #endregion

    //ToString Method
    public override string ToString()
    {
        /* Format: qualityLevel(int) vsyncEnabled(bool) soundEnabled(bool), movementType(int) leftHandedControls(bool) */
        return (int)qualityLevel + " " + vsyncEnabled + " " + soundEnabled + " " + (int)movementType + " " + leftHandedControls;
    }

    //Method for implicit conversion
    public static implicit operator Settings_Data(string s) => new Settings_Data(s);
}