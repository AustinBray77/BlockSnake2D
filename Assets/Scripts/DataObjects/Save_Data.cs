using System;
using System.Linq;
using System.Collections.Generic;

//Wrapper class for saving users data to the System
[Serializable]
public class Save_Data
{
    //Properties to store the active skin, purchased skins, level, gearcount, highscore, and settings
    public int activeSkin { get; private set; }
    public HashSet<string> purchasedSkins { get; private set; }
    public Level level { get; private set; }
    public int gearCount { get; private set; }
    public int highScore { get; private set; }
    public Settings_Data settings { get; private set; }
    public bool[] activatedLevelTriggers { get; private set; }
    public long lastRewardTime { get; private set; }
    public int lastReward { get; private set; }
    public int daysSinceLastReward { get; private set; }

    //Default constructor
    public Save_Data(int _activeSkin, HashSet<string> _purchasedSkins, Level _level, int _gearCount, int _highScore, Settings_Data _settings, bool[] _activatedLevelTriggers,
        long _lastRewardTime, int _lastReward)
    {
        //Assigns all inputs to the corresponding properties
        activeSkin = _activeSkin;
        purchasedSkins = _purchasedSkins;
        level = _level;
        gearCount = _gearCount;
        highScore = _highScore;
        settings = _settings;
        activatedLevelTriggers = _activatedLevelTriggers;
        lastRewardTime = _lastRewardTime;
        lastReward = _lastReward;
    }

    //Constructor with string data for parsing
    public Save_Data(string data)
    {
        //Parses the given data
        ParseData(data);
    }

    //Constructor with string array data for parsing
    public Save_Data(string[] data)
    {
        //Parses the given data
        ParseData(data);
    }

    //Method to parse a parameter from saved data to a given type
    public T ParseParameter<T>(string[] vals, int index)
    {
        //Trigger if the index is in bounds
        if (index < vals.Length && index >= 0)
        {
            //Return the value at the index converted to the given type
            return Functions.TryGenericConversion<string, T>(vals[index]);
        }

        //Else return the default for the given type
        return default(T);
    }

    //Method to parse the data from a string
    public void ParseData(string data)
    {
        UnityEngine.Debug.Log("Parsing Data");

        //Splits data
        string[] vals = data.Split('\n');

        //If statements are to prevent index out of bounds error, if the data is too short the data is set to a defualt value
        activeSkin = ParseParameter<int>(vals, 0);

        //Initializes the purchased skins hash set
        purchasedSkins = new HashSet<string>();

        //Triggers if the purchased skins is in data
        if (vals.Length >= 2)
        {
            //Converts purchased skins
            string[] purchasedSkinsStrs = vals[1].Split(' ');

            //Loops through each bool value in the line
            for (int i = 0; i < purchasedSkinsStrs.Length && i < Shop_UI.skinCount; i++)
            {
                //Trigger if the player has purchased the skin
                if (purchasedSkinsStrs[i].ToLower() == "true")
                {
                    //Adds the skin name
                    purchasedSkins.Add(Gamemanager.Instance.Skins[i].title);
                }
            }
        }
        else
        {
            //Else only the first skin is purchased
            purchasedSkins.Add(Gamemanager.Instance.Skins[0].title);
        }

        //Triggers if the level data is in the data
        if (vals.Length >= 3)
        {
            //Converts the level implicitly
            level = vals[2];
        }
        else
        {
            //Else sets level to base value, level with 0 xp
            level = new Level(0);
        }

        //Convert the 4th line to gearCount
        gearCount = ParseParameter<int>(vals, 3);

        //Convert the 5th line to the highScore
        highScore = ParseParameter<int>(vals, 4);

        //Triggers if the settings data is in the data
        if (vals.Length >= 6)
        {
            //Converts settings data using string constructor
            settings = new Settings_Data(vals[5]);
        }
        else
        {
            //Else sets settings to base value
            settings = new Settings_Data("");
        }

        //Initializes activated level triggers
        activatedLevelTriggers = new bool[Gamemanager.Instance.LevelUpTriggers.Count];

        //Triggers if the activated level triggers is in the data
        if (vals.Length >= 7)
        {
            //Gets each bool value from the line
            string[] activatedTriggerStrings = vals[6].Split(' ');

            //Loops for each of the bool values
            for (int i = 0; i < activatedLevelTriggers.Length && i < activatedTriggerStrings.Length; i++)
            {
                //If the bool is true, set trigger to activated
                activatedLevelTriggers[i] = activatedTriggerStrings[i].ToLower() == "true";
            }

            //Sets unknowns to false
            for (int i = activatedTriggerStrings.Length; i < activatedLevelTriggers.Length; i++)
            {
                activatedLevelTriggers[i] = false;
            }
        }

        //Convert the 8th line to last reward time
        lastRewardTime = ParseParameter<long>(vals, 7);

        //Trigger if last reward time is 0 (Unix)
        if (lastRewardTime == 0)
        {
            //Set last reward time to today
            lastRewardTime = Functions.CurrentTimeInMillis() + (3600000 * Functions.TimezoneOffset());
        }

        //Convert the 9th line to last reward
        lastReward = ParseParameter<int>(vals, 8);
    }

    //Method to parse the data from a string array
    private void ParseData(string[] data)
    {
        //Convert the 1st line to active skin 
        activeSkin = ParseParameter<int>(data, 0);

        //Triggers if purchased skins is in the data
        if (data.Length >= 2)
        {
            //Set the purchased skins to the line split by each space
            purchasedSkins = new HashSet<string>(data[1].Split(' '));
        }
        else
        {
            //Set the purchased skins to just the first line
            purchasedSkins = new HashSet<string>(new string[] { "White" });
        }

        //Triggers if the level data is in the data
        if (data.Length >= 3)
        {
            //Converts the level implicitly
            level = data[2];
        }
        else
        {
            //Else sets level to base value, level with 0 xp
            level = new Level(0);
        }

        //Convert the 4th line to gearCount
        gearCount = ParseParameter<int>(data, 3);

        //Convert the 5th line to the highScore
        highScore = ParseParameter<int>(data, 4);

        //Triggers if the settings data is in the data
        if (data.Length >= 6)
        {
            //Converts settings data using string constructor
            settings = new Settings_Data(data[5]);
        }
        else
        {
            //Else sets settings to base value
            settings = new Settings_Data("");
        }

        //Initializes activated level triggers
        activatedLevelTriggers = new bool[Gamemanager.Instance.LevelUpTriggers.Count];

        if (data.Length >= 7)
        {
            //Gets each bool value from the line
            string[] activatedTriggerStrings = data[6].Split(' ');

            //Loops for each of the bool values
            for (int i = 0; i < activatedLevelTriggers.Length && i < activatedTriggerStrings.Length; i++)
            {
                //If the bool is true, set trigger to activated
                activatedLevelTriggers[i] = activatedTriggerStrings[i].ToLower() == "true";
            }

            //Sets unknowns to false
            for (int i = activatedTriggerStrings.Length; i < activatedLevelTriggers.Length; i++)
            {
                activatedLevelTriggers[i] = false;
            }
        }

        //Convert the 8th line to last reward time
        lastRewardTime = ParseParameter<long>(data, 7);

        //Trigger if last reward time is 0 (Unix)
        if (lastRewardTime == 0)
        {
            //Set last reward time to today
            lastRewardTime = Functions.CurrentTimeInMillis() + (3600000 * Functions.TimezoneOffset());
        }

        //Convert the 9th line to last reward
        lastReward = ParseParameter<int>(data, 8);
    }

    //Public setters for instance variables
    #region Setters
    public void SetActiveSkin(int skin) =>
        activeSkin = skin;

    public void SetLevel(Level _level) =>
        level = _level;

    public void SetGearCount(int count) =>
        gearCount = count;

    public void SetHighScore(int score) =>
        highScore = score;

    public void SetSettings(Settings_Data _settings) =>
        settings = _settings;

    public void SetLastRewardTime(long _lastRewardTime) =>
        lastRewardTime = _lastRewardTime;

    public void SetLastReward(int _lastReward) =>
        lastReward = _lastReward;

    public void SetLastRewardDay(int days) =>
        daysSinceLastReward = days;
    #endregion

    //Method called to save the purchasing of a skin.
    public void PurchaseSkin(int index, int gearPrice)
    {
        //If the skin is already purchased or the price is too expensive the method returns
        if (purchasedSkins.Contains(Gamemanager.Instance.Skins[index].title) || gearCount < gearPrice)
            return;

        //Removes the gears that the skin costs and saves that the user purchased the skin
        gearCount -= gearPrice;
        purchasedSkins.Add(Gamemanager.Instance.Skins[index].title);
    }

    //Method to activate a trigger
    public void TriggerActivated(int index)
    {
        //Sets the trigger to actiavted
        activatedLevelTriggers[index] = true;

        //Adds the rewards to the gear count
        gearCount += Gamemanager.Instance.LevelUpTriggers[index].reward.gearReward;
    }

    //ToString Method
    public override string ToString()
    {
        /*File Format:
            activeSkin: int
            purchasedSkins: string array (space seperated)
            level: float
            gearCount: int
            highScore: int
            settings: *settings to string*
            activatedLevelTriggers: bool array (space seperated)
            lastRewardTime: long
            lastReward: int
        */
        return activeSkin + "\n" +
        Functions.ArrayToString<string>(purchasedSkins.ToArray()) + "\n" +
        level + "\n" +
        gearCount + "\n" +
        highScore + "\n" +
        settings + "\n" +
        Functions.ArrayToString<bool>(activatedLevelTriggers) + "\n" +
        lastRewardTime + "\n" +
        lastReward;
    }
}
