using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Class to control the parallax background
public class Parallax_Object : MonoBehaviour
{
    //Variables to store the layer and speed
    public int layer;
    private static float baseSpeed = 2;
    private float speed;

    //Called when the object is instantiated
    private void Awake()
    {
        speed = baseSpeed / (layer + 1);
    }

    //Method called on each frame
    private void Update()
    {
        //If the player is not at finish or dead and moves the object by the calculated speed
        if(!Player.isAtFinish && !Player.isDead)
            transform.position += new Vector3(-speed, 0) * Time.deltaTime;
    }
}
