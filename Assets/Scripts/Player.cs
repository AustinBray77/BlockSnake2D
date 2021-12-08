using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

//Class to contorl the player
[RequireComponent(typeof(Rigidbody2D))]
public class Player : MonoBehaviour
{
    //Refrences to the segments following the player
    [SerializeField] private Segment segmentAfter;
    [SerializeField] private Segment segmentLast;
    [SerializeField] private GameObject segmentPrefab;

    //Refrence to the fade panel
    [SerializeField] private Image fadePanel;

    private Rigidbody2D rb;

    //Properties to control how the player moves
    [HideInInspector] public float turnDelay;
    [HideInInspector] public float speed;
    [HideInInspector] public float rotSpeed;

    //Saves the last card the player upgraded
    [HideInInspector] public Card lastUpgraded;

    //Saves the move buttons the user is touching
    private bool[] moveArr = new bool[] { false, false };

    //Stores how much objects should move eacht ime a segment is added
    public static float increaseFactor = 1.5f;

    //Stores whether the player is dead or at a finish
    public static bool isDead, isAtFinish;

    //Stores the score, level, gear count
    public static int score, gearCount;
    public static Level level;

    //Stores variables at the last finish.
    public static int scoreAtLastFinish, segmentCountAtLastFinish, backupAtLastFinish;
    public static float xBoundAtLastFinish, yBoundAtLastFinish;

    //Stores the starting bounds of the camera
    public static int xStartBounds = 30, yStartBounds = 12;
    
    //Property to get the active skin
    public static Skin activeSkin => 
        Shop_UI.skins[Serializer.activeData.activeSkin];

    //Variable which stores the offset of how many iterations the camera should wait before moving again, { backup <= 0 }
    private int camBackup;

    //Variable to store the transforms for the segments to move into
    private Queue<Vector3> positions;
    private Queue<Vector3> rotations;

    //Variables to store when the player should begin dequeuing the transforms
    private float timeCounter;
    private bool beginDequeing;

    //Variable to store the amount of shields the player has left
    public int shieldCount;

    //Method called on scene load
    private void Start()
    {
        //Fades in the scene
        StartCoroutine(AnimationPlus.FadeToColor(fadePanel, new Color(0, 0, 0, 0), UI.fadeTime, false));

        //Assigns the default vaules for the variables
        rb = GetComponent<Rigidbody2D>();
        isDead = false;
        isAtFinish = false;
        score = 0;
        scoreAtLastFinish = 0;
        camBackup = 0;
        turnDelay = 0.2f;
        speed = 100000;
        rotSpeed = 10;
        shieldCount = 0;

        if (!Gamemode.inLevel("Tutorial"))
        {
            gearCount = Serializer.activeData.gearCount;
            level = new Level(Serializer.activeData.level.xp);
        }

        GetComponent<SpriteRenderer>().sprite = activeSkin.frontSprite;
        segmentPrefab.GetComponent<SpriteRenderer>().sprite = activeSkin.segmentSprite;

        positions = new Queue<Vector3>();
        rotations = new Queue<Vector3>();

        timeCounter = 0;
        beginDequeing = false;
    }

    //Method called on each frame
    private void Update()
    {
        //Waits the turn delay before dequeing the tranforms
        if (!beginDequeing)
            timeCounter += Time.deltaTime;

        if (timeCounter >= turnDelay)
            beginDequeing = true;

        //Sets the velocity to 0 if the player is dead or at a finish
        if (isAtFinish || isDead)
        {
            rb.velocity = new Vector2(0, 0);
        }

        //Sets the angular velocity to zore
        rb.angularVelocity = 0;

        //Moves up if the player is hitting the up or w button
        if (Input.GetKey(KeyCode.W) || moveArr[0])
        {
            Move(1);
        }

        //Move down if the player if the down or s button
        if(Input.GetKey(KeyCode.S) || moveArr[1])
        {
            Move(-1);
        }

        //Triggers if the player is to move
        if (transform.rotation.eulerAngles.z != 0 && !isAtFinish && !isDead)
        {
            //Triggers if the player is pointing down
            if(transform.rotation.eulerAngles.z >= 270)
            {
                //Resets the velocity
                rb.velocity = new Vector2(0, 0);
                //Adds downward velocity
                rb.AddForce(Vector2.up * Time.deltaTime * -speed * ((transform.rotation.eulerAngles.z - 360) * -1 / 90));
            }
            //Triggers if the polayer is pointing up
            else
            {
                //Resets the velocity
                rb.velocity = new Vector2(0, 0);
                //Adds upward velocity
                rb.AddForce(Vector2.up * Time.deltaTime * speed * (transform.rotation.eulerAngles.z / 90));
            }
        }

        //Triggers if the player is not dead or at finish
        if (!isAtFinish && !isDead)
        {
            //Saves the transforms
            positions.Enqueue(transform.position);
            rotations.Enqueue(transform.rotation.eulerAngles);

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
        }
    }
    
    //Method called when the player clicks down on a movement button
    public void OnMovePointerDown(int button)
    {
        moveArr[button] = true;
    }

    //Method called when the player unclicks on a movement button
    public void OnMovePointerUp(int button)
    {
        moveArr[button] = false;
    }

    //Method called to rotate the player
    public void Move(int direction)
    {
        //Calculates the next z axis rotation
        float nextZ = transform.rotation.eulerAngles.z + (rotSpeed * direction * Time.deltaTime);

        //Assigns the new rotation
        transform.rotation = Quaternion.Euler(0, 0, 
            nextZ < 270 && nextZ > 90 ? 
                direction == -1 ? 
                    270 : 90 
                        : nextZ);
    }

    //Method called when the player is to be killed
    public void KillPlayer()
    {
        //Uses shield if the player has one
        if(shieldCount >= 1)
        {
            UseShield();
            return;
        }

        //Stops all couroutines in the following segments
        StopAllMovement();

        //Triggers if not in the tutorial
        if (!Gamemode.inLevel("Tutorial"))
        {
            //If the players achieved a new highscore, the highscore is set to the score
            if (Serializer.activeData.highScore < score)
            {
                Serializer.activeData.SetHighScore(score);
            }

            //Sets the gear count in the serializer
            Serializer.activeData.SetGearCount(gearCount);
        }

        //Triggers that the player is dead
        isDead = true;

        //Tells the generator that the player is dead
        Refrence.gen.OnPlayerDeath();

        //Hides the game UI and shows the death UI
        Refrence.gameUI.Hide();
        Refrence.deathUI.Show();
    }

    //Method called when the player is to respawn
    public void Respawn()
    {
        //Removes the level from the last upgraded card
        lastUpgraded.RemoveLevel();

        //Moves the entire player to the middle of the screen
        MoveWhole(new Vector3(0, 0), new Vector3(0, 0, 0));

        //Sets the properties to there values at last finish
        score = scoreAtLastFinish;
        Refrence.cam.orthographicSize = yBoundAtLastFinish;
        Refrence.wallTop.transform.position = new Vector3(0, yBoundAtLastFinish - 0.5f);
        Refrence.wallBottom.transform.position = new Vector3(0, -yBoundAtLastFinish + 0.5f);
        Refrence.gen.transform.position = new Vector3(xBoundAtLastFinish, 0);
        Generator.SetBounds(yBoundAtLastFinish - 2);
        Refrence.des.transform.position = new Vector3(-xBoundAtLastFinish, 0);
        SetBackups(backupAtLastFinish);

        //Resets the segment count to the segment count at last finish
        if (CountSegments() - segmentCountAtLastFinish > 0)
        {
            RemoveSegments(CountSegments() - segmentCountAtLastFinish);
        }
        else if (CountSegments() - segmentCountAtLastFinish < 0)
        {
            AddSegments((CountSegments() - segmentCountAtLastFinish) * -1);
        }

        //Shows the game UI, sets the score text, and sets is dead to false
        Refrence.gameUI.Show();
        Refrence.gameUI.UpdateScore(score);
        isDead = false;
    }

    //Method called when the player enters a finish
    public void OnEnterFinish()
    {
        //Sets the trigger for at finish to true
        isAtFinish = true;

        //Saves the values to the at last finish refrence
        segmentCountAtLastFinish = CountSegments();
        xBoundAtLastFinish = Refrence.gen.transform.position.x;
        yBoundAtLastFinish = Refrence.cam.orthographicSize;
        backupAtLastFinish = camBackup;
        scoreAtLastFinish = score;
    }

    //Method called to add gears to the player
    public void AddGears(int amount)
    {
        //Adds the gears
        gearCount += amount;

        //Binds the gear count to 9999
        if (gearCount > 9999)
        {
            gearCount = 9999;
        }

        //Sets the gear text to the current gear count
        Refrence.gameUI.UpdateGearCount(gearCount);

        //Adds XP
        level.AddXP(amount);
    }

    //Method called to add score to the player
    public void AddScore(int add)
    {
        //Adds the score
        score += add;

        //Sets the score text to the current score
        Refrence.gameUI.UpdateScore(score);

        //Triggers if not in the tutorial
        if (!Gamemode.inLevel("Tutorial"))
        {
            //Adds XP
            level.AddXP(add);
        }

        //Adds Segments
        AddSegments(add);
    }

    //Method called to add segments to the player
    public void AddSegments(int add)
    {
        //Loops to add each segment
        for (int i = 0; i < add; i++)
        {
            //Triggers if there is already one other segement
            if (segmentLast != null)
            {
                //Creates the new segment and adds refrence to it to the player and segment before it
                GameObject newSegment = Instantiate(segmentPrefab, segmentLast.transform.position - new Vector3(1.5f, 0f), segmentLast.transform.rotation);
                segmentLast.segmentAfter = newSegment.GetComponent<Segment>();
                segmentLast = newSegment.GetComponent<Segment>();
            }
            //Triggers if this is the players first segment
            else
            {
                //Creates the new segment and adds refrences to it to the player
                GameObject newSegment = Instantiate(segmentPrefab, transform.position - new Vector3(1.5f, 0f), transform.rotation);
                segmentAfter = newSegment.GetComponent<Segment>();
                segmentLast = newSegment.GetComponent<Segment>();
            }

            //Increases the speed of objects
            Object.speed *= 1.01f;
        }

        //Calls on segment change in the other objects
        Refrence.camController.OnSegmentChange(add);
        Refrence.gen.OnSegmentChange(add);
        Refrence.des.OnSegmentChange(add);
        Refrence.wallTop.OnSegmentChange(add);
        Refrence.wallBottom.OnSegmentChange(add);
    }

    //Method called to set the backups in the other objects
    public void SetBackups(int amount)
    {
        //Sets the backups
        camBackup = amount;
        Refrence.gen.backup = amount;
        Refrence.des.backup = amount;
        Refrence.wallTop.backup = amount;
        Refrence.wallBottom.backup = amount;
    }

    //Method called to remove segments from the player
    public void RemoveSegments(int remove)
    {
        //Variable to store the current segment, the amount of segments removed, and a refrence to all segments
        Segment cur = segmentAfter;
        int amountRemoved = 0;
        List<Segment> segmentList = new List<Segment>();

        //Gets all segmetns
        while(cur != null)
        {
            segmentList.Add(cur);
            cur = cur.segmentAfter;
        }

        //Loops to remove the amount of segments
        for(int i = Mathf.Max(0, segmentList.Count - remove); i < segmentList.Count; i++, amountRemoved++)
        {
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
    }

    //Method called to move the entire player and its segments to a set location and rotation
    private void MoveWhole(Vector3 position, Vector3 rotation, Segment segment = null)
    {
        //Triggers if no segment is passed as a parameter
        if (segment == null)
        {
            //Sets the position and rotation of the player
            transform.position = position;
            transform.rotation = Quaternion.Euler(rotation);

            //Triggers if the player has a segment
            if(segmentAfter != null)
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
    private void StopAllMovement(Segment segment = null)
    {
        //Triggers if no segment is passed as a parameter
        if(segment == null)
        {
            //Stops the movement in the player
            positions = new Queue<Vector3>();
            rotations = new Queue<Vector3>();
            beginDequeing = false;
            timeCounter = 0;

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
            if(segment.segmentAfter != null)
            {
                //Recursises using that segment
                StopAllMovement(segment.segmentAfter);
            }
        }
    }

    //Counts the segments the player has
    private int CountSegments()
    {
        //Variable to stroe the current segment and the segment count
        int output = 0;
        var cur = segmentAfter;

        //Loop while there is another segment
        while(cur != null)
        {
            output++;
            cur = cur.segmentAfter;
        }

        //Returns the segment count
        return output;
    }

    //Method called to get which of the current control buttons are being pressed down
    public bool[] GetControls() =>
        moveArr;

    //Method called to add shields to the player
    public void AddShields(int count)
    {
        //Adds shields and updates the UI
        shieldCount += count;
        Refrence.gameUI.UpdateShieldCount(shieldCount);
    }

    //Method called when the player uses a shield instead of dying
    public void UseShield()
    {
        //Removes a shield and updates the UI
        shieldCount--;
        Refrence.gameUI.UpdateShieldCount(shieldCount);
    }
}
