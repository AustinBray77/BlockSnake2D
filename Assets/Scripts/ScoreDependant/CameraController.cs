using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
//Class to control the camera
public class CameraController : ScoreDependent
{
    //Variables for animation
    private float targetSize;
    private float timeCounter;
    private float targetTime;

    //Method called on scene instatiation
    private void Start()
    {
        //Sets variables to base values
        timeCounter = 0;
        targetTime = 0;

        //Gets fov from camera componenet on the object
        targetSize = GetComponent<Camera>().orthographicSize;
    }

    //Method called on every frame
    private void Update()
    {
        //Trigger if the player is not dead or at a finish
        if (!Player.isDead && !Player.isAtFinish)
        {
            //Move the camera by the object speed
            transform.position += new Vector3(Object.speed * Time.deltaTime * Generator.GetRelativeSpeed(), 0);
        }

        //Animate the camera
        HandleAnimation();
    }

    //Method to handle the animation of the camera size
    private void HandleAnimation()
    {
        //Triggers if the object should still be animating (time left in animation)
        if (targetTime > timeCounter)
        {
            //Increase cam fov size by the desired amount depending on size difference and time left
            Reference.cam.orthographicSize += Time.deltaTime * (targetSize - Reference.cam.orthographicSize) / (targetTime - timeCounter);

            //Increment time counter by last frame time
            timeCounter += Time.deltaTime;
        }
        //Else trigger if the animation has not ended, but should (time left is negative and animation is active)
        else if (targetTime <= timeCounter && timeCounter != 0)
        {
            //Reset animation variables
            targetTime = 0;
            timeCounter = 0;

            //Set size to target size
            Reference.cam.orthographicSize = targetSize;
        }
    }

    //Method called when a segment is added to the player
    public override void OnSegmentChange(int amount, bool useAnimation)
    {
        //Trigger if animation is to be used
        if (useAnimation)
        {
            //Increase target fov size by amount of segments added
            targetSize += Player.increaseFactor * amount;

            //Add time to allow for animation
            targetTime += amount * 0.2f;
        }
        else
        {
            //Else increase fov size by amount of segments added
            Reference.cam.orthographicSize += Player.increaseFactor * amount;
        }
    }

    //Method to set the camera fov size and override animation
    public void SetSize(float size)
    {
        //Sets size
        Reference.cam.orthographicSize = size;
        targetSize = size;

        //Resets variables
        targetTime = 0;
        timeCounter = 0;
    }
}
