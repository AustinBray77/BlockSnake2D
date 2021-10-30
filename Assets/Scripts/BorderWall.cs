using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Class to control the Top and Bottom border walls
public class BorderWall : MonoBehaviour
{
    //Variable which stores the offset of how many iterations it should wait before moving again, { backup <= 0 }
    public int backup;

    //Method called on scene load, assigns default values
    private void Start()
    {
        backup = 0;
    }

    //Method called when the 
    public void OnSegmentChange(int amount)
    {
        //Triggers if the border wall should wait an amount of iterations
        if (backup != 0)
        { 
            //Triggers if that iteration amount is less than the amount of segments added
            if (backup + amount > 0)
            {
                //Changes the position and size so the wall is just on the border of the screen
                transform.position += new Vector3(0, (backup + amount) * Player.increaseFactor * (transform.position.y < 0 ? -1 : 1));
                transform.localScale += new Vector3(4 * Player.increaseFactor * (16 / 9) * (backup + amount), 0);

                //Resets backup
                backup = 0;
            }
            //Else just uniterates backup by the amount
            else
            {
                backup += amount;
            }
        }
        //Else just adds the amount and changes the position and size by the desired amount
        else
        {
            transform.position += new Vector3(0, amount * Player.increaseFactor * (transform.position.y < 0 ? -1 : 1));
            transform.localScale += new Vector3(4 * Player.increaseFactor * (16 / 9) * amount, 0);
        }
    }

    //Method called when an another object collides with the object
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Kills the player if they collide with the object
        if (collision.tag == "Player" || collision.tag == "Segment")
            Refrence.player.KillPlayer();
    }
}
