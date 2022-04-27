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
        //If the object should not be handling collisions, return
        if (!interactable)
        {
            return;
        }

        //Triggers if the player was hit
        if (collision.gameObject.tag == "Player")
        {
            //Triggers if sound is enabled
            if (Serializer.Instance.activeData.settings.soundEnabled)
            {
                //Gets the audio source
                AudioSource audioSource = GetComponent<AudioSource>();

                //Calculated the pitch to use
                audioSource.pitch = Mathf.Pow(2, Player.score / 12f);

                //Plays at the correct pitch
                audioSource.Play();
            }

            //Adds score to the player
            Reference.player.AddScore(scoreAdded);

            //Destroys itself
            ObjectDestroy();
        }
    }
}
