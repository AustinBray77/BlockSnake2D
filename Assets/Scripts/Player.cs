using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

//Class to contorl the player
[RequireComponent(typeof(AudioSource))]
public class Player : MonoBehaviour
{
    //Enum for movement types
    public enum MovementType
    {
        Buttons,
        Mouse,
        Keyboard
    }

    //Refrences to the segments following the player
    private Segment segmentAfter;
    private Segment segmentLast;
    [SerializeField] private GameObject segmentPrefab;

    //Refrence to the fade panel
    [SerializeField] private Image fadePanel;

    //Properties to control how the player moves
    [HideInInspector] public static float turnDelay;
    [HideInInspector] public float speed;
    [HideInInspector] public float rotSpeed;

    //Saves the last card the player upgraded
    [HideInInspector] public Card lastUpgraded;

    //Saves the move buttons the user is touching
    private bool[] moveArr = new bool[] { false, false };

    //Stores how much objects should move each time a segment is added
    public static float increaseFactor = 1.5f;

    //Stores whether the player is dead or at a finish
    public static bool isDead, isAtFinish;

    //Stores the score, level, gear count
    public static int score, gearCount;
    public static Level level;

    //Stores variables at the last finish.
    public static int scoreAtLastFinish, segmentCountAtLastFinish, backupAtLastFinish;
    public static float xBoundAtLastFinish, yBoundAtLastFinish;
    private static Vector3 positionAtLastFinish;

    //Stores the starting bounds of the camera
    public static int xStartBounds = 30, yStartBounds = 12;

    //Property to get the active skin
    public static Skin activeSkin =>
        Shop_UI.skins[Serializer.activeData.activeSkin];

    //Variable to store the transforms for the segments to move into, and the times of the rotations and positions
    private List<Vector3> positions;
    private List<Vector3> rotations;
    private List<float> rotPosTimes;
    private int next;

    //Variable to store the amount of shields the player has left
    public int shieldCount;

    //Variable to store the slow down percentage of the slow down object
    private static float slowdownPercentage;

    //Variable to store the delay of the slow down power up
    private const float slowdownTime = 50f, slowdownUseTime = 10f;
    private static bool canUseSlowDown;

    //Refrence to cover for the slowdown img
    [SerializeField] private RectTransform slowDownCover;

    //Saves a refrence to the movement type the player using
    private MovementType movementType;

    //Refrence to the last finish the player hit
    public static GameObject lastFinishHit = null;

    //Refrence to the camera controller
    [SerializeField] private CameraController cameraController;

    //Backup for all objects
    private static int backup;

    //Audio source and clips
    private AudioSource audioSource;
    [SerializeField] private AudioClip scoreAddedSound, deathSound, shieldSound, slowdownTheme;
    private static int adCounter;

    //Method called on scene load
    private void Start()
    {
        //Fades in the scene
        StartCoroutine(AnimationPlus.FadeToColor(fadePanel, new Color(0, 0, 0, 0), UI.fadeTime, false));

        //Assigns the default vaules for the variables
        isDead = false;
        isAtFinish = false;
        score = 0;
        scoreAtLastFinish = 0;
        turnDelay = 0.25f;
        speed = 50;
        rotSpeed = 10;
        shieldCount = 0;

        if (!Gamemode.inLevel("Tutorial"))
        {
            gearCount = Serializer.activeData.gearCount;
            level = new Level(Serializer.activeData.level.xp);
        }

        GetComponent<SpriteRenderer>().sprite = activeSkin.frontSprite;
        segmentPrefab.GetComponent<SpriteRenderer>().sprite = activeSkin.segmentSprite;

        positions = new List<Vector3>();
        rotations = new List<Vector3>();
        rotPosTimes = new List<float>();
        next = -1;

        slowdownPercentage = 0;
        canUseSlowDown = true;
        slowDownCover.gameObject.SetActive(false);

        movementType = Serializer.activeData.settings.movementType;

        lastFinishHit = null;
        lastUpgraded = null;
        backup = 0;

        audioSource = GetComponent<AudioSource>();
    }

    //Method called on each frame
    private void Update()
    {
        //Checks if the player went out of bounds
        if (Mathf.Abs(transform.position.y) >= Refrence.wallTop.transform.position.y && !Player.isDead)
        {
            shieldCount = 0;
            KillPlayer();
        }

        //Gets the direction the player is trying to move
        GetInput();

        //Moves the player if they are trying to move
        if (moveArr[0])
        {
            Move(1);
        }
        if (moveArr[1])
        {
            Move(-1);
        }

        //Moves the player if they are not dead or at finish
        if (!isAtFinish && !isDead)
        {
            MovePlayer();
        }

        //Triggers if the player is not dead or at finish
        if (!isAtFinish && !isDead)
        {
            MoveSegments();
        }
    }

    //Method used to move the player
    private void MovePlayer()
    {
        transform.position += new Vector3(Object.speed * Time.deltaTime * Generator.GetRelativeSpeed(), 0);
        //Triggers if the player is to move
        if (transform.rotation.eulerAngles.z != 0)
        {
            //Triggers if the player is pointing down
            if (transform.rotation.eulerAngles.z >= 270)
            {
                //Moves downward
                transform.position += new Vector3(0, -1 * Time.deltaTime * speed * ((transform.rotation.eulerAngles.z - 360) * -1 / 90));
            }
            //Triggers if the polayer is pointing up
            else
            {
                //Moves upward
                transform.position += new Vector3(0, Time.deltaTime * speed * (transform.rotation.eulerAngles.z / 90));
            }
        }
    }

    //Method used to move the segments
    private void MoveSegments()
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

    //Method used to get the direction the player should be moving
    private void GetInput()
    {
        //Reset moveArr before input checking
        moveArr[0] = false;
        moveArr[1] = false;

        //Checks for mouse based movement if that is what the user has selected
        if (movementType == MovementType.Mouse)
        {
            //Mouse input checking
            if (Input.GetMouseButton(0) || Input.touchCount > 0)
            {
                //Gets the mouse position
                Vector2 inputPosition = Refrence.cam.ScreenToWorldPoint(Input.mousePosition);

                //If the mouse position is above the player move up
                if (inputPosition.y > transform.position.y)
                {
                    moveArr[0] = true;
                }
                //Else move down
                else
                {
                    moveArr[1] = true;
                }
            }
        }

        //Checks for keyboard based movement if that is what the user has selected
        if (movementType == MovementType.Keyboard)
        {
            //Moves up if the player is hitting the up or w button
            if (Input.GetKey(KeyCode.W))
            {
                moveArr[0] = true;
            }

            //Move down if the player if the down or s button
            if (Input.GetKey(KeyCode.S))
            {
                moveArr[1] = true;
            }
        }

        //Checks for keyboard based movement if that is what the user has selected
        if (movementType == MovementType.Buttons)
        {
            //Gets the bounds for the buttons from the gameUI
            Vector2[] bounds = Refrence.gameUI.GetButtonBounds();

            //Checks if the mouse position is in bounds and the user is clicking
            if (Functions.PositionIsInBounds2D(Input.mousePosition, bounds[0], bounds[3]) && Functions.UserIsClicking())
            {
                //Gets which button the mouse is in and sets it to true
                if (Input.mousePosition.y >= bounds[1].y)
                {
                    moveArr[0] = true;
                }
                else
                {
                    moveArr[1] = true;
                }
            }
        }

        if (Functions.UserIsClicking() || Input.touchCount > 0)
        {
            Vector2[] bounds = Refrence.gameUI.GetButtonBounds();
            if (Functions.PositionIsInBounds2D(Input.mousePosition, bounds[4], bounds[5]))
            {
                UseSlowDown();
            }
        }
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
        if (shieldCount >= 1)
        {
            UseShield();
            return;
        }

        //Stops any other sounds
        if (audioSource.isPlaying)
            audioSource.Stop();

        //Plays death sound
        audioSource.PlayOneShot(deathSound);

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

        Refrence.gameUI.transform.parent.GetComponent<GraphicRaycaster>().enabled = true;

        CleanMovementBuffers();

        //Hides the game UI and shows an ad then the death UI
        Refrence.gameUI.Hide();
        Refrence.deathUI.Show();

        if (!Gamemode.inLevel("Tutorial"))
        {
            if (adCounter >= 2)
            {
                Refrence.adManager.ShowIntersertialAdThenCall(() =>
                {
                    Debug.Log("Ad was completed");
                });
                adCounter = 0;
            }
            else
            {
                adCounter++;
            }
        }
    }

    //Method called when the player is to respawn
    public void Respawn()
    {
        //Removes the level from the last upgraded card
        if (lastUpgraded != null)
        {
            lastUpgraded.RemoveLevel();
            lastUpgraded = null;
        }

        //Moves the entire player to the middle of the screen
        MoveWhole(new Vector3(positionAtLastFinish.x, 0), new Vector3(0, 0, 0));

        //Resets the segment count to the segment count at last finish
        if (CountSegments() - segmentCountAtLastFinish > 0)
        {
            RemoveSegments(CountSegments() - segmentCountAtLastFinish, false);
        }
        else if (CountSegments() - segmentCountAtLastFinish < 0)
        {
            AddSegments((CountSegments() - segmentCountAtLastFinish) * -1, false);
        }

        //Sets the properties to there values at last finish
        score = scoreAtLastFinish;
        Debug.Log(xBoundAtLastFinish + " " + yBoundAtLastFinish);
        Refrence.camController.SetSize(yBoundAtLastFinish);
        Refrence.cam.transform.position = new Vector3(positionAtLastFinish.x, 0, -1);
        Refrence.wallTop.SetPosition(new Vector3(positionAtLastFinish.x, yBoundAtLastFinish - 0.5f));
        Refrence.wallBottom.SetPosition(new Vector3(positionAtLastFinish.x, -yBoundAtLastFinish + 0.5f));
        Refrence.gen.transform.position = new Vector3(positionAtLastFinish.x + xBoundAtLastFinish, 0);
        Refrence.gen.SetDestroyerPosition(positionAtLastFinish.x - xBoundAtLastFinish);
        Generator.SetBounds(yBoundAtLastFinish - 2);
        backup = backupAtLastFinish;

        //Shows the game UI, sets the score text, and sets is dead to false
        Refrence.gameUI.Show();
        Refrence.gameUI.UpdateScore(score);
        isDead = false;
        //Allows the player to use the slowdown (if they have it) and resets the movement type and last finish hit
        canUseSlowDown = true;
        movementType = Serializer.activeData.settings.movementType;
        lastFinishHit = null;
        positions = new List<Vector3>();
        rotations = new List<Vector3>();
        rotPosTimes = new List<float>();
        next = -1;

        Refrence.gameUI.UpdateShieldCount(shieldCount);
        Refrence.gameUI.transform.parent.GetComponent<GraphicRaycaster>().enabled = false;
    }

    //Method called when the player enters a finish
    public void OnEnterFinish()
    {
        //Sets the trigger for at finish to true
        isAtFinish = true;

        //Saves the values to the at last finish refrence
        positionAtLastFinish = Functions.CopyVector3(transform.position);
        segmentCountAtLastFinish = CountSegments();
        xBoundAtLastFinish = Refrence.gen.transform.position.x - positionAtLastFinish.x;
        yBoundAtLastFinish = Refrence.cam.orthographicSize;
        backupAtLastFinish = backup;
        scoreAtLastFinish = score;

        CleanMovementBuffers();
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
            level.AddXP(add * Gamemode.ModeMultiplier(Gamemode.mode));
        }

        //Adds Segments
        AddSegments(add);
    }

    //Method called to add segments to the player
    public void AddSegments(int add, bool useAnimation = true)
    {
        Debug.Log("Adding segments: " + add);
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
            Refrence.camController.OnSegmentChange(add, useAnimation);
            Refrence.gen.OnSegmentChange(add);
            Refrence.wallTop.OnSegmentChange(add, useAnimation);
            Refrence.wallBottom.OnSegmentChange(add, useAnimation);
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
        Debug.Log("Removing segments: " + remove);
        //Variable to store the current segment, the amount of segments removed, and a refrence to all segments
        Segment cur = segmentAfter;
        int amountRemoved = 0;
        List<Segment> segmentList = new List<Segment>();

        //Gets all segmetns
        while (!ReferenceEquals(cur, null))
        {
            segmentList.Add(cur);
            cur = cur.segmentAfter;
        }

        //Loops to remove the amount of segments, uses animation based on the input
        for (int i = Mathf.Max(0, segmentList.Count - remove); i < segmentList.Count; i++, amountRemoved++)
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
    private void MoveWhole(Vector3 position, Vector3 rotation, Segment segment = null)
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
    private void StopAllMovement(Segment segment = null)
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
    private int CountSegments()
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
        //Removes a shield, updates the UI, and plays the sound
        shieldCount--;
        Refrence.gameUI.UpdateShieldCount(shieldCount);

        if (Serializer.activeData.settings.soundEnabled)
        {
            audioSource.PlayOneShot(shieldSound);
        }
    }

    //Method called to increase the slowdown percentage of the slowdown powerup
    public void IncreaseSlowDownPercentage(float percentage)
    {
        if (slowdownPercentage == 0)
        {
            //Increases the slow down percentage if it was at 0
            slowdownPercentage += percentage;
        }
        else
        {
            //Multiple the slow down percentage if it was above 0
            slowdownPercentage = 1 - ((1 - slowdownPercentage) * (1 - percentage));
        }

        //Enable the slow down if it wasn't already
        if (slowdownPercentage - percentage == 0)
        {
            Refrence.gameUI.EnableSlowDown(true);
        }
    }

    //Method called to use the slowdown powerup
    public void UseSlowDown()
    {
        if (canUseSlowDown && slowdownPercentage != 0)
        {
            StartCoroutine(CountSlowDown());
        }
    }

    //Coroutine used to wait call the powerup for a certain time and wait for reuse
    private IEnumerator CountSlowDown()
    {
        canUseSlowDown = false;
        float time = 0;

        Generator.SetRelativeSpeed(slowdownPercentage > 1 ? 0.01f : 1 - slowdownPercentage);

        slowDownCover.gameObject.SetActive(true);
        slowDownCover.offsetMax = new Vector2(0, 0);

        if (Serializer.activeData.settings.soundEnabled)
        {
            audioSource.clip = slowdownTheme;
            audioSource.loop = true;
            audioSource.Play();
        }

        while (time < slowdownUseTime)
        {
            time += Time.deltaTime;
            Debug.Log(Time.deltaTime / slowdownUseTime);
            slowDownCover.offsetMax -= new Vector2(0, 100 * Time.deltaTime / slowdownUseTime);
            yield return new WaitForEndOfFrame();

            yield return new WaitUntil(() => !isAtFinish);
        }

        audioSource.Stop();
        audioSource.clip = null;
        audioSource.loop = false;

        Generator.SetRelativeSpeed(1);
        time = 0;

        slowDownCover.offsetMax = new Vector2(0, 0);

        while (time < slowdownTime)
        {
            time += Time.deltaTime;
            slowDownCover.offsetMax -= new Vector2(0, 100 * Time.deltaTime / slowdownTime);
            yield return new WaitForEndOfFrame();

            yield return new WaitUntil(() => !isAtFinish);
        }

        slowDownCover.offsetMax = new Vector2(0, -100);

        canUseSlowDown = true;

        yield return null;
    }

    //Returns the default movement type for the platform
    public static Player.MovementType DefaultMovementType(Gamemode.Platform platform) =>
        (platform == Gamemode.Platform.Windows) ?
            MovementType.Keyboard : MovementType.Buttons;

    //Method called to collect unused times and positions in the segments and player
    private void CleanMovementBuffers()
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
