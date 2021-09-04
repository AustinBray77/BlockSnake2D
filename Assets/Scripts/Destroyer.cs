using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroyer : MonoBehaviour
{
    private BoxCollider2D _collider;
    public int backup;

    private void Start()
    {
        backup = 0;
        _collider = GetComponent<BoxCollider2D>();
    }

    public void OnSegmentChange(int amount)
    {
        if (backup != 0)
            if (backup + amount > 0)
            {
                transform.position -= new Vector3(Player.increaseFactor * 2 * (backup + amount), 0);
                backup = 0;
            }
            else
                backup += amount;
        else
            transform.position -= new Vector3(Player.increaseFactor * 2 * amount, 0);
        
        _collider.size = new Vector3(2, Generator.GetBoundsDistance() + 2);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Object")
        {
            if(collision.name.Contains("Finish")) 
                Refrence.gen.OnFinishDestroyed();

            Destroy(collision.gameObject);
        }
    }
}
