using UnityEngine;
using System.Collections.Generic;

public class AnimatedSnake : Segmentable
{
    [SerializeField] private List<Segment> segments;

    // Start is called before the first frame update
    private void Start()
    {
        turnDelay = 0.25f;
        ySpeed = 50;
        xSpeed = 0;
        rotSpeed = 10;

        Segmentable_Init();

        segmentAfter = segments[0];

        for (int i = 0; i < segments.Count - 1; i++)
        {
            segments[i].segmentAfter = segments[i + 1];
        }

        segmentLast = segments[segments.Count - 1];
    }

    // Update is called once per frame
    private void Update()
    {
        if (transform.position.y < 0)
        {
            Rotate(YDirection.Up);
        }
        else if (transform.position.y > 0)
        {
            Rotate(YDirection.Down);
        }

        Move();
        MoveSegments();
    }
}
