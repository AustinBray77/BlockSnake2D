using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Class to control the gear objects
public class GearObject : Object
{
    //Stores the amount of gears the object adds
    [SerializeField] private int gearsAdded;

    //Method called when an another object collides with the object
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Adds the specified amount of gears to the player if the object hit the player
        if (collision.gameObject.tag == "Player")
        {
            Refrence.player.AddGears(gearsAdded);
            Destroy(gameObject);
        }
    }
}
