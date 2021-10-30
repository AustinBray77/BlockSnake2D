using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Class to handle the segments which spawn behind the player
[RequireComponent(typeof(Rigidbody2D))]
public class Segment : MonoBehaviour
{
    //Refrence to the segment after the current segment
    [HideInInspector] public Segment segmentAfter;
    
    //Refrence to this obejcts collider and whether it should collide
    private BoxCollider2D _collider;
    private bool canCollide;

    //Called on the objects instantiation
    private void Awake()
    {
        //Sets the default values for the collider
        _collider = GetComponent<BoxCollider2D>();
        canCollide = false;
        _collider.size = new Vector2(0, 0);
    }

    //Method called to set the movement of the segment
    public void SetMovement(Vector3 previousPosition, Vector3 previousRotation, float turnDelay)
    {
        //Triggers if the segment has a segment after it
        if (segmentAfter != null)
        {
            //Moves the segment after it
            StartCoroutine(MoveNext(transform.position - (transform.right * 1.5f), transform.rotation.eulerAngles, turnDelay));
        }

        //Sets the rotation and position
        transform.rotation = Quaternion.Euler(previousRotation);
        transform.position = previousPosition;

        //Triggers if the segment was not yet set to collide
        if(!canCollide)
        {
            //Allows the segment to collide, and sizes the collider
            canCollide = true;
            _collider.size = new Vector2(1, 1);
            _collider.offset = new Vector2(0, 0);
        }
    }

    //Enumerator to move the segment after this segment
    private IEnumerator MoveNext(Vector3 position, Vector3 rotation, float turnDelay)
    {
        //Variable to count the time
        float time = 0;

        //Waits for turn delay seconds
        while (time < turnDelay)
        {
            //Adds a set amount of time to time and waits the time
            time += Time.fixedDeltaTime;
            yield return new WaitForSeconds(Time.fixedDeltaTime);

            //If the player is at finish, wait until they aren't
            yield return new WaitUntil(() => { return !Player.isAtFinish; });

            //If the player removes the segment after this one while at the finish, break out of the loop
            if (segmentAfter == null)
            {
                break;
            }
        }

        //Triggers if there is still a segment after this one
        if (segmentAfter != null)
        {
            //Sets the movement of the segment after this one
            segmentAfter.SetMovement(position, rotation, turnDelay);
        }
    }

    //Method called on the gameobjects destruction
    public void OnDestroy()
    {
        //Stops all the coroutines
        StopAllCoroutines();
    }
}
