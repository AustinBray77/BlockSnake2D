using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Class to control the object destroyer object
public class Destroyer : MonoBehaviour
{
    //Properties for the collider and the offset of how many iterations it should wait before moving again, { backup <= 0 }
    private BoxCollider2D _collider;
    public int backup;

    //Method called on scene load
    private void Start()
    {
        //Sets default values for backup and _collider
        backup = 0;
        _collider = GetComponent<BoxCollider2D>();
    }

    //Method called when a segment is added to the player
    public void OnSegmentChange(int amount)
    {
        //Triggers if the destroyer should wait an amount of iterations
        if (backup != 0)
        {
            //Triggers if that iteration amount is less than the amount of segments added
            if (backup + amount > 0)
            {
                //Changes the position so the destroyer is just on the left border of the screen
                transform.position -= new Vector3(Player.increaseFactor * 2 * (backup + amount), 0);

                //Resets backup
                backup = 0;
            }
            //Else just uniterates backup by the amount
            else
            {
                backup += amount;
            }
        }
        //Else just adds the amount and changes the position by the desired amount
        else
        {
            transform.position -= new Vector3(Player.increaseFactor * 2 * amount, 0);
        }
        
        //Changes to collider to cover the entire screen
        _collider.size = new Vector3(2, Generator.GetBoundsDistance() + 2);
    }

    //Method called when an another object collides with the object
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //If the object is an object or a parallax object the object is destroyed
        if (collision.tag == "Object" || collision.tag == "Parallax")
        {
            //If the object is a finish object the generator is told that the finish object has been destroyed
            if(collision.name.Contains("Finish")) 
                Refrence.gen.OnFinishDestroyed();

            //Destroys the colliding object
            Destroy(collision.gameObject);
        }
    }
}
