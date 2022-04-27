using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Class to handle the segments which spawn behind the player
//[RequireComponent(typeof(Rigidbody2D))]
public class Segment : BaseBehaviour
{
    //Refrence to the segment after the current segment
    [HideInInspector] public Segment segmentAfter;

    //Refrence to this obejcts collider and whether it should collide
    private BoxCollider2D _collider;
    public bool canCollide;

    //Stores the previous transforms (rotation, position, time) of the segment
    public List<Vector3> positions;
    public List<Vector3> rotations;
    public List<float> rotPosTimes;

    //Stores the index of the next item in the transforms lists
    public int next;

    //Variable to store if animations should be used on instatiation
    public bool useAnimation = true;

    //Method called on the objects instantiation
    private void Awake()
    {
        //Sets the segment to be invisible
        gameObject.transform.localScale = new Vector2(0, 0);

        //Sets the default values for the collider
        _collider = GetComponent<BoxCollider2D>();
        canCollide = false;
        _collider.size = new Vector2(0, 0);

        //Sets default values of transforms lists
        ResetMovement();
    }

    //Method called each frame
    private void Update()
    {
        //Trigger if the player is not in the home screen
        if (!Gamemanager.InLevel("Start"))
        {
            //Trigger if the segment is above or below the walls position
            if (Mathf.Abs(transform.position.y) >= Reference.wallTop.transform.position.y)
            {
                //Kill the player
                Reference.player.KillPlayer(true);
            }
        }

        //Trigger if the player is not dead or at finish
        if (!Player.isAtFinish && !Player.isDead)
        {
            //Moves the segment by the object speed and relative game speed over time
            transform.position += new Vector3(Object.speed * Time.deltaTime * Generator.GetRelativeSpeed(), 0);
        }
    }

    //Method to stop reset all the movement in the segment
    public void ResetMovement()
    {
        //Sets movement values to default
        positions = new List<Vector3>();
        rotations = new List<Vector3>();
        rotPosTimes = new List<float>();
        next = -1;
    }

    //Method called to destroy the segment
    public IEnumerator DestroySegment()
    {
        //Trigger if the segment should use animations
        if (useAnimation)
        {
            //Animates the shrinkage of the segment
            gameObject.LeanScale(new Vector2(0, 0), 0.1f);

            //Waits for the segment to shrink
            yield return new WaitForSeconds(0.1f);
        }

        //Destroys the segment
        Destroy(gameObject);
    }
}
