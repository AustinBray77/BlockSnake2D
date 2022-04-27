using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

//Class to contorl the player
//Requires an audio source and a sprite renderer
[RequireComponent(typeof(AudioSource), typeof(SpriteRenderer))]
public class Player : Segmentable
{
    #region Enums & Structs
    //Enum for movement types
    public enum MovementType
    {
        Buttons,
        Touch,
        Keyboard
    }

    //Struct to encapsulate data related to the power ups
    private struct PowerUpData
    {
        //Variable to store the amount of shields the player has left
        public int ShieldCount;

        //Variable to store the slow down percentage of the slow down object
        public float SlowdownPercentage;

        //Variable to store the delay of the slow down power up
        public const float SlowdownRechargeTime = 50f, SlowdownDuration = 10f;
        public bool CanUseSlowDown;

        //Variable to store the amount of Shocks the player has
        public int ShockCount;

        //Method used to add shields 
        public void AddShields(int amount)
        {
            //Increments shield count and updates the UI
            ShieldCount += amount;
            Game_UI.Instance.UpdateShieldCount(ShieldCount);
        }

        //Method used to remove shields
        public void RemoveShields(int amount)
        {
            //Decrements shield count and updates the UI
            ShieldCount -= amount;
            Game_UI.Instance.UpdateShieldCount(ShieldCount);
        }

        //Method used to add shocks
        public void AddShocks(int amount)
        {
            //Increments shock count and updates the UI
            ShockCount += amount;
            Game_UI.Instance.UpdateShockCount(ShockCount);
        }

        //Method used to remove shocks
        public void RemoveShocks(int amount)
        {
            //Decrements shock count and updates the UI
            ShockCount -= amount;
            Game_UI.Instance.UpdateShockCount(ShockCount);
        }
    }

    //Class used to create events which can only be called once before being reset
    private class SingleCallEvent
    {
        //Stores the state and event
        private bool called;
        private UnityEvent _event;

        //Base constructor
        public SingleCallEvent()
        {
            //Sets default values
            called = false;
            _event = new UnityEvent();
        }

        //Method used to invoke the event
        public void Invoke()
        {
            //If the event has not been called, invoke it
            if (!called)
            {
                _event.Invoke();
            }

            //Flag that the event has been called
            called = true;
        }

        //Method to reset the state
        public void Reset()
            => called = false;

        //Method to add a listener(action) to the event
        public void AddListener(UnityAction action)
            => _event.AddListener(action);
    }
    #endregion

    #region Fields
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

    //Reference for storing power up data
    private PowerUpData _powerUpData;

    //Refrence to cover for the slowdown img
    //[SerializeField] private RectTransform slowDownCover;

    //Saves a refrence to the movement type the player using
    private MovementType movementType;

    //Refrence to the last finish the player hit
    public static GameObject lastFinishHit = null;

    //Refrence to the camera controller
    [SerializeField] private CameraController cameraController;

    //Audio source and clips
    private AudioSource audioSource;
    [SerializeField] private AudioClip scoreAddedSound, deathSound, shieldSound, slowdownTheme;

    //Counter counting the amount of deaths since an add has been shown
    private static int adCounter;

    //Single call event for shock and slowdown powerups
    private SingleCallEvent _useShock, _useSlowdown;
    #endregion

    #region Property Getters
    //Method called to get which of the current control buttons are being pressed down
    public bool[] ButtonsPressed =>
        moveArr;

    //Property to get the players current shield count
    public int ShieldCount =>
        _powerUpData.ShieldCount;

    //Property to get the active skin
    public static Skin ActiveSkin =>
        Gamemanager.Instance.Skins[Serializer.Instance.activeData.activeSkin];

    //Returns the default movement type for the platform
    public static MovementType DefaultMovementType(Gamemanager.Platform platform) =>
        (platform == Gamemanager.Platform.Windows) ?
            MovementType.Keyboard : MovementType.Buttons;
    #endregion

    #region Unity Methods
    //Method called on scene load
    private new void Start()
    {
        base.Start();
        //Assigns the default vaules for the variables
        /*Player starts alive and not at finish*/
        isDead = false;
        isAtFinish = false;

        /*Player has no score on start*/
        score = 0;
        scoreAtLastFinish = 0;

        /*Base Movement Assignments*/
        turnDelay = 0.25f;
        ySpeed = 50;
        xSpeed = Object.speed;
        rotSpeed = 10;

        /*Level is equal to the players xp at start*/
        level = new Level(Serializer.Instance.activeData.level.xp);

        /*Skin Assignments to front and segments*/
        GetComponent<SpriteRenderer>().sprite = ActiveSkin.frontSprite;
        segmentPrefab.GetComponent<SpriteRenderer>().sprite = ActiveSkin.segmentSprite;

        /*No powerups on start*/
        _powerUpData.ShieldCount = 0;
        _powerUpData.SlowdownPercentage = 0;
        _powerUpData.CanUseSlowDown = true;

        /*Intializes Single Call Event for shock and slowdown powerups*/
        _useShock = new SingleCallEvent();
        _useSlowdown = new SingleCallEvent();
        _useShock.AddListener(UseShock);
        _useSlowdown.AddListener(UseSlowdown);

        /*Sets movement type*/
        movementType = Serializer.Instance.activeData.settings.movementType;

        /*Player has not hit a finish*/
        lastFinishHit = null;
        lastUpgraded = null;

        /*Gets Audio Source*/
        audioSource = GetComponent<AudioSource>();

        /*Player has not been shown a tip (for tutorial)*/
        Finish_UI.tipWasShown = false;

        //Fades in the scene
        StartCoroutine(AnimationPlus.FadeToColor(fadePanel, new Color(0, 0, 0, 0), UI.fadeTime, false));
    }

    //Method called on each frame
    private void Update()
    {
        //Assigns speed (might be unnessecary)
        xSpeed = Object.speed;

        //Checks if the player went out of bounds
        if (Mathf.Abs(transform.position.y) >= Reference.wallTop.transform.position.y && !Player.isDead)
        {
            //Kills player if they are above or below the walls
            KillPlayer(true);
        }

        //Gets the direction the player is trying to move
        GetInput();

        //Moves the player if they are trying to move
        if (moveArr[0])
        {
            //Rotate up
            Rotate(YDirection.Up);
        }
        if (moveArr[1])
        {
            //Rotate down
            Rotate(YDirection.Down);
        }

        //Moves the player if they are not dead or at finish
        if (!isAtFinish && !isDead)
        {
            //Moves the player
            Move();
        }

        //Triggers if the player is not dead or at finish
        if (!isAtFinish && !isDead)
        {
            //Moves the segments
            MoveSegments();
        }
    }
    #endregion

    #region Contol Methods
    //Method used to get input from the player
    private void GetInput()
    {
        //Gets the bounds for the buttons from the gameUI
        Vector2[] bounds = Game_UI.Instance.ButtonBounds;

        //If the bounds have not been loaded, exit
        if (bounds == null)
        {
            return;
        }

        Log($"Bounds Counts:{bounds.Length}");


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
            //If bounds is not long enough, return
            if (bounds.Length <= 3)
            {
                return;
            }

            //Checks if the mouse position is in bounds and the user is clicking
            if (Functions.PositionIsInBounds2D(Input.mousePosition, bounds[0], bounds[3]) && Functions.UserIsClicking())
            {
                //Moves up if the player is hitting the upper button (bounds[1] is the lower left corner of the top button)
                if (Input.mousePosition.y >= bounds[1].y)
                {
                    moveArr[0] = true;
                }
                //Else moves down
                else
                {
                    moveArr[1] = true;
                }
            }
        }

        //Triggers if the user is clicking
        if (Functions.UserIsClicking())
        {
            //Triggers if the player is hitting the slowdown button (bounds[4] and bounds[5])
            if (Functions.PositionIsInBounds2D(Input.mousePosition, bounds[4], bounds[5]))
            {
                //Try and use the slowdown powerup
                _useSlowdown.Invoke();
            }
            //Triggers if the player is hitting the shock button (bounds[6] and bounds[7])
            else if (Functions.PositionIsInBounds2D(Input.mousePosition, bounds[6], bounds[7]))
            {
                //Try and use the shock powerup
                _useShock.Invoke();
            }
        }

        //Trigger is the user is not clicking
        if (!Functions.UserIsClicking())
        {
            //Reset both invokes (prevents problems from the user holding the buttons down)
            _useShock.Reset();
            _useSlowdown.Reset();
        }
    }

    //Method called when the player is to be killed, returns if the player was actually killed
    public bool KillPlayer(bool ignoreShields)
    {
        Log("Killing Player?");

        //Uses shield if the player has one
        if (_powerUpData.ShieldCount >= 1 && !ignoreShields)
        {
            Log("Using Shield...");
            //Use the shield and then return that the player has not died
            UseShield();
            return false;
        }

        //Stops any other sounds
        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }

        //Plays death sound
        audioSource.PlayOneShot(deathSound);

        //Stops all couroutines in the following segments
        StopAllMovement();

        //Triggers if not in the tutorial
        if (!Gamemanager.InLevel("Tutorial"))
        {
            //Triggers if the player achieved a new highscore
            if (Serializer.Instance.activeData.highScore < score)
            {
                //Sets the highscore in the active data for saving
                Serializer.Instance.activeData.SetHighScore(score);
            }
        }

        //Triggers that the player is dead
        isDead = true;

        //Tells the generator that the player is dead
        Reference.gen.OnPlayerDeath();

        //Reenables the UI input (disabled for optimization)
        Game_UI.Instance.transform.parent.GetComponent<GraphicRaycaster>().enabled = true;

        //Removes unnecessary and harmful data from movement buffers
        CleanMovementBuffers();

        //Hides the game UI and shows the death UI
        Game_UI.HideInstance();
        Death_UI.ShowInstance();

        //Triggers if not in the tutorial
        if (!Gamemanager.InLevel("Tutorial"))
        {
            //Triggers if on android
            if (Gamemanager.Instance.CurrentPlatform == Gamemanager.Platform.Android)
            {
                //Saves score to google play games leaderboard
                PlayGamesService.Instance.AddLeaderboardScore(score, PlayGamesService.HighScoreID);
            }

            //Triggers if the player has not received an ad in the last three deaths
            if (adCounter >= 2)
            {
                //Shows an ad
                UnityAdsService.Instance.ShowIntersertialAdThenCall(() =>
                {
                    Log("Ad was completed");
                });
                //Resets the counter since last ad played
                adCounter = 0;
            }
            else
            {
                //Else increment the counter
                adCounter++;
            }
        }

        //Return that the player has died
        return true;
    }

    //Method called when the player is to respawn
    public void Respawn()
    {
        //Triggers if the player used an upgrade (card)
        if (lastUpgraded != null)
        {
            //Removes the last upgrade from the last used upgrade
            lastUpgraded.RemoveLevel();
            Log(lastUpgraded.title);
            //Removes reference to last used upgrade to prevent double deleveling
            lastUpgraded = null;
        }

        //Moves the entire player to the middle of the screen
        MoveWhole(new Vector3(positionAtLastFinish.x, 0), new Vector3(0, 0, 0));

        //Triggers if the player has more segments than they did at last finish
        if (CountSegments() - segmentCountAtLastFinish > 0)
        {
            //Removes the difference in segments without animation
            RemoveSegments(CountSegments() - segmentCountAtLastFinish, false);
        }
        //Triggers if the player has less segments than they did at last finish
        else if (CountSegments() - segmentCountAtLastFinish < 0)
        {
            //Adds the difference in segments with score dependance (to reset backup and make sure sizing is correct) and without animation
            AddSegments((CountSegments() - segmentCountAtLastFinish) * -1, true, false);
        }

        //Sets the properties to their values at last finish
        score = scoreAtLastFinish;
        Reference.camController.SetSize(yBoundAtLastFinish);
        Reference.cam.transform.position = new Vector3(positionAtLastFinish.x, 0, -1);
        Reference.wallTop.SetPosition(new Vector3(positionAtLastFinish.x, yBoundAtLastFinish - 0.5f));
        Reference.wallBottom.SetPosition(new Vector3(positionAtLastFinish.x, -yBoundAtLastFinish + 0.5f));
        Reference.gen.transform.position = new Vector3(positionAtLastFinish.x + xBoundAtLastFinish, 0);
        Reference.gen.SetDestroyerPosition(positionAtLastFinish.x - xBoundAtLastFinish);
        Generator.SetBounds(yBoundAtLastFinish - 2);
        backup = backupAtLastFinish;

        //Assigns level from saved xp from last death
        level = new Level(Serializer.Instance.activeData.level.xp);

        //Shows the game UI
        Game_UI.ShowInstance();

        //Updates the score, gear, and shield count
        Game_UI.Instance.UpdateScore(score);
        Game_UI.Instance.UpdateGearCount(Serializer.Instance.activeData.gearCount);
        Game_UI.Instance.UpdateShieldCount(_powerUpData.ShieldCount);

        //Flags that the player is not dead
        isDead = false;

        //Allows the player to use the slowdown (if they have it)
        _powerUpData.CanUseSlowDown = true;

        //Sets the movement type to the one saved (possibly redundant)
        movementType = Serializer.Instance.activeData.settings.movementType;

        //Sets that the user has not hit a finish yet
        lastFinishHit = null;

        //Resets the data to do with moving
        SegmentableReset();

        //Disables UI input (optimization)
        Game_UI.Instance.transform.parent.GetComponent<GraphicRaycaster>().enabled = false;
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

        //Removes unnecessary and harmful data from movement buffers
        CleanMovementBuffers();
    }

    //Method called to add score to the player
    public void AddScore(int add)
    {
        //Adds the score
        score += add;

        //Sets the score text to the current score
        Game_UI.Instance.UpdateScore(score);

        //Triggers if not in the tutorial
        if (!Gamemanager.InLevel("Tutorial"))
        {
            //Adds XP
            level.AddXP(add * Gamemanager.Instance.ModeMultiplier());
        }

        //Adds associated number of segments
        AddSegments(add, true);
    }
    #endregion

    #region Use / Control Powerup Methods
    //Method called when the player uses a shield instead of dying
    public void UseShield()
    {
        //Removes a shield, updates the UI, and plays the sound
        _powerUpData.RemoveShields(1);

        //Triggers if the player has sound enabled
        if (Serializer.Instance.activeData.settings.soundEnabled)
        {
            //Plays the shield sound
            audioSource.PlayOneShot(shieldSound);
        }
    }

    //Method called to use the slowdown powerup
    public void UseSlowdown()
    {
        //Triggers if the player can use the slowdown power up and they have it unlocked
        if (_powerUpData.CanUseSlowDown && _powerUpData.SlowdownPercentage != 0)
        {
            //Uses the slowdown powerup
            StartCoroutine(CountSlowdown());
        }
    }

    //Coroutine used to wait call the powerup for a certain time and wait for reuse
    private IEnumerator CountSlowdown()
    {
        //Set that the player cannot use the slowdown again (until the current one is done)
        _powerUpData.CanUseSlowDown = false;

        //Scopes isolated to prevent variable overlap
        {
            //Counter for time
            float time = 0;

            //Slows the game speed to the correct speed
            Generator.SetRelativeSpeed(_powerUpData.SlowdownPercentage > 1 ? 0.01f : 1 - _powerUpData.SlowdownPercentage);

            //Shows the slow down cover image and centers it
            Game_UI.Instance.SlowDownCover.gameObject.SetActive(true);
            Game_UI.Instance.SlowDownCover.offsetMax = new Vector2(0, 0);

            //Triggers if the player has sound enabled
            if (Serializer.Instance.activeData.settings.soundEnabled)
            {
                //Loops the slowdown theme 
                audioSource.clip = slowdownTheme;
                audioSource.loop = true;
                audioSource.Play();
            }

            //Loops while the time counter is less than the time the slowdown lasts
            while (time < PowerUpData.SlowdownDuration)
            {
                //Increment time counter by the time between the last frames 
                time += Time.deltaTime;
                Log(Time.deltaTime / PowerUpData.SlowdownDuration);

                //Shrinks the slowdown cover by the appropriate amount based on the time between last frames
                Game_UI.Instance.SlowDownCover.offsetMax -= new Vector2(0, 100 * Time.deltaTime / PowerUpData.SlowdownDuration);

                //Wait until the end of the frame
                yield return new WaitForEndOfFrame();

                //Wait until the player is not at a finish
                yield return new WaitUntil(() => !isAtFinish);
            }

            //Stops the slowdown theme
            audioSource.Stop();
            audioSource.clip = null;
            audioSource.loop = false;

            //Resets the 
            Generator.SetRelativeSpeed(1);
        }
        {
            //Counter for time
            float time = 0;

            //Centers the slowdown cover over the whole image
            Game_UI.Instance.SlowDownCover.offsetMax = new Vector2(0, 0);

            //Loops while the time counter is less than the time to recharge the slowdown powerup
            while (time < PowerUpData.SlowdownRechargeTime)
            {
                //Increment time counter by the time between the last frames
                time += Time.deltaTime;

                //Shrinks the slowdown cover by the appropriate amount based on the time between last frames
                Game_UI.Instance.SlowDownCover.offsetMax -= new Vector2(0, 100 * Time.deltaTime / PowerUpData.SlowdownRechargeTime);

                //Wait until the end of the frame
                yield return new WaitForEndOfFrame();

                //Wait until the player is not at a finish
                yield return new WaitUntil(() => !isAtFinish);
            }
        }

        //Hides the slowdown cover (sets height to 0)
        Game_UI.Instance.SlowDownCover.offsetMax = new Vector2(0, -100);

        //Allows the player to use the power up again
        _powerUpData.CanUseSlowDown = true;
    }

    //Method called to use the shock powerup
    public void UseShock()
    {
        //Removes a shock from the player
        _powerUpData.RemoveShocks(1);

        //Destroys all objects capable of killing/damaging the player
        Reference.gen.DestoryDamageObjects();

        //Triggers if the player has sound enabled
        if (Serializer.Instance.activeData.settings.soundEnabled)
        {
            //Plays the shock sound (not currently implemented)
            //audioSource.PlayOneShot(shockSound);
        }
    }

    #endregion

    #region Power Up Increase / Decrease
    /* Power Up Increase / Decrease Methods */

    //Method called to remove segments from card
    public void RemoveSegments(float value, int cardIndex) =>
        RemoveSegments((int)Mathf.Abs(value));

    //Method called to add segments from card
    public void AddSegments(float value, int cardIndex) =>
        AddSegments((int)value, true);

    //Method called to increase y speed from card
    public void IncreaseYSpeed(float value, int cardIndex) =>
        IncreaseYSpeed(value);

    //Method called to decrease y speed from card
    public void DecreaseYSpeed(float value, int cardIndex) =>
        DecreaseYSpeed(value);

    //Method called to add sheilds y speed from card
    public void AddShields(float value, int cardIndex) =>
        _powerUpData.AddShields((int)value);

    //Method called to remove sheilds from card
    public void RemoveShields(float value, int cardIndex) =>
        _powerUpData.RemoveShields((int)value);

    //Method called to increase the rotation speed from card
    public void IncreaseRotSpeed(float value, int cardIndex) =>
        IncreaseRotSpeed(value);

    //Method called to decrease the rotation speed from card
    public void DecreaseRotSpeed(float value, int cardIndex) =>
        DecreaseRotSpeed(value);

    //Method called to increase the slowdown percentage of the slowdown powerup
    public void IncreaseSlowDownPercentage(float percentage, int cardIndex)
    {
        //Triggers if the slowdown is just being unlocked (ie level 0 -> level 1)
        if (_powerUpData.SlowdownPercentage == 0)
        {
            //Increases the slow down percentage by the inputted amount
            _powerUpData.SlowdownPercentage += percentage;

            //Enables the slowdown image in the UI
            Game_UI.Instance.EnableSlowDown(true);
        }
        else
        {
            //Else multiply the slow down percentage by the inputted amount
            _powerUpData.SlowdownPercentage = 1 - ((1 - _powerUpData.SlowdownPercentage) * (1 - percentage));
        }
    }

    //Method called to decrease the slowed percentage of the slowdown powerup
    public void DecreaseSlowDownPercentage(float percentage, int cardIndex)
    {
        //Triggers if the slowdown is being locked again (ie level 1 -> level 0)
        if (Reference.cardTypes[cardIndex].level == 0)
        {
            Log("Level is 0, disabling UI...");

            //Fully disables the slowdown powerup
            _powerUpData.SlowdownPercentage = 0;

            //Disables in UI without animation and ignoring the current state of the image (force false)
            Game_UI.Instance.EnableSlowDown(false, false, true);
        }
        else
        {
            Log("Level is not 0, dividing...");

            //Else divide the slow down percentage by the inputted amount
            _powerUpData.SlowdownPercentage = 1 - ((1 - _powerUpData.SlowdownPercentage) * (1 - (1 / percentage)));
        }
    }

    //Method called by card to add shocks to the player
    public void AddShocks(float value, int cardIndex) =>
        _powerUpData.AddShocks((int)value);

    //Method called by card to remove shocks from the player
    public void RemoveShocks(float value, int cardIndex) =>
        _powerUpData.RemoveShocks((int)value);
    #endregion
}
