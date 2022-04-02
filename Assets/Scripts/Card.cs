using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

//Serializable class to store the data for each data cards
[System.Serializable]
public class Card
{
    //Serilizable class to store when the card should upgrade
    [System.Serializable]
    public class IncrementTrigger
    {
        public float increaseAmount;
        public int levelAmount;

        public IncrementTrigger(IncrementTrigger trigger)
        {
            increaseAmount = trigger.increaseAmount;
            levelAmount = trigger.levelAmount;
        }
    }

    //Properties to store data about the card
    public string title, desc;
    public int level;
    public Sprite image;
    public float value;

    [SerializeField] private bool isPercentage;
    [SerializeField] private UnityEvent<float, int> normalEvent, inverseEvent;
    [SerializeField] private List<IncrementTrigger> valueCurve;

    private IncrementTrigger lastValueTriggered;
    private int cardIndex;

    //Method to set the index of the card
    public void SetIndex(int index)
    {
        cardIndex = index;
    }

    //Method clalled when the card is clicked
    public void Call()
    {
        //Invokes the event associated with the card and increments the level
        RunEvent(normalEvent);
        level++;

        //Triggers if the card may be able to level up
        if (valueCurve.Count >= 1)
        {
            //Triggers if the card should level up and increases its value
            if (valueCurve[0].levelAmount <= level)
            {
                value += valueCurve[0].increaseAmount;
                lastValueTriggered = new IncrementTrigger(valueCurve[0]);
                valueCurve.RemoveAt(0);
            }
        }
    }

    //Method called which returns the description for this card
    public string GetDescription()
    {
        //Base value for return string
        string description = "";

        //Adds a plus or minus according the value
        if (value > 0)
        {
            description += "+";
        }

        //Formats the description if it is a percentage
        if (isPercentage)
        {
            float rawVal = value - 1f;
            description += Mathf.Round(rawVal * 100).ToString() + "%";
        }
        //Else just adds the value
        else
        {
            description += value.ToString();
        }

        //Returns the formatted description
        return description;
    }

    //Method called to remove a level from the card, called after respawn
    public void RemoveLevel()
    {
        //Decrements level
        level--;
        RunEvent(inverseEvent);

        //Triggers if the card has experenced an upgrade
        if (lastValueTriggered != null)
        {
            //Triggers if the card should deupgrade, and reverts the upgrade
            if (lastValueTriggered.levelAmount > level)
            {
                value -= lastValueTriggered.increaseAmount;
                valueCurve.Insert(0, lastValueTriggered);
                lastValueTriggered = null;
            }
        }
    }

    //Base method to call powerup upgrades
    public void RunEvent(UnityEvent<float, int> method)
    {
        //Runs the card method
        method.Invoke(value, cardIndex);

        //Hides the Finish UI and sets the last upgraded card to the selected card
        Finish_UI.Instance.GetComponent<Finish_UI>().Hide();
        Reference.player.lastUpgraded = Reference.cardTypes[cardIndex];
    }
}
