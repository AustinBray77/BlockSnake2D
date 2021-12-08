using UnityEngine;

//Class to store the users settings data
[System.Serializable]
public class Settings_Data
{
    //Variables to store the users settings
    public QualityController.QualityLevel qualityLevel { get; private set; }
    public bool vsyncEnabled { get; private set; }
    public bool soundEnabled { get; private set; }

    //Default contructor
    public Settings_Data(QualityController.QualityLevel _qualityLevel, bool _vsyncEnabled, bool _soundEnabled)
    {
        //Assigns the instance values
        qualityLevel = _qualityLevel;
        vsyncEnabled = _vsyncEnabled;
        soundEnabled = _soundEnabled;
    }

    //Public setters for instance variables
    public void SetQualityLevel(QualityController.QualityLevel _qualityLevel) =>
        qualityLevel = _qualityLevel;

    public void EnableVsync(bool enable)
    {
        //Enables vsync in unity and saves the value
        QualitySettings.vSyncCount = enable ? 1 : 0;
        vsyncEnabled = enable;
    }

    public void EnableSound(bool enable) =>
        soundEnabled = enable;
}