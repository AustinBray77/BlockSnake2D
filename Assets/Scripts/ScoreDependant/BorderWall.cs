using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Class to control the Top and Bottom border walls
public class BorderWall : ScoreDependent
{
    //Variables for animation
    private Vector3 targetPosition;
    private float timeCounter;
    private float targetTime;

    //Method called on scene load, assigns default values
    private void Start()
    {
        timeCounter = 0;
        targetTime = 0;
        targetPosition = transform.position;
    }

    //Method called on every frame
    private void Update()
    {
        Vector3 position = transform.position;

        if (!Player.isDead && !Player.isAtFinish)
        {
            transform.position += new Vector3(Object.speed * Time.deltaTime * Generator.GetRelativeSpeed(), 0);
            targetPosition += new Vector3(Object.speed * Time.deltaTime * Generator.GetRelativeSpeed(), 0);
            position = transform.position;
        }

        if (targetTime > timeCounter)
        {
            transform.position += Time.deltaTime * (targetPosition - position) / (targetTime - timeCounter);
            timeCounter += Time.deltaTime;
        }
        else if (targetTime <= timeCounter && timeCounter != 0)
        {
            targetTime = 0;
            timeCounter = 0;
            transform.position = targetPosition;
        }

        if (Mathf.Abs(targetPosition.y) > Mathf.Abs(position.y) && timeCounter == 0)
        {
            targetPosition = transform.position;
        }
    }

    //Method called when the 
    public override void OnSegmentChange(int amount, bool useAnimation)
    {

        //Changes the position and size so the wall is just on the border of the screen, uses animation if requested
        if (useAnimation)
        {
            targetPosition += new Vector3(0, amount * Player.increaseFactor * (transform.position.y < 0 ? -1 : 1));
            targetTime += amount * 0.2f;
        }
        else
        {
            transform.position += new Vector3(0, amount * Player.increaseFactor * (transform.position.y < 0 ? -1 : 1));
        }

        transform.localScale += new Vector3(4 * Player.increaseFactor * Reference.cam.aspect * amount, 0);
    }

    //Method called to set position without messing up the animation
    public void SetPosition(Vector3 position)
    {
        transform.position = position;
        targetPosition = position;
        targetTime = 0;
        timeCounter = 0;
    }
}
