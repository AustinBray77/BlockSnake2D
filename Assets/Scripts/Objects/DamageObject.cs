using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageObject : Object
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player" || collision.gameObject.tag == "Segment")
        {
            Refrence.player.KillPlayer();
            Destroy(gameObject);
        }
    }
}
