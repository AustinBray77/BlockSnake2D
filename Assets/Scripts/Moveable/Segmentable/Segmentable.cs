using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class Segmentable : Moveable
{
    //Refrences to the segments following the object
    protected Segment segmentAfter;
    protected Segment segmentLast;
    [SerializeField] protected GameObject segmentPrefab;

    //Variable to store the transforms for the segments to move into, and the times of the rotations and positions
    private List<Vector3> positions;
    private List<Vector3> rotations;
    private List<float> rotPosTimes;
    private int next;

    //Backup for all objects
    protected static int backup;

    //Method used to initiate possibly null objects
    protected void Start()
    {
        //Sets default values for important fields
        SegmentableReset();
        backup = 0;
    }

    //Method used to reset the values of movement values
    protected void SegmentableReset()
    {
        //Sets the default values for fields relating to movement
        positions = new List<Vector3>();
        rotations = new List<Vector3>();
        rotPosTimes = new List<float>();

        //Next is -1 as there is no times in the rotPosTimes
        next = -1;
    }

    #region Movement

    //Method used to move the segments
    protected void MoveSegments()
    {
        //Variables to store the next position to move to
        int next = this.next;
        int newNext = FindNextPosition(next);

        //Saves the transforms for the current time
        positions.Add(transform.position);
        rotations.Add(transform.rotation.eulerAngles);
        rotPosTimes.Add(0);

        //Stores the rotation and position from a certain time period ago
        Vector3 curPos = new Vector3();
        Vector3 curRot = new Vector3();
        Segment curSegment = segmentAfter, segmentBefore = null;

        //Triggers if there is a position to dequeue
        if (newNext > -1 && newNext != next)
        {
            //Save the position and rotation from the index to be dequeued
            curPos = Functions.CopyVector3(positions[newNext]);
            curRot = Functions.CopyVector3(rotations[newNext]);

            //Saves the index into the next variable
            this.next = newNext;
        }

        //Loops while there is a segment to move
        while (curSegment != null)
        {
            //Triggers if there is a position to dequeue
            if (newNext > -1 && newNext != next)
            {
                //Moves the current segment
                MoveSegment(curSegment, segmentBefore, curPos, curRot);
            }

            //Triggers if the currentSegment is the lastSegment
            if (ReferenceEquals(curSegment, segmentLast))
            {
                break;
            }

            //Reassign variables to store the next position to move to
            next = curSegment.next;
            newNext = FindNextPosition(next, curSegment);

            //Triggers if there is a position to dequeue
            if (newNext > -1 && newNext != next)
            {
                //Save the position and rotation from the index to be dequeued
                curPos = Functions.CopyVector3(curSegment.positions[newNext]);
                curRot = Functions.CopyVector3(curSegment.rotations[newNext]);

                //Saves the index into the next variable
                curSegment.next = newNext;
            }

            //Saves the current state of the segment
            curSegment.positions.Add(curSegment.transform.position);
            curSegment.rotations.Add(curSegment.transform.rotation.eulerAngles);
            curSegment.rotPosTimes.Add(0);

            //Moves to next segment
            segmentBefore = curSegment;
            curSegment = curSegment.segmentAfter;
        }
    }

    //Method to find the next position to move to in the root, returns the index of hte next position
    private int FindNextPosition(int next)
    {
        //Loop through each position
        for (int i = next + 1; i < rotPosTimes.Count; i++)
        {
            //Add the last frame time to the rotation position time
            rotPosTimes[i] += Time.deltaTime;

            //Triggers if the time is greater than the time it takes to turn
            if (rotPosTimes[i] >= turnDelay)
            {
                //Assign next variable to current position index
                next = i;
            }
        }

        //Returns the next index to move to
        return next;
    }

    ///Method to find the next position to move to in a segment, returns the index of the next position
    private int FindNextPosition(int next, Segment segment)
    {
        //Loop through each position
        for (int i = next + 1; i < segment.rotPosTimes.Count; i++)
        {
            //Add the last frame time to the rotation position time
            segment.rotPosTimes[i] += Time.deltaTime;

            //Triggers if the time is greater than the time it takes to turn
            if (segment.rotPosTimes[i] >= turnDelay)
            {
                //Assign next variable to current position index
                next = i;
            }
        }

        //Returns the next index to move to
        return next;
    }

    //Method to move a given segment
    private void MoveSegment(Segment segment, Segment segmentBefore, Vector3 position, Vector3 rotation)
    {
        //Sets the segments rotation to the saved rotation
        segment.transform.rotation = Quaternion.Euler(rotation);

        //Triggers if the segment can currently not collide
        if (!segment.canCollide)
        {
            //Triggers if the segment should animate
            if (segment.useAnimation)
            {
                //Animates the segment instantiation
                segment.transform.LeanScale(new Vector2(1, 1), 0.1f);
            }
            else
            {
                //Else don't animate, but set use animation to true to trigger future animations
                segment.transform.localScale = new Vector2(1, 1);
                segment.useAnimation = true;
            }

            //Gets the collider for the segment and sets it to the proper size
            segment.GetComponent<BoxCollider2D>().size = new Vector2(1, 1);

            //Sets the segments position to the saved position
            segment.transform.position = new Vector3(position.x + (Object.speed - 5f) / 6f, position.y);

            //Stores the current position of the segment
            Vector3 newPosition = segment.transform.position;

            //Store the position of the previousSegment, if the before segment is null set it to the first position, else set it to the segment before's position
            Vector3 previousSegmentPosition = segmentBefore == null ? transform.position : segmentBefore.transform.position;

            //Triggers if the position is too far to the right or to the left
            if (newPosition.x + 1f > previousSegmentPosition.x || newPosition.x < previousSegmentPosition.x - 1.5f)
            {
                //Move the position to 1.5 units from the previous segment position
                segment.transform.position = new Vector3(previousSegmentPosition.x - 1.5f, segment.transform.position.y);
            }

            //Flags that the segment can now collide
            segment.canCollide = true;
        }
        else
        {
            //Else move the segment to the y level of the saved position
            segment.transform.position = new Vector3(segment.transform.position.x, position.y);
        }
    }

    //Method called to move the entire player and its segments to a set location and rotation
    protected void MoveWhole(Vector3 position, Vector3 rotation, Segment segment = null)
    {
        //Triggers if no segment is passed as a parameter
        if (segment == null)
        {
            //Sets the position and rotation of the player
            transform.position = position;
            transform.rotation = Quaternion.Euler(rotation);

            //Triggers if the player has a segment
            if (segmentAfter != null)
            {
                //Calls move whole from that segment
                MoveWhole(position - transform.right * 1.5f, rotation, segmentAfter);
            }
        }
        //Else triggers if a segment was passed
        else
        {
            //Sets the segments position and rotation
            segment.transform.position = position;
            segment.transform.rotation = Quaternion.Euler(rotation);

            //Triggers if the segment has a segment after it
            if (segment.segmentAfter != null)
            {
                //Calls move whole from the next segment
                MoveWhole(position - transform.right * 1.5f, rotation, segment.segmentAfter);
            }
        }
    }

    //Method called to stop the movement in all in the segments
    protected void StopAllMovement(Segment segment = null)
    {
        //Triggers if no segment is passed as a parameter
        if (segment == null)
        {
            //Stops the movement in the player
            positions = new List<Vector3>();
            rotations = new List<Vector3>();
            rotPosTimes = new List<float>();
            next = -1;

            //Triggers if the player has a segment
            if (segmentAfter != null)
            {
                //Recursises using that segment
                StopAllMovement(segmentAfter);
            }
        }
        //Else triggers if a segment was passed
        else
        {
            //Stops the movement in the segment
            segment.ResetMovement();

            //Triggers if the segment has a segment after it
            if (segment.segmentAfter != null)
            {
                //Recursises using that segment
                StopAllMovement(segment.segmentAfter);
            }
        }
    }

    //Method called to collect unused times and positions in the segments and player
    protected void CleanMovementBuffers()
    {
        //Trigger if there are positions that are not being used and there is still acitve data in the lists
        if (next + 1 > 0 && positions.Count > next + 1)
        {
            //Skip the unused data (everything before current (next) is unused)
            positions = positions.Skip(next + 1).ToList();
            rotations = rotations.Skip(next + 1).ToList();
            rotPosTimes = rotPosTimes.Skip(next + 1).ToList();
        }
        //Trigger if there is no used data in the lists
        else if (positions.Count <= next + 1)
        {
            //Reset all the lists
            positions = new List<Vector3>();
            rotations = new List<Vector3>();
            rotPosTimes = new List<float>();
        }

        //Reassign next to null position
        next = -1;

        //Stores current segment for Loop
        Segment curSegment = segmentAfter;

        //Loop for each segment
        while (curSegment != null)
        {
            //Trigger if there are positions that are not being used and there is still acitve data in the lists
            if (0 < curSegment.next + 1 && curSegment.positions.Count != curSegment.next + 1)
            {
                //Skip the unused data (everything before current (next) is unused)
                curSegment.positions = curSegment.positions.Skip(curSegment.next + 1).ToList();
                curSegment.rotations = curSegment.rotations.Skip(curSegment.next + 1).ToList();
                curSegment.rotPosTimes = curSegment.rotPosTimes.Skip(curSegment.next + 1).ToList();
            }
            //Trigger if there is no used data in the lists
            else if (positions.Count == next + 1)
            {
                //Reset all the lists
                curSegment.positions = new List<Vector3>();
                curSegment.rotations = new List<Vector3>();
                curSegment.rotPosTimes = new List<float>();
            }

            //Reassign next to null position
            curSegment.next = -1;

            //Moves to the next segment
            curSegment = curSegment.segmentAfter;
        }
    }

    #endregion

    //Method called to add segments to the player
    public void AddSegments(int add, bool useScoreDependance, bool useAnimation = true)
    {
        //Loops to add each segment
        for (int i = 0; i < add; i++)
        {
            //Triggers if there is already one other segement
            if (!ReferenceEquals(segmentLast, null))
            {
                //Creates the new segment and adds refrence to it to the player and segment before it
                GameObject newSegment = Instantiate(segmentPrefab, segmentLast.transform.position, segmentLast.transform.rotation);
                newSegment.transform.parent = transform.parent;
                Segment newSegmentRefrence = newSegment.GetComponent<Segment>();
                newSegmentRefrence.useAnimation = useAnimation;
                segmentLast.segmentAfter = newSegmentRefrence;
                segmentLast = newSegmentRefrence;
            }
            //Triggers if this is the players first segment
            else
            {
                //Creates the new segment and adds refrences to it to the player
                GameObject newSegment = Instantiate(segmentPrefab, transform.position, transform.rotation);
                newSegment.transform.parent = transform.parent;
                Segment newSegmentRefrence = newSegment.GetComponent<Segment>();
                newSegmentRefrence.useAnimation = useAnimation;
                segmentAfter = newSegmentRefrence;
                segmentLast = newSegmentRefrence;
            }

            //Increases the speed of objects
            Object.speed *= 1.01f;
        }

        //Offset add by the backup value
        add += backup;

        if (add > 0)
        {
            //Trigger if the adding of segments has an associated score change
            if (useScoreDependance)
            {
                //Notify all score dependant objects that a segment has been added
                ScoreDependent.OnScoreChange(add, useAnimation);
            }

            //Reset backup
            backup = 0;
        }

        //Trigger if back up was not reset and thus add was negative
        if (backup < 0)
        {
            //Set it to the minimum of add and 0
            backup = Mathf.Min(add, 0);
        }
    }

    //Method called to remove segments from the player
    public void RemoveSegments(int remove, bool useAnimation = true)
    {
        //Variable to store the current segment, the amount of segments removed, and a refrence to all segments
        Segment cur = segmentAfter;
        List<Segment> segmentList = new List<Segment>();

        //Loop while the current segment is not null
        while (!ReferenceEquals(cur, null))
        {
            //Add the segment to the list
            segmentList.Add(cur);

            //Moves on to the next segment
            cur = cur.segmentAfter;
        }

        //Loops to remove the amount of segments
        for (int i = Mathf.Max(0, segmentList.Count - remove); i < segmentList.Count; i++)
        {
            //Set the segment to use animation based on input
            segmentList[i].useAnimation = useAnimation;

            //Destroy the segment
            StartCoroutine(segmentList[i].DestroySegment());
        }

        //Triggers if there are still segments left
        if (segmentList.Count - remove - 1 >= 0)
        {
            //Sets the last segment and its segment after if there are
            segmentLast = segmentList[segmentList.Count - remove - 1];
            segmentLast.segmentAfter = null;
        }
        //Else just sets that there are no segments
        else
        {
            //Sets both segment pointers to null
            segmentLast = null;
            segmentAfter = null;
        }

        //Decrements backup (backup is always below 0)
        backup -= Mathf.Min(remove, segmentList.Count);
    }

    //Counts the segments the player has
    protected int CountSegments()
    {
        //Variable to stroe the current segment and the segment count
        int output = 0;
        Segment cur = segmentAfter;

        //Loop while there is another segment
        while (cur != null)
        {
            //Increment output
            output++;

            //Move to next segment
            cur = cur.segmentAfter;
        }

        //Returns the segment count
        return output;
    }
}
