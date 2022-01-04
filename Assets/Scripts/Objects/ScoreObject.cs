using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Class to control objects which add score
[RequireComponent(typeof(AudioSource))]
public class ScoreObject : Object
{
    //Stores the amount of score the object adds
    [SerializeField] private int scoreAdded;

    //Method called when an another object collides with the object
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!interactable)
        {
            return;
        }

        //Adds score to the player if the object hit the player
        if (collision.gameObject.tag == "Player")
        {
            if (Serializer.activeData.settings.soundEnabled)
            {
                AudioSource audioSource = GetComponent<AudioSource>();
                audioSource.pitch = Mathf.Pow(2, Player.score / 12f);
                audioSource.Play();
            }

            Refrence.player.AddScore(scoreAdded);
            ObjectDestroy();
        }
    }
}
