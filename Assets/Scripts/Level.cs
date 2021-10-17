using UnityEngine;

[System.Serializable]
public class Level
{
    public float xp { get; private set; }
    [HideInInspector] public int level => xpToLevel(xp);

    public Level(int _level)
    {
        xp = levelToXP(_level);
    }

    public Level(float _xp)
    {
        xp = _xp;
    }

    public void SetXP(float _xp) =>
        xp = _xp;

    public void AddXP(float increase) =>
        xp += increase;

    public static float levelToXP(int level) =>
        2.5f * level * level;

    public static int xpToLevel(float xp) =>
        (int) Mathf.Sqrt(xp / 2.5f);
}
