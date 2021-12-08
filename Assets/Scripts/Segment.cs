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

    //Stores the previous transforms of the segment
    private Queue<Vector3> positions;
    private Queue<Vector3> rotations;

    //Variables to store when the player should begin dequeuing the transforms
    private float timeCounter;
    private bool beginDequeing;

    //Called on the objects instantiation
    private void Awake()
    {
        //Animates the segment instantiation
        gameObject.transform.localScale = new Vector2(0, 0);
        gameObject.LeanScale(new Vector2(1, 1), 1f);

        //Sets the default values for the collider
        _collider = GetComponent<BoxCollider2D>();
        canCollide = false;
        _collider.size = new Vector2(0, 0);

        positions = new Queue<Vector3>();
        rotations = new Queue<Vector3>();

        timeCounter = 0;
        beginDequeing = false;
    }

    //Method called to set the movement of the segment
    public void SetMovement(Vector3 previousPosition, Vector3 previousRotation)
    {
        //Waits the turn delay before dequeing the tranforms
        if (!beginDequeing)
            timeCounter += Time.deltaTime;

        if (timeCounter >= Refrence.player.turnDelay)
            beginDequeing = true;

        //Sets the rotation and position
        transform.rotation = Quaternion.Euler(previousRotation);
        transform.position = previousPosition;

        //Saves the transforms
        positions.Enqueue(previousPosition);
        rotations.Enqueue(previousRotation);

        //Triggers if the current transform should be popped
        if (beginDequeing)
        {
            //Gets the rotation and position from a certain time peroid ago
            Vector3 curPos = positions.Dequeue();
            Vector3 curRot = rotations.Dequeue();

            //Triggers if there is a segment after
            if (segmentAfter != null)
            {
                //Sets the segments movement
                segmentAfter.SetMovement(curPos + -1.5f * Vector3.right, curRot);
            }
        }

        //Triggers if the segment was not yet set to collide
        if (!canCollide)
        {
            //Allows the segment to collide, and sizes the collider
            canCollide = true;
            _collider.size = new Vector2(1, 1);
            _collider.offset = new Vector2(0, 0);
        }
    }

    //Stops all the movement in the segment
    public void StopMovement()
    {
        //Sets movement values to default
        positions = new Queue<Vector3>();
        rotations = new Queue<Vector3>();
        beginDequeing = false;
        timeCounter = 0;
    }

    //Method called to destroy the segment
    public IEnumerator DestroySegment()
    {
        //Animates the shrinkage of the segment
        gameObject.LeanScale(new Vector2(0, 0), 1f);
        yield return new WaitForSeconds(1);

        //Destroys the segment
        Destroy(gameObject);
    }
}
