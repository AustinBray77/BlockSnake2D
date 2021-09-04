using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreObject : Object
{
    [SerializeField] private int scoreAdded;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Refrence.player.AddScore(scoreAdded);
            Destroy(gameObject);
        }
    }
}
