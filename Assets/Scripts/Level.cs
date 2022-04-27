using UnityEngine;
using System.Collections.Generic;

//Serializable class to control the level
[System.Serializable]
public class Level
{
    //Serializable class to control the level triggers
    [System.Serializable]
    public class LevelUpTrigger
    {
        //Serializable class to control the rewards for each level up class
        [System.Serializable]
        public class Reward
        {
            //Stores the amount of gears the trigger rewards
            public int gearReward;

            //Stores if the trigger rewards a skin
            public bool unlocksSkin;

            //Base constructor
            public Reward(int reward, bool unlocks)
            {
                //Assigns values to the passed values
                gearReward = reward;
                unlocksSkin = unlocks;
            }
        }

        //Stores the level of the trigger
        public int levelTrigger;

        //Stores the reward of the tigger
        public Reward reward;

        //Base constructor
        public LevelUpTrigger(int trigger, int gearReward, bool unlocksSkin)
        {
            //Assigns values to passed values
            levelTrigger = trigger;

            //Converts to Reward class
            reward = new Reward(gearReward, unlocksSkin);
        }
    }

    //Properties to store the xp and level
    public float xp { get; private set; }
    [HideInInspector] public int level => XPToLevel(xp);

    //Constructor using level
    public Level(int _level)
    {
        //Sets xp from passed level
        xp = LevelToXP(_level);
    }

    //Constructor using XP
    public Level(float _xp)
    {
        //Sets xp to passed xp
        xp = _xp;
    }

    //Constructor using String, stirng will be xp in string form
    public Level(string xpStr)
    {
        //Converts xp from string to float
        float.TryParse(xpStr, out float _xp);

        //Sets the xp
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
    public override string ToString() =>
        xp.ToString();

    //Implicitally converts from a string to a level
    public static implicit operator Level(string s) => new Level(s);
}
