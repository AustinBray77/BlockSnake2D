using UnityEngine;
using System.Collections.Generic;

//Class to control the animated snake at the start screen
public class AnimatedSnake : Segmentable
{
    //Property to store the segments
    [SerializeField] private List<Segment> segments;

    // Start is called before the first frame update
    private new void Start()
    {
        base.Start();
        //Sets base values
        turnDelay = 0.25f;
        ySpeed = 50;
        xSpeed = 0;
        rotSpeed = 10;

        //Sets segment after to first segment
        segmentAfter = segments[0];

        //Assigns each segment to the segment before it
        for (int i = 0; i < segments.Count - 1; i++)
        {
            segments[i].segmentAfter = segments[i + 1];
        }

        //Sets the last segment to the last segment in the list
        segmentLast = segments[segments.Count - 1];
    }

    // Update is called once per frame
    private void Update()
    {
        //If below the reference line, rotate up
        if (transform.position.y < 0)
        {
            Rotate(YDirection.Up);
        }
        //Else if below the reference line, rotate down
        else if (transform.position.y > 0)
        {
            Rotate(YDirection.Down);
        }

        //Moves the player and the segments
        Move();
        MoveSegments();
    }
}
