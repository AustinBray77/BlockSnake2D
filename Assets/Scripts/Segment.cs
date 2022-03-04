using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Class to handle the segments which spawn behind the player
//[RequireComponent(typeof(Rigidbody2D))]
public class Segment : MonoBehaviour
{
    //Refrence to the segment after the current segment
    [HideInInspector] public Segment segmentAfter;

    //Refrence to this obejcts collider and whether it should collide
    private BoxCollider2D _collider;
    public bool canCollide;

    //Stores the previous transforms of the segment
    public List<Vector3> positions;
    public List<Vector3> rotations;
    public List<float> rotPosTimes;
    public int next;

    //Variable to store if animations should be used on instatiation
    public bool useAnimation = true;

    private Vector3 constantPosition;

    //Called on the objects instantiation
    private void Awake()
    {
        gameObject.transform.localScale = new Vector2(0, 0);

        //Sets the default values for the collider
        _collider = GetComponent<BoxCollider2D>();
        canCollide = false;
        _collider.size = new Vector2(0, 0);

        positions = new List<Vector3>();
        rotations = new List<Vector3>();
        rotPosTimes = new List<float>();
        next = -1;
    }

    private void Update()
    {
        if (!Gamemode.inLevel("Start"))
        {
            if (Mathf.Abs(constantPosition.y) >= Reference.wallTop.transform.position.y)
            {
                Reference.player.shieldCount = 0;
                Reference.player.KillPlayer();
            }
        }

        if (!Player.isAtFinish && !Player.isDead)
        {
            transform.position += new Vector3(Object.speed * Time.deltaTime * Generator.GetRelativeSpeed(), 0);
        }
    }

    //Stops all the movement in the segment
    public void StopMovement()
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
        if (useAnimation)
        {
            //Animates the shrinkage of the segment
            gameObject.LeanScale(new Vector2(0, 0), 0.1f);
            yield return new WaitForSeconds(0.1f);
        }

        //Destroys the segment
        Destroy(gameObject);
    }

    public void UpdateConstantPosition() =>
        constantPosition = transform.position;
}
