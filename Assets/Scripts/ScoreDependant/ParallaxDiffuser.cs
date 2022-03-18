using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxDiffuser : ScoreDependent
{
    //Method called on every frame
    private void Update()
    {
        if (!Player.isDead && !Player.isAtFinish)
        {
            transform.position += new Vector3(Object.speed * Time.deltaTime * Generator.GetRelativeSpeed(), 0);
        }
    }

    //Method called when the 
    public override void OnSegmentChange(int amount, bool useAnimation)
    {
        transform.localScale += new Vector3(4 * Player.increaseFactor * Reference.cam.aspect * amount, amount * Player.increaseFactor * 2);
    }
}
