using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GearObject : Object
{
    [SerializeField] private int gearsAdded;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Refrence.player.AddGears(gearsAdded);
            Destroy(gameObject);
        }
    }
}
