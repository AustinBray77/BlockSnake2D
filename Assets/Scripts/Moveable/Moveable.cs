using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Class to control objects that move
public class Moveable : BaseBehaviour
{
    //Enum for the Y directions
    public enum YDirection
    {
        Up = 1,
        Down = -1
    }

    //Enum for the X directions
    public enum XDirection
    {
        Right = 1,
        Left = -1
    }

    //Properties to control how the object moves
    [HideInInspector] public static float turnDelay;
    [HideInInspector] public float rotSpeed;
    [HideInInspector] public float xSpeed;
    [HideInInspector] public float ySpeed;

    //Method used to move the player
    protected void Move()
    {
        //Move on the x axis by the speed times the relative speed and time, don't move on the y (yet)
        float xMove = xSpeed * Time.deltaTime * Generator.GetRelativeSpeed();
        float yMove = 0;

        //Triggers if the player is to move (ie: not flat on the x axis)
        if (transform.rotation.eulerAngles.z != 0)
        {
            //Triggers if the player is pointing down
            if (transform.rotation.eulerAngles.z >= 270)
            {
                //Calculates how much up the player should move based on the steepness of its angle of attack
                yMove = -1 * Time.deltaTime * ySpeed * ((transform.rotation.eulerAngles.z - 360) * -1 / 90);
            }
            //Triggers if the polayer is pointing up
            else
            {
                //Calculates how much up the player should move based on the steepness of its angle of attack
                yMove = Time.deltaTime * ySpeed * (transform.rotation.eulerAngles.z / 90);
            }
        }

        //Move the moveable by the determined values
        transform.position += new Vector3(xMove, yMove);
    }

    //Method called to rotate the player
    protected void Rotate(YDirection dir)
    {
        //Calculates the next z axis rotation
        float nextZ = transform.rotation.eulerAngles.z + (rotSpeed * (int)dir * Time.deltaTime);

        //Assigns the new rotation locked between 90 and 270 on the z axis
        transform.rotation = Quaternion.Euler(0, 0,
            nextZ < 270 && nextZ > 90 ?
                dir == YDirection.Down ?
                    270 : 90
                        : nextZ);
    }

    //Method called to increase the moveables rotation speed
    public void IncreaseRotSpeed(float value) =>
        rotSpeed *= value;

    //Method called to decrease the moveables rotation speed
    public void DecreaseRotSpeed(float value) =>
        rotSpeed /= value;

    //Method called to increase the moveables yspeed
    public void IncreaseYSpeed(float value) =>
        ySpeed *= value;

    //Method called to decrease the moveables yspeed
    public void DecreaseYSpeed(float value) =>
        ySpeed /= value;
}
