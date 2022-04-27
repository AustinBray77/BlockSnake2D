using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Class to control the parallax diffuser object
public class ParallaxDiffuser : ScoreDependent
{
    //Method called on every frame
    private void Update()
    {
        //Trigger if the player is not dead or at finish
        if (!Player.isDead && !Player.isAtFinish)
        {
            //Move the diffuser by the object speed times the relative speed and last frame time
            transform.position += new Vector3(Object.speed * Time.deltaTime * Generator.GetRelativeSpeed(), 0);
        }
    }

    //Method called when the segment count updates
    public override void OnSegmentChange(int amount, bool useAnimation)
    {
        //Increases the size to cover the larger fov
        transform.localScale += new Vector3(4 * Player.increaseFactor * Reference.cam.aspect * amount, amount * Player.increaseFactor * 2);
    }
}
