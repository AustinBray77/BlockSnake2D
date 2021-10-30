using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Class to control objects which add score
public class ScoreObject : Object
{
    //Stores the amount of score the object adds
    [SerializeField] private int scoreAdded;

    //Method called when an another object collides with the object
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Adds score to the player if the object hit the player
        if (collision.gameObject.tag == "Player")
        {
            Refrence.player.AddScore(scoreAdded);
            Destroy(gameObject);
        }
    }
}
