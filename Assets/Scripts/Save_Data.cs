[System.Serializable]
class Save_Data
{
    public int activeSkin { get; private set; }
    public bool[] purchasedSkins { get; private set; }
    public Level level { get; private set; }
    public int gearCount { get; private set; }
    public int highScore { get; private set; }

    public Save_Data(int activeSkin, bool[] purchasedSkins, Level level, int gearCount, int highScore)
    {
        this.activeSkin = activeSkin;
        this.purchasedSkins = purchasedSkins;
        this.level = level;
        this.gearCount = gearCount;
        this.highScore = highScore;
    }

    public void SetActiveSkin(int skin) =>
        activeSkin = skin;

    public void SetLevel(Level _level) =>
        level = _level;

    public void SetGearCount(int count) =>
        gearCount = count;

    public void SetHighScore(int score) =>
        highScore = score;

    public void PurchaseSkin(int index, int gearPrice)
    {
        if (purchasedSkins[index] || gearCount < gearPrice)
            return;

        gearCount -= gearPrice;
        purchasedSkins[index] = true;
    }
}
