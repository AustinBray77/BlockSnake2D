using System;

//Wrapper class for saving users data to the System
[System.Serializable]
class Save_Data
{
    //Properties to store the active skin, purchased skins, level, gearcount, highscore, and settings
    public int activeSkin { get; private set; }
    public bool[] purchasedSkins { get; private set; }
    public Level level { get; private set; }
    public int gearCount { get; private set; }
    public int highScore { get; private set; }
    public Settings_Data settings { get; private set; }

    //Default constructor
    public Save_Data(int _activeSkin, bool[] _purchasedSkins, Level _level, int _gearCount, int _highScore, Settings_Data _settings)
    {
        activeSkin = _activeSkin;
        purchasedSkins = _purchasedSkins;
        level = _level;
        gearCount = _gearCount;
        highScore = _highScore;
        settings = _settings;
    }

    //Constructor with string data for parsin
    public Save_Data(string data)
    {
        //Splits data
        string[] vals = data.Split('\n');

        //If statements are to prevent index out of bounds error, if the data is too short the data is set to a defualt value

        if (vals.Length >= 0)
        {
            //Converts Active Skin
            int.TryParse(vals[0], out int _activeSkin);
            activeSkin = _activeSkin;
        }
        else
        {
            //Sets active skin to base value
            activeSkin = 0;
        }

        //Sets purchased skins to base value
        purchasedSkins = new bool[Shop_UI.skinCount];

        if (vals.Length >= 2)
        {
            //Converts purchased skins
            string[] purchasedSkinsStrs = vals[1].Split(' ');

            for (int i = 0; i < purchasedSkinsStrs.Length && i < purchasedSkins.Length; i++)
            {
                purchasedSkins[i] = purchasedSkinsStrs[i].ToLower() == "true";
            }

            for (int i = purchasedSkinsStrs.Length; i < purchasedSkins.Length; i++)
            {
                purchasedSkins[i] = false;
            }
        }
        else
        {
            //Only the first skin is purchased
            purchasedSkins[0] = true;
        }

        if (vals.Length >= 3)
        {
            //Converts level
            level = new Level(vals[2]);
        }
        else
        {
            //Sets level to base value, level with 0 xp
            level = new Level(0);
        }

        if (vals.Length >= 4)
        {
            //Converts gear count
            int.TryParse(vals[3], out int _gearCount);
            gearCount = _gearCount;
        }
        else
        {
            //Sets gear count to base value
            gearCount = 0;
        }

        if (vals.Length >= 5)
        {
            //Converts high score
            int.TryParse(vals[4], out int _highScore);
            highScore = _highScore;
        }
        else
        {
            //Sets highscore to base value
            highScore = 0;
        }

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
        if (purchasedSkins[index] || gearCount < gearPrice)
            return;

        //Removes the gears that the skin costs and saves that the user purchased the skin
        gearCount -= gearPrice;
        purchasedSkins[index] = true;
    }

    //Method to set the settings
    public void SetSettings(Settings_Data _settings) =>
        settings = _settings;

    //ToString Method
    public override string ToString()
    {
        return activeSkin + "\n" +
        Functions.ArrayToString<bool>(purchasedSkins) + "\n" +
        level + "\n" +
        gearCount + "\n" +
        highScore + "\n" +
        settings + "\n";
    }
}
