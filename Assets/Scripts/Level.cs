using UnityEngine;

//Serializable class to control the level
[System.Serializable]
public class Level
{
    //Properties to store the xp and level
    public float xp { get; private set; }
    [HideInInspector] public int level => XPToLevel(xp);

    //Constructor using level
    public Level(int _level)
    {
        xp = LevelToXP(_level);
    }

    //Constructor using XP
    public Level(float _xp)
    {
        xp = _xp;
    }

    //Method called to set the xp
    public void SetXP(float _xp) =>
        xp = _xp;

    //Method called to add xp
    public void AddXP(float increase) =>
        xp += increase;

    //Method called to convert Level to XP
    public static float LevelToXP(int level) =>
        2.5f * level * level;

    //Method called to convert XP to Level
    public static int XPToLevel(float xp) =>
        (int) Mathf.Sqrt(xp / 2.5f);
}
