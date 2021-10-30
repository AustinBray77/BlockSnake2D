using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Class to control the parallax background
public class Parallax_Object : MonoBehaviour
{
    //Variables to store the layer and speed
    public int layer;
    private static float speed = 2;

    //Method called on each frame
    private void Update()
    {
        //If the player is not at finish or dead and moves the object by the calculated speed
        if(!Player.isAtFinish && !Player.isDead)
            transform.position += new Vector3(-speed / (layer + 1), 0) * Time.deltaTime;
    }
}
