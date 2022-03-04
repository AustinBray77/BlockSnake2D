using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Class to control objects which kill the player
public class DamageObject : Object
{
    //Method called when an another object collides with the object
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!interactable)
        {
            return;
        }

        //Kills the player if the object hit the player or one of its segments
        if (collision.gameObject.tag == "Player" || collision.gameObject.tag == "Segment")
        {
            bool destroy = Reference.player.shieldCount > 0;
            Reference.player.KillPlayer();

            if (destroy)
            {
                if (Serializer.activeData.settings.soundEnabled)
                {
                    //audioSource.PlayOneShot(audioSource.clip);
                }

                ObjectDestroy();
            }
        }
    }
}
