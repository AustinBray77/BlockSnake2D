using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
//Class to control the camera
public class CameraController : MonoBehaviour
{
    //Variables for animation
    private float targetSize;
    private float timeCounter;
    private float targetTime;

    //Method called on scene instatiation
    private void Start()
    {
        timeCounter = 0;
        targetTime = 0;
        targetSize = GetComponent<Camera>().orthographicSize;
    }

    //Method called on every frame
    private void Update()
    {
        if (!Player.isDead && !Player.isAtFinish)
        {
            transform.position += new Vector3(Object.speed * Time.deltaTime * Generator.GetRelativeSpeed(), 0);
        }

        if (targetTime > timeCounter)
        {
            Refrence.cam.orthographicSize += Time.deltaTime * (targetSize - Refrence.cam.orthographicSize) / (targetTime - timeCounter);
            timeCounter += Time.deltaTime;
        }
        else if (targetTime <= timeCounter && timeCounter != 0)
        {
            targetTime = 0;
            timeCounter = 0;
            Refrence.cam.orthographicSize = targetSize;
        }
    }

    //Method called when a segment is added to the player
    public void OnSegmentChange(int amount, bool useAnimation)
    {
        //Changes the screen size of the camera, uses animation if requested
        if (useAnimation)
        {
            targetSize += Player.increaseFactor * amount;
            targetTime += amount * 0.2f;
        }
        else
        {
            Refrence.cam.orthographicSize += Player.increaseFactor * amount;
        }
    }
}
