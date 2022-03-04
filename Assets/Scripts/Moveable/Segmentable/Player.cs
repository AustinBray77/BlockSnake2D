using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

//Class to contorl the player
[RequireComponent(typeof(AudioSource), typeof(SpriteRenderer))]
public class Player : Segmentable
{
    //Enum for movement types
    public enum MovementType
    {
        Buttons,
        Touch,
        Keyboard
    }

    //Refrence to the fade panel
    [SerializeField] private Image fadePanel;

    //Saves the last card the player upgraded
    [HideInInspector] public Card lastUpgraded;

    //Saves the move buttons the user is touching
    private bool[] moveArr = new bool[] { false, false };

    //Stores how much objects should move each time a segment is added
    public static float increaseFactor = 1.5f;

    //Stores whether the player is dead or at a finish
    public static bool isDead, isAtFinish;

    //Stores the score, level, gear count
    public static int score;
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

    //Audio source and clips
    private AudioSource audioSource;
    [SerializeField] private AudioClip scoreAddedSound, deathSound, shieldSound, slowdownTheme;
    private static int adCounter;

    //Method called on scene load
    private void Start()
    {
        //Assigns the default vaules for the variables
        isDead = false;
        isAtFinish = false;
        score = 0;
        scoreAtLastFinish = 0;
        turnDelay = 0.25f;
        ySpeed = 50;
        xSpeed = Object.speed;
        rotSpeed = 10;
        shieldCount = 0;
        level = new Level(Serializer.activeData.level.xp);

        GetComponent<SpriteRenderer>().sprite = activeSkin.frontSprite;
        segmentPrefab.GetComponent<SpriteRenderer>().sprite = activeSkin.segmentSprite;

        Segmentable_Init();

        slowdownPercentage = 0;
        canUseSlowDown = true;
        slowDownCover.gameObject.SetActive(false);

        movementType = Serializer.activeData.settings.movementType;

        lastFinishHit = null;
        lastUpgraded = null;

        audioSource = GetComponent<AudioSource>();

        Finish_UI.tipWasShown = false;

        //Fades in the scene
        StartCoroutine(AnimationPlus.FadeToColor(fadePanel, new Color(0, 0, 0, 0), UI.fadeTime, false));
    }

    //Method called on each frame
    private void Update()
    {
        xSpeed = Object.speed;

        //Checks if the player went out of bounds
        if (Mathf.Abs(transform.position.y) >= Reference.wallTop.transform.position.y && !Player.isDead)
        {
            shieldCount = 0;
            KillPlayer();
        }

        //Gets the direction the player is trying to move
        GetInput();

        //Moves the player if they are trying to move
        if (moveArr[0])
        {
            Rotate(YDirection.Up);
        }
        if (moveArr[1])
        {
            Rotate(YDirection.Down);
        }

        //Moves the player if they are not dead or at finish
        if (!isAtFinish && !isDead)
        {
            Move();
        }

        //Triggers if the player is not dead or at finish
        if (!isAtFinish && !isDead)
        {
            MoveSegments();
        }
    }

    //Method used to get the direction the player should be moving
    private void GetInput()
    {
        //Reset moveArr before input checking
        moveArr[0] = false;
        moveArr[1] = false;

        //Checks for mouse based movement if that is what the user has selected
        if (movementType == MovementType.Touch)
        {
            //Mouse input checking
            if (Input.GetMouseButton(0) || Input.touchCount > 0)
            {
                //Gets the mouse position
                Vector2 inputPosition = Reference.cam.ScreenToWorldPoint(Input.mousePosition);

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
            Vector2[] bounds = Reference.gameUI.GetButtonBounds();

            if (bounds == null)
            {
                return;
            }

            if (bounds.Length <= 3)
            {
                return;
            }

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
            Vector2[] bounds = Reference.gameUI.GetButtonBounds();
            if (Functions.PositionIsInBounds2D(Input.mousePosition, bounds[4], bounds[5]))
            {
                UseSlowDown();
            }
        }
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
            //Serializer.activeData.SetGearCount(gearCount);
        }

        //Triggers that the player is dead
        isDead = true;

        //Tells the generator that the player is dead
        Reference.gen.OnPlayerDeath();

        Reference.gameUI.transform.parent.GetComponent<GraphicRaycaster>().enabled = true;

        CleanMovementBuffers();

        //Hides the game UI and shows an ad then the death UI
        Reference.gameUI.Hide();
        Reference.deathUI.Show();

        if (!Gamemode.inLevel("Tutorial"))
        {
            if (adCounter >= 2)
            {
                Reference.adManager.ShowIntersertialAdThenCall(() =>
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
            Debug.Log(lastUpgraded.title);
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
        Reference.camController.SetSize(yBoundAtLastFinish);
        Reference.cam.transform.position = new Vector3(positionAtLastFinish.x, 0, -1);
        Reference.wallTop.SetPosition(new Vector3(positionAtLastFinish.x, yBoundAtLastFinish - 0.5f));
        Reference.wallBottom.SetPosition(new Vector3(positionAtLastFinish.x, -yBoundAtLastFinish + 0.5f));
        Reference.gen.transform.position = new Vector3(positionAtLastFinish.x + xBoundAtLastFinish, 0);
        Reference.gen.SetDestroyerPosition(positionAtLastFinish.x - xBoundAtLastFinish);
        Generator.SetBounds(yBoundAtLastFinish - 2);
        backup = backupAtLastFinish;
        level = new Level(Serializer.activeData.level.xp);

        //Shows the game UI, sets the score text, and sets is dead to false
        Reference.gameUI.Show();
        Reference.gameUI.UpdateScore(score);
        Reference.gameUI.UpdateGearCount(Serializer.activeData.gearCount);
        isDead = false;
        //Allows the player to use the slowdown (if they have it) and resets the movement type and last finish hit
        canUseSlowDown = true;
        movementType = Serializer.activeData.settings.movementType;
        lastFinishHit = null;
        positions = new List<Vector3>();
        rotations = new List<Vector3>();
        rotPosTimes = new List<float>();
        next = -1;

        Reference.gameUI.UpdateShieldCount(shieldCount);
        Reference.gameUI.transform.parent.GetComponent<GraphicRaycaster>().enabled = false;
    }

    //Method called when the player enters a finish
    public void OnEnterFinish()
    {
        //Sets the trigger for at finish to true
        isAtFinish = true;

        //Saves the values to the at last finish refrence
        positionAtLastFinish = Functions.CopyVector3(transform.position);
        segmentCountAtLastFinish = CountSegments();
        xBoundAtLastFinish = Reference.gen.transform.position.x - positionAtLastFinish.x;
        yBoundAtLastFinish = Reference.cam.orthographicSize;
        backupAtLastFinish = backup;
        scoreAtLastFinish = score;

        CleanMovementBuffers();
    }

    //Method called to add score to the player
    public void AddScore(int add)
    {
        //Adds the score
        score += add;

        //Sets the score text to the current score
        Reference.gameUI.UpdateScore(score);

        //Triggers if not in the tutorial
        if (!Gamemode.inLevel("Tutorial"))
        {
            //Adds XP
            level.AddXP(add * Gamemode.ModeMultiplier(Gamemode.mode));
        }

        //Adds Segments
        AddSegments(add);
    }

    //Method called to get which of the current control buttons are being pressed down
    public bool[] GetControls() =>
        moveArr;

    //Method called to add shields to the player
    public void AddShields(int count)
    {
        //Adds shields and updates the UI
        shieldCount += count;
        Reference.gameUI.UpdateShieldCount(shieldCount);
    }

    //Method called to remove shields from the player
    public void RemoveShields(int count)
    {
        shieldCount -= count;
        Reference.gameUI.UpdateShieldCount(shieldCount);
    }

    //Method called when the player uses a shield instead of dying
    public void UseShield()
    {
        //Removes a shield, updates the UI, and plays the sound
        shieldCount--;
        Reference.gameUI.UpdateShieldCount(shieldCount);

        if (Serializer.activeData.settings.soundEnabled)
        {
            audioSource.PlayOneShot(shieldSound);
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

    #region Power Up Increase / Decrease
    /* Power Up Increase / Decrease Methods */

    //Method called to remove segments from card
    public void RemoveSegments(float value, int cardIndex) =>
        RemoveSegments((int)Mathf.Abs(value));

    //Method called to add segments from card
    public void AddSegments(float value, int cardIndex) =>
        AddSegments((int)value);

    //Method called to increase y speed from card
    public void IncreaseYSpeed(float value, int cardIndex) =>
        IncreaseYSpeed(value);

    //Method called to decrease y speed from card
    public void DecreaseYSpeed(float value, int cardIndex) =>
        DecreaseYSpeed(value);

    //Method called to add sheilds y speed from card
    public void AddShields(float value, int cardIndex) =>
        AddShields((int)value);

    //Method called to remove sheilds from card
    public void RemoveShields(float value, int cardIndex) =>
        RemoveShields((int)value);

    //Method called to increase the rotation speed from card
    public void IncreaseRotSpeed(float value, int cardIndex) =>
        IncreaseRotSpeed(value);

    //Method called to decrease the rotation speed from card
    public void DecreaseRotSpeed(float value, int cardIndex) =>
        DecreaseRotSpeed(value);

    //Method called to increase the slowdown percentage of the slowdown powerup
    public void IncreaseSlowDownPercentage(float percentage, int cardIndex)
    {
        if (slowdownPercentage == 0)
        {
            //Increases the slow down percentage if it was at 0
            slowdownPercentage += percentage;
            Reference.gameUI.EnableSlowDown(true);
        }
        else
        {
            //Multiply the slow down percentage if it was above 0
            slowdownPercentage = 1 - ((1 - slowdownPercentage) * (1 - percentage));
        }
    }

    //Method called to decrease the slowed percentage of the slowdown powerup
    public void DecreaseSlowDownPercentage(float percentage, int cardIndex)
    {
        if (Reference.cardTypes[cardIndex].level == 0)
        {
            Debug.Log("Level is 0, disabling UI...");
            //Fully disable the slowdown powerup
            slowdownPercentage = 0;
            Reference.gameUI.EnableSlowDown(false, false, true);
        }
        else
        {
            Debug.Log("Level is not 0, dividing...");
            //Divide the slow down percentage if it was above 0
            slowdownPercentage = 1 - ((1 - slowdownPercentage) * (1 - (1 / percentage)));
        }
    }
    #endregion
}
