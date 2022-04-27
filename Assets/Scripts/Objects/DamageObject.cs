using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Class to control objects which kill the player
public class DamageObject : Object
{
    //Method called on object instantiation
    private new void Awake()
    {
        base.Awake();

        //Set gameobject tag
        gameObject.tag = "Damage";
    }

    //Method called when an another object collides with the object
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Log("Registered Collision...");

        //If the object should not be handling collisions, return
        if (!interactable)
        {
            return;
        }

        //Kills the player if the object hit the player or one of its segments
        if (collision.gameObject.tag == "Player" || collision.gameObject.tag == "Segment")
        {
            Log("Hit Player");

            //Stores if the object should be destroyed (if the player doesn't die, destroy it)
            bool destroy = !Reference.player.KillPlayer(false);

            //Triggers if the object shoudl be destroyed
            if (destroy)
            {
                //Defunct
                if (Serializer.Instance.activeData.settings.soundEnabled)
                {
                    //audioSource.PlayOneShot(audioSource.clip);
                }

                //"Destroy" the object
                ObjectDestroy();
            }
        }
    }
}
