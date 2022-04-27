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
        //Stores the amount to increase the power up by
        public float increaseAmount;

        //Stores the level at which this trigger triggers
        public int levelAmount;

        //Constructor to copy from other trigger
        public IncrementTrigger(IncrementTrigger trigger)
        {
            //Sets variables to passed values
            increaseAmount = trigger.increaseAmount;
            levelAmount = trigger.levelAmount;
        }
    }

    //Stores the title, level, image, and value of the card
    public string title;
    public int level;
    public Sprite image;
    public float value;

    //Stores if the value is a percentage
    [SerializeField] private bool isPercentage;

    //Stores the events to call when the card is chosen and when the card is reverted
    [SerializeField] private UnityEvent<float, int> normalEvent, inverseEvent;

    //Stores all the associated increment triggers 
    [SerializeField] private List<IncrementTrigger> valueCurve;

    //Stores the last increment trigger that was hit
    private IncrementTrigger lastValueTriggered;

    //Stores the index of the card in the card array
    private int cardIndex;

    //Method to set the index of the card
    public void SetIndex(int index)
    {
        //Sets the index to the passed index
        cardIndex = index;
    }

    //Method clalled when the card is clicked
    public void Call()
    {
        //Invokes the event associated with the card
        RunEvent(normalEvent);

        //Levels up the card
        level++;

        //Triggers if the card may be able to level up
        if (valueCurve.Count >= 1)
        {
            //Triggers if the card should level up and increases its value
            if (valueCurve[0].levelAmount <= level)
            {
                //Increase the value by the increase amount
                value += valueCurve[0].increaseAmount;

                //Save the current trigger as the last value triggered
                lastValueTriggered = new IncrementTrigger(valueCurve[0]);

                //Remove the trigger from the list as it has been triggered
                valueCurve.RemoveAt(0);
            }
        }
    }

    //Method called which returns the description for this card
    public string GetDescription()
    {
        //Base value for return string
        string description = "";

        //Triggers if the value is over 0
        if (value > 0)
        {
            //Adds a plus sign
            description += "+";
        }

        //Triggers if the value is a percentage
        if (isPercentage)
        {
            //Removes 100% from the value
            float rawVal = value - 1f;

            //Adds the value to the description (rounded)
            description += Mathf.Round(rawVal * 100).ToString() + "%";
        }
        else
        {
            //Else just adds the value to the description
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

        //Runs the event to revert the card's effect
        RunEvent(inverseEvent);

        //Triggers if the card has experenced an upgrade
        if (lastValueTriggered != null)
        {
            //Triggers if the card should deupgrade, and reverts the upgrade
            if (lastValueTriggered.levelAmount > level)
            {
                //Decreases value by the last increase amount
                value -= lastValueTriggered.increaseAmount;

                //Adds the last value trigger back to the list at the 0th index
                valueCurve.Insert(0, lastValueTriggered);

                //Nullifies last value triggered
                lastValueTriggered = null;
            }
        }
    }

    //Method to call powerup upgrades
    public void RunEvent(UnityEvent<float, int> method)
    {
        //Runs the card method
        method.Invoke(value, cardIndex);

        //Hides the Finish UI
        Finish_UI.Instance.GetComponent<Finish_UI>().Hide();

        //Sets the last upgraded card on the player to the selected card
        Reference.player.lastUpgraded = Reference.cardTypes[cardIndex];
    }
}
