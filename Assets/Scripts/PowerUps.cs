using UnityEngine;

public class PowerUps : MonoBehaviour
{
    //Increases the players speed
    public void Card_IncreaseSpeed(int cardIndex)
    {
        //Invert if inverse is called
        if (!Refrence.cardTypes[cardIndex].inverse)
        {
            //Increases the players speed by the current determind amount
            Refrence.player.speed *= Refrence.cardTypes[cardIndex].value;
        }
        else
        {
            //Decreases the players speed by the current determind amount
            Refrence.player.speed /= Refrence.cardTypes[cardIndex].value;
        }

        //Hides the Finish UI and sets the last upgraded card to the selected card
        Refrence.finishUI.GetComponent<Finish_UI>().Hide();
        Refrence.player.lastUpgraded = Refrence.cardTypes[cardIndex];
    }

    //Increases the players rotation speed
    public void Card_IncreaseRotSpeed(int cardIndex)
    {
        //Invert if inverse is called
        if (!Refrence.cardTypes[cardIndex].inverse)
        {
            //Increases the players rotation speed by the current determind amount
            Refrence.player.rotSpeed *= Refrence.cardTypes[cardIndex].value;
        }
        else
        {
            //Decreases the players rotation speed by the current determind amount
            Refrence.player.rotSpeed /= Refrence.cardTypes[cardIndex].value;
        }

        //Hides the Finish UI and sets the last upgraded card to the selected card
        Refrence.finishUI.GetComponent<Finish_UI>().Hide();
        Refrence.player.lastUpgraded = Refrence.cardTypes[cardIndex];
    }

    //Removes a segment from the player
    public void Card_RemoveSegment(int cardIndex)
    {
        //Invert if inverse is called
        if (!Refrence.cardTypes[cardIndex].inverse)
        {
            //Removes the determind amount of segments
            Refrence.player.RemoveSegments((int)Refrence.cardTypes[cardIndex].value * -1);
        }
        else
        {
            //Adds the determind amount of segments
            Refrence.player.AddSegments((int)Refrence.cardTypes[cardIndex].value * -1);
        }

        //Hides the Finish UI and sets the last upgraded card to the selected card
        Refrence.finishUI.GetComponent<Finish_UI>().Hide();
        Refrence.player.lastUpgraded = Refrence.cardTypes[cardIndex];
    }

    //Decreases the spawn speed of the objects
    public void Card_DecreaseSpawnSpeed(int cardIndex)
    {
        //Invert if inverse is called
        if (!Refrence.cardTypes[cardIndex].inverse)
        {
            //Decreases the spawn speed by the current determind amount
            Refrence.gen.spawnSpeed *= Refrence.cardTypes[cardIndex].value;
        }
        else
        {
            //Increases the spawn speed by the current determind amount
            Refrence.gen.spawnSpeed /= Refrence.cardTypes[cardIndex].value;
        }

        //Hides the Finish UI and sets the last upgraded card to the selected card
        Refrence.finishUI.GetComponent<Finish_UI>().Hide();
        Refrence.player.lastUpgraded = Refrence.cardTypes[cardIndex];
    }

    //Adds a shield to the player
    public void Card_AddShield(int cardIndex)
    {
        //Invert if inverse is called
        if (!Refrence.cardTypes[cardIndex].inverse)
        {
            //Adds the shields to the player
            Refrence.player.AddShields((int)Refrence.cardTypes[cardIndex].value);
        }
        else
        {
            //Removes the shields from the player
            Refrence.player.AddShields(-(int)Refrence.cardTypes[cardIndex].value);
        }
        //Hides the Finish UI and sets the last upgraded card to the selected card
        Refrence.finishUI.GetComponent<Finish_UI>().Hide();
        Refrence.player.lastUpgraded = Refrence.cardTypes[cardIndex];
    }

    //Adds a slow down object to the the player
    public void Card_SlowDown(int cardIndex)
    {
        //Enables the slow down object if the player does not have it yet
        Refrence.player.IncreaseSlowDownPercentage(Refrence.cardTypes[cardIndex].value);

        //Hides the Finish UI and sets the last upgraded card to the selected card
        Refrence.finishUI.GetComponent<Finish_UI>().Hide();
        Refrence.player.lastUpgraded = Refrence.cardTypes[cardIndex];
    }
}