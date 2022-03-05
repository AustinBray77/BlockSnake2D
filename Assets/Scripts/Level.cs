using UnityEngine;
using System.Collections.Generic;

//Serializable class to control the level
[System.Serializable]
public class Level
{
    [System.Serializable]
    public class LevelUpTrigger
    {
        [System.Serializable]
        public class Reward
        {
            public int gearReward;
            public bool unlocksSkin;

            public Reward(int reward, bool unlocks)
            {
                gearReward = reward;
                unlocksSkin = unlocks;
            }
        }

        public int levelTrigger;
        public Reward reward;

        public LevelUpTrigger(int trigger, int gearReward, bool unlocksSkin)
        {
            levelTrigger = trigger;
            reward = new Reward(gearReward, unlocksSkin);
        }
    }

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

    //Constructor using String, stirng will be xp in string form
    public Level(string xpStr)
    {
        //Converts xp from string to float
        float.TryParse(xpStr, out float _xp);
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
        (int)Mathf.Sqrt(xp / 2.5f);

    //ToString Method
    public override string ToString()
    {
        return xp.ToString();
    }

    public static implicit operator Level(string s) => new Level(s);
}
