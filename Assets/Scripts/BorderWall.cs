using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BorderWall : MonoBehaviour
{
    public int backup;

    private void Start()
    {
        backup = 0;
    }

    public void OnSegmentChange(int amount)
    {
        if (backup != 0)
            if (backup + amount > 0)
            {
                transform.position += new Vector3(0, (backup + amount) * Player.increaseFactor * (transform.position.y < 0 ? -1 : 1));
                transform.localScale += new Vector3(4 * Player.increaseFactor * (16 / 9) * (backup + amount), 0);
                backup = 0;
            }
            else
                backup += amount;
        else
        {
            transform.position += new Vector3(0, amount * Player.increaseFactor * (transform.position.y < 0 ? -1 : 1));
            transform.localScale += new Vector3(4 * Player.increaseFactor * (16 / 9) * amount, 0);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player" || collision.tag == "Segment")
            Refrence.player.KillPlayer();
    }
}
