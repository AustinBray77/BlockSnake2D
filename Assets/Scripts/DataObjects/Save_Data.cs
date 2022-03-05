using System;
using System.Linq;
using System.Collections.Generic;

//Wrapper class for saving users data to the System
[Serializable]
class Save_Data
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

    //Default constructor
    public Save_Data(int _activeSkin, HashSet<string> _purchasedSkins, Level _level, int _gearCount, int _highScore, Settings_Data _settings, bool[] _activatedLevelTriggers,
        long _lastRewardTime, int _lastReward)
    {
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
        ParseData(data);
    }

    //Constructor with string array data for parsing
    public Save_Data(string[] data)
    {
        ParseData(data);
    }

    public T ParseParameter<T>(string[] vals, int index)
    {
        if (index < vals.Length)
        {
            return Functions.TryGenericConversion<string, T>(vals[index]);
        }

        return default(T);
    }

    public void ParseData(string data)
    {
        //Splits data
        string[] vals = data.Split('\n');

        //If statements are to prevent index out of bounds error, if the data is too short the data is set to a defualt value
        activeSkin = ParseParameter<int>(vals, 0);

        //Sets purchased skins to base value
        bool[] bpurchasedSkins = new bool[Shop_UI.skinCount];

        if (vals.Length >= 2)
        {
            //Converts purchased skins
            string[] purchasedSkinsStrs = vals[1].Split(' ');

            for (int i = 0; i < purchasedSkinsStrs.Length && i < Shop_UI.skinCount; i++)
            {
                bpurchasedSkins[i] = purchasedSkinsStrs[i].ToLower() == "true";
            }

            for (int i = purchasedSkinsStrs.Length; i < Shop_UI.skinCount; i++)
            {
                bpurchasedSkins[i] = false;
            }
        }
        else
        {
            //Only the first skin is purchased
            bpurchasedSkins[0] = true;
        }

        if (vals.Length >= 3)
        {
            //Converts level
            level = vals[2];
        }
        else
        {
            //Sets level to base value, level with 0 xp
            level = new Level(0);
        }

        gearCount = ParseParameter<int>(vals, 3);

        highScore = ParseParameter<int>(vals, 4);

        if (vals.Length >= 6)
        {
            //Converts settings
            settings = new Settings_Data(vals[5]);
        }
        else
        {
            //Sets settings to base value
            settings = new Settings_Data("");
        }

        activatedLevelTriggers = new bool[Reference.levelUpTriggers.Count];

        if (vals.Length >= 7)
        {
            string[] activatedTriggerStrings = vals[6].Split(' ');

            //The string values to bools
            for (int i = 0; i < activatedLevelTriggers.Length && i < activatedTriggerStrings.Length; i++)
            {
                activatedLevelTriggers[i] = activatedTriggerStrings[i].ToLower() == "true";
            }

            //Sets unknowns to false
            for (int i = activatedTriggerStrings.Length; i < activatedLevelTriggers.Length; i++)
            {
                activatedLevelTriggers[i] = false;
            }
        }

        lastRewardTime = ParseParameter<long>(vals, 7);

        if (lastRewardTime == 0)
        {
            lastRewardTime = Functions.CurrentTimeInMillis() + (3600000 * Functions.TimezoneOffset());
        }

        lastReward = ParseParameter<int>(vals, 8);
    }

    private void ParseData(string[] data)
    {
        //If statements are to prevent index out of bounds error, if the data is too short the data is set to a defualt value
        activeSkin = ParseParameter<int>(data, 0);

        //Sets purchased skins to base value
        bool[] bpurchasedSkins = new bool[Shop_UI.skinCount];

        if (data.Length >= 2)
        {
            purchasedSkins = new HashSet<string>(data[1].Split(' '));
        }
        else
        {
            purchasedSkins = new HashSet<string>(new string[] { "White" });
        }

        if (data.Length >= 3)
        {
            //Converts level
            level = data[2];
        }
        else
        {
            //Sets level to base value, level with 0 xp
            level = new Level(0);
        }

        gearCount = ParseParameter<int>(data, 3);

        highScore = ParseParameter<int>(data, 4);

        if (data.Length >= 6)
        {
            //Converts settings
            settings = new Settings_Data(data[5]);
        }
        else
        {
            //Sets settings to base value
            settings = new Settings_Data("");
        }

        activatedLevelTriggers = new bool[Reference.levelUpTriggers.Count];

        if (data.Length >= 7)
        {
            string[] activatedTriggerStrings = data[6].Split(' ');

            //The string values to bools
            for (int i = 0; i < activatedLevelTriggers.Length && i < activatedTriggerStrings.Length; i++)
            {
                activatedLevelTriggers[i] = activatedTriggerStrings[i].ToLower() == "true";
            }

            //Sets unknowns to false
            for (int i = activatedTriggerStrings.Length; i < activatedLevelTriggers.Length; i++)
            {
                activatedLevelTriggers[i] = false;
            }
        }

        lastRewardTime = ParseParameter<long>(data, 7);

        if (lastRewardTime == 0)
        {
            lastRewardTime = Functions.CurrentTimeInMillis() + (3600000 * Functions.TimezoneOffset());
        }

        lastReward = ParseParameter<int>(data, 8);
    }

    //Public setters for instance variables
    public void SetActiveSkin(int skin) =>
        activeSkin = skin;

    public void SetLevel(Level _level) =>
        level = _level;

    public void SetGearCount(int count) =>
        gearCount = count;

    public void SetHighScore(int score) =>
        highScore = score;

    //Method called to save the purchasing of a skin.
    public void PurchaseSkin(int index, int gearPrice)
    {
        //If the skin is already purchased or the price is too expensive the method returns
        if (purchasedSkins.Contains(Shop_UI.skins[index].title) || gearCount < gearPrice)
            return;

        //Removes the gears that the skin costs and saves that the user purchased the skin
        gearCount -= gearPrice;
        purchasedSkins.Add(Shop_UI.skins[index].title);
    }

    public void TriggerActivated(int index)
    {
        activatedLevelTriggers[index] = true;
        gearCount += Reference.levelUpTriggers[index].reward.gearReward;
    }

    //Method to set the settings
    public void SetSettings(Settings_Data _settings) =>
        settings = _settings;

    //Method to set the last reward time
    public void SetLastRewardTime(long _lastRewardTime) =>
        lastRewardTime = _lastRewardTime;

    //Method to set the last reward
    public void SetLastReward(int _lastReward) =>
        lastReward = _lastReward;

    //ToString Method
    public override string ToString()
    {
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
