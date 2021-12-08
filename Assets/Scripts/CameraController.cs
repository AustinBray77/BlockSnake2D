using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Class to control the camera
public class CameraController : MonoBehaviour
{
    //Stores how many iterations it should wait before moving again, { backup <= 0 }
    public int backup;

    //Method called when a segment is added to the player
    public void OnSegmentChange(int amount)
    {
        //Triggers if the camera should wait iterations
        if (backup != 0)
        {
            //Triggers if that iteration amount is less than the amount of segments added
            if (backup + amount > 0)
            {
                //Changes the screen size of the camera
                StartCoroutine(AnimationPlus.ZoomOutOrthagraphic(Refrence.cam, amount, Refrence.cam.orthographicSize + Player.increaseFactor * (backup + amount)));

                //Resets the backup
                backup = 0;
            }
            //Else deiterates the backup by the amount of segments added
            else
            {
                backup += amount;
            }
        }
        //Else changes the screen size of the camera by the desired amount
        else
        {
            StartCoroutine(AnimationPlus.ZoomOutOrthagraphic(Refrence.cam, amount, Refrence.cam.orthographicSize + Player.increaseFactor * amount));
        }
    }
}
