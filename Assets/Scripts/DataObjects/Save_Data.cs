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
}
