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
    protected List<Vector3> positions;
    protected List<Vector3> rotations;
    protected List<float> rotPosTimes;
    protected int next;

    //Backup for all objects
    protected static int backup;

    //Method used to initiate possibly null objects
    protected void Segmentable_Init()
    {
        positions = new List<Vector3>();
        rotations = new List<Vector3>();
        rotPosTimes = new List<float>();
        next = -1;
        backup = 0;
    }

    //Method used to move the segments
    protected void MoveSegments()
    {
        int next = this.next;
        int newNext = next;

        for (int i = newNext + 1; i < rotPosTimes.Count; i++)
        {
            rotPosTimes[i] += Time.deltaTime;

            if (rotPosTimes[i] >= turnDelay)
            {
                newNext = i;
            }
        }

        //Saves the transforms
        positions.Add(transform.position);
        rotations.Add(transform.rotation.eulerAngles);
        rotPosTimes.Add(0);

        //Gets the rotation and position from a certain time peroid ago
        Vector3 curPos = new Vector3();
        Vector3 curRot = new Vector3();
        Segment curSegment = segmentAfter, segmentBefore = null;

        if (newNext > -1 && newNext != next)
        {
            curPos = Functions.CopyVector3(positions[newNext]);
            curRot = Functions.CopyVector3(rotations[newNext]);

            this.next = newNext;
        }

        while (curSegment != null)
        {
            if (newNext > -1 && newNext != next)
            {
                curSegment.transform.rotation = Quaternion.Euler(curRot);

                if (!curSegment.canCollide)
                {
                    if (curSegment.useAnimation)
                    {
                        //Animates the segment instantiation
                        curSegment.transform.LeanScale(new Vector2(1, 1), 0.1f);
                    }
                    else
                    {
                        //Otherwise don't animate, but set use animation to true to trigger future animations
                        curSegment.transform.localScale = new Vector2(1, 1);
                        curSegment.useAnimation = true;
                    }

                    curSegment.GetComponent<BoxCollider2D>().size = new Vector2(1, 1);

                    curSegment.transform.position = new Vector3(curPos.x + (Object.speed - 5f) / 6f, curPos.y);
                    curSegment.canCollide = true;

                    Vector3 segPos = curSegment.transform.position;

                    if (segmentBefore == null)
                    {
                        if (segPos.x + 1f > transform.position.x)
                        {
                            curSegment.transform.position = new Vector3(transform.position.x - 1.5f, curSegment.transform.position.y);
                        }
                        else if (segPos.x < transform.position.x - 1.5f)
                        {
                            curSegment.transform.position = new Vector3(transform.position.x - 1.5f, curSegment.transform.position.y);
                        }
                    }
                    else
                    {
                        if (segPos.x + 1f > segmentBefore.transform.position.x)
                        {
                            curSegment.transform.position = new Vector3(segmentBefore.transform.position.x - 1.5f, curSegment.transform.position.y);
                        }
                        else if (segPos.x < segmentBefore.transform.position.x - 1.5f)
                        {
                            curSegment.transform.position = new Vector3(segmentBefore.transform.position.x - 1.5f, curSegment.transform.position.y);
                        }
                    }

                }
                else
                {
                    curSegment.transform.position = new Vector3(curSegment.transform.position.x, curPos.y);
                }

                //curSegment.UpdateConstantPosition();
            }

            if (ReferenceEquals(curSegment, segmentLast))
            {
                break;
            }

            next = curSegment.next;
            newNext = next;

            for (int i = newNext + 1; i < curSegment.rotPosTimes.Count; i++)
            {
                curSegment.rotPosTimes[i] += Time.deltaTime;

                if (curSegment.rotPosTimes[i] >= turnDelay)
                {
                    newNext = i;
                }
            }

            if (newNext > -1 && newNext != next)
            {
                curPos = Functions.CopyVector3(curSegment.positions[newNext]);
                curRot = Functions.CopyVector3(curSegment.rotations[newNext]);

                curSegment.next = newNext;
            }

            curSegment.positions.Add(curSegment.transform.position);
            curSegment.rotations.Add(curSegment.transform.rotation.eulerAngles);
            curSegment.rotPosTimes.Add(0);

            segmentBefore = curSegment;
            curSegment = curSegment.segmentAfter;
        }
    }


    //Method called to add segments to the player
    public void AddSegments(int add, bool useAnimation = true)
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

        add += backup;

        if (add > 0)
        {
            //Calls on segment change in the other objects
            Reference.camController.OnSegmentChange(add, useAnimation);
            Reference.gen.OnSegmentChange(add);
            Reference.wallTop.OnSegmentChange(add, useAnimation);
            Reference.wallBottom.OnSegmentChange(add, useAnimation);
            backup = 0;
        }

        if (backup < 0)
        {
            backup = Mathf.Min(add, 0);
        }
    }

    //Method called to remove segments from the player
    public void RemoveSegments(int remove, bool useAnimation = true)
    {
        //Variable to store the current segment, the amount of segments removed, and a refrence to all segments
        Segment cur = segmentAfter;
        List<Segment> segmentList = new List<Segment>();

        //Gets all segmetns
        while (!ReferenceEquals(cur, null))
        {
            segmentList.Add(cur);
            cur = cur.segmentAfter;
        }

        //Loops to remove the amount of segments, uses animation based on the input
        for (int i = Mathf.Max(0, segmentList.Count - remove); i < segmentList.Count; i++)
        {
            segmentList[i].useAnimation = useAnimation;
            StartCoroutine(segmentList[i].DestroySegment());
        }

        //Triggers if there are still segments left
        if (segmentList.Count - remove - 1 >= 0)
        {
            //Sets the last segment and its segment after if there are
            segmentLast = segmentList[segmentList.Count - remove - 1];
            segmentLast.segmentAfter = null;
        }
        //Else just sets that there are no segment
        else
        {
            segmentLast = null;
            segmentAfter = null;
        }

        backup -= Mathf.Min(remove, segmentList.Count);
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
            segment.StopMovement();

            //Triggers if the segment has a segment after it
            if (segment.segmentAfter != null)
            {
                //Recursises using that segment
                StopAllMovement(segment.segmentAfter);
            }
        }
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
            output++;
            cur = cur.segmentAfter;
        }

        //Returns the segment count
        return output;
    }

    //Method called to collect unused times and positions in the segments and player
    protected void CleanMovementBuffers()
    {
        if (next + 1 > 0 && positions.Count > next + 1)
        {
            positions = positions.Skip(next + 1).ToList();
            rotations = rotations.Skip(next + 1).ToList();
            rotPosTimes = rotPosTimes.Skip(next + 1).ToList();
        }
        else if (positions.Count <= next + 1)
        {
            positions = new List<Vector3>();
            rotations = new List<Vector3>();
            rotPosTimes = new List<float>();
        }

        next = -1;

        Segment curSegment = segmentAfter;

        while (curSegment != null)
        {
            if (0 < curSegment.next + 1 && curSegment.positions.Count != curSegment.next + 1)
            {
                curSegment.positions = curSegment.positions.Skip(curSegment.next + 1).ToList();
                curSegment.rotations = curSegment.rotations.Skip(curSegment.next + 1).ToList();
                curSegment.rotPosTimes = curSegment.rotPosTimes.Skip(curSegment.next + 1).ToList();
            }
            else if (positions.Count == next + 1)
            {
                curSegment.positions = new List<Vector3>();
                curSegment.rotations = new List<Vector3>();
                curSegment.rotPosTimes = new List<float>();
            }

            curSegment.next = -1;

            curSegment = curSegment.segmentAfter;
        }
    }
}
