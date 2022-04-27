using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Class to control the Top and Bottom border walls
public class BorderWall : ScoreDependent
{
    //Stores animation data
    private Vector3 targetPosition;
    private float timeCounter;
    private float targetTime;

    //Method called on scene load,
    private void Start()
    {
        //Assigns default values
        timeCounter = 0;
        targetTime = 0;
        targetPosition = transform.position;
    }

    //Method called on every frame
    private void Update()
    {
        //Triggers if the player is not dead or at a finish
        if (!Player.isDead && !Player.isAtFinish)
        {
            //Moves the gameobject and its target position by the object speed
            transform.position += new Vector3(Object.speed * Time.deltaTime * Generator.GetRelativeSpeed(), 0);
            targetPosition += new Vector3(Object.speed * Time.deltaTime * Generator.GetRelativeSpeed(), 0);
        }

        //Animate the border wall
        HandleAnimation();
    }

    //Method to handle the animating of the Border walls
    private void HandleAnimation()
    {
        //Saves the position for optimization
        Vector3 position = transform.position;

        //Triggers if the object should still be animating (time left in animation)
        if (targetTime > timeCounter)
        {
            //Moves the border wall the associated amount based on the distance and time to complete
            position += Time.deltaTime * (targetPosition - position) / (targetTime - timeCounter);

            //Increments time counter by last frame time
            timeCounter += Time.deltaTime;
        }
        //Else trigger if the animation has not ended, but should (time left is negative and animation is active)
        else if (targetTime <= timeCounter && timeCounter != 0)
        {
            //Reset variables
            targetTime = 0;
            timeCounter = 0;

            //Set position to target position
            position = targetPosition;
        }

        //Trigger if the target position is above the current position but the wall should not be animating
        if (Mathf.Abs(targetPosition.y) > Mathf.Abs(position.y) && timeCounter == 0)
        {
            //Set target position to current position
            targetPosition = position;
        }

        //Reassign position to transform
        transform.position = position;
    }

    //Method called when the 
    public override void OnSegmentChange(int amount, bool useAnimation)
    {

        //Trigger if animation should be used
        if (useAnimation)
        {
            //Move the target position by the amount of segments the player got
            targetPosition += new Vector3(0, amount * Player.increaseFactor * (transform.position.y < 0 ? -1 : 1));

            //Add time to allow for animation
            targetTime += amount * 0.2f;
        }
        else
        {
            //Else move the object to new position
            transform.position += new Vector3(0, amount * Player.increaseFactor * (transform.position.y < 0 ? -1 : 1));
        }

        //Widen the border wall to account for increasing player fov
        transform.localScale += new Vector3(4 * Player.increaseFactor * Reference.cam.aspect * amount, 0);
    }

    //Method called to set position without messing up the animation
    public void SetPosition(Vector3 position)
    {
        //Sets positions
        transform.position = position;
        targetPosition = position;

        //Resets animation
        targetTime = 0;
        timeCounter = 0;
    }
}
