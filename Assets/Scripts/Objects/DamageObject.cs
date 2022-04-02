using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Class to control objects which kill the player
public class DamageObject : Object
{
    private new void Awake()
    {
        base.Awake();
        gameObject.tag = "Damage";
    }

    //Method called when an another object collides with the object
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Log("Registered Collision...");

        if (!interactable)
        {

            return;
        }

        //Kills the player if the object hit the player or one of its segments
        if (collision.gameObject.tag == "Player" || collision.gameObject.tag == "Segment")
        {
            Log("Hit Player");
            bool destroy = !Reference.player.KillPlayer(false);

            if (destroy)
            {
                if (Serializer.Instance.activeData.settings.soundEnabled)
                {
                    //audioSource.PlayOneShot(audioSource.clip);
                }

                ObjectDestroy();
            }
        }
    }
}
