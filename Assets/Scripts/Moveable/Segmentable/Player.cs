using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

//Class to contorl the player
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

    private struct PowerUpData
    {
        //Variable to store the amount of shields the player has left
        public int ShieldCount;

        //Variable to store the slow down percentage of the slow down object
        public float SlowdownPercentage;

        //Variable to store the delay of the slow down power up
        public const float SlowdownTime = 50f, SlowdownUseTime = 10f;
        public bool CanUseSlowDown;

        //Variable to store the amount of Shocks the player has
        public int ShockCount;

        public void AddShields(int amount)
        {
            ShieldCount += amount;
            Game_UI.Instance.UpdateShieldCount(ShieldCount);
        }

        public void RemoveShields(int amount)
        {
            ShieldCount -= amount;
            Game_UI.Instance.UpdateShieldCount(ShieldCount);
        }

        public void AddShocks(int amount)
        {
            ShockCount += amount;
            Game_UI.Instance.UpdateShockCount(ShockCount);
        }

        public void RemoveShocks(int amount)
        {
            ShockCount -= amount;
            Game_UI.Instance.UpdateShockCount(ShockCount);
        }
    }

    private class SingleCallEvent
    {
        private bool called;
        private UnityEvent _event;

        public SingleCallEvent()
        {
            called = false;
            _event = new UnityEvent();
        }

        public void Invoke()
        {
            if (!called)
            {
                _event.Invoke();
            }

            called = true;
        }

        public void Reset()
            => called = false;

        public void AddListener(UnityAction action)
            => _event.AddListener(action);
    }
    #endregion

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
        Gamemanager.Instance.Skins[Serializer.Instance.activeData.activeSkin];

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
    private static int adCounter;

    private SingleCallEvent _useShock, _useSlowdown;

    #region Unity Methods
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
        _powerUpData.ShieldCount = 0;
        level = new Level(Serializer.Instance.activeData.level.xp);

        GetComponent<SpriteRenderer>().sprite = activeSkin.frontSprite;
        segmentPrefab.GetComponent<SpriteRenderer>().sprite = activeSkin.segmentSprite;

        Segmentable_Init();

        _powerUpData.SlowdownPercentage = 0;
        _powerUpData.CanUseSlowDown = true;

        _useShock = new SingleCallEvent();
        _useSlowdown = new SingleCallEvent();

        _useShock.AddListener(UseShock);
        _useSlowdown.AddListener(UseSlowdown);

        movementType = Serializer.Instance.activeData.settings.movementType;

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
            KillPlayer(true);
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
    #endregion

    //Method used to get the direction the player should be moving
    private void GetInput()
    {
        //Gets the bounds for the buttons from the gameUI
        Vector2[] bounds = Game_UI.Instance.ButtonBounds;

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

        if (Functions.UserIsClicking())
        {
            if (Functions.PositionIsInBounds2D(Input.mousePosition, bounds[4], bounds[5]))
            {
                _useSlowdown.Invoke();
            }
            else if (Functions.PositionIsInBounds2D(Input.mousePosition, bounds[6], bounds[7]))
            {
                _useShock.Invoke();
            }
        }

        if (!Functions.UserIsClicking())
        {
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
            UseShield();
            return false;
        }

        //Stops any other sounds
        if (audioSource.isPlaying)
            audioSource.Stop();

        //Plays death sound
        audioSource.PlayOneShot(deathSound);

        //Stops all couroutines in the following segments
        StopAllMovement();

        //Triggers if not in the tutorial
        if (!Gamemanager.InLevel("Tutorial"))
        {
            //If the players achieved a new highscore, the highscore is set to the score
            if (Serializer.Instance.activeData.highScore < score)
            {
                Serializer.Instance.activeData.SetHighScore(score);
            }

            //Sets the gear count in the serializer
            //Serializer.activeData.SetGearCount(gearCount);
        }

        //Triggers that the player is dead
        isDead = true;

        //Tells the generator that the player is dead
        Reference.gen.OnPlayerDeath();

        Game_UI.Instance.transform.parent.GetComponent<GraphicRaycaster>().enabled = true;

        CleanMovementBuffers();

        //Hides the game UI and shows an ad then the death UI
        Game_UI.HideInstance();
        Death_UI.ShowInstance();

        if (!Gamemanager.InLevel("Tutorial"))
        {

            if (Gamemanager.Instance.CurrentPlatform == Gamemanager.Platform.Android)
            {
                PlayGamesService.Instance.AddLeaderboardScore(score, PlayGamesService.HighScoreID);
            }

            if (adCounter >= 2)
            {
                UnityAdsService.Instance.ShowIntersertialAdThenCall(() =>
                {
                    Log("Ad was completed");
                });
                adCounter = 0;
            }
            else
            {
                adCounter++;
            }
        }

        return true;
    }

    //Method called when the player is to respawn
    public void Respawn()
    {
        //Removes the level from the last upgraded card
        if (lastUpgraded != null)
        {
            lastUpgraded.RemoveLevel();
            Log(lastUpgraded.title);
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
            AddSegments((CountSegments() - segmentCountAtLastFinish) * -1, true, false);
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
        level = new Level(Serializer.Instance.activeData.level.xp);

        //Shows the game UI, sets the score text, and sets is dead to false
        Game_UI.ShowInstance();
        Game_UI.Instance.UpdateScore(score);
        Game_UI.Instance.UpdateGearCount(Serializer.Instance.activeData.gearCount);
        isDead = false;
        //Allows the player to use the slowdown (if they have it) and resets the movement type and last finish hit
        _powerUpData.CanUseSlowDown = true;
        movementType = Serializer.Instance.activeData.settings.movementType;
        lastFinishHit = null;
        positions = new List<Vector3>();
        rotations = new List<Vector3>();
        rotPosTimes = new List<float>();
        next = -1;

        Game_UI.Instance.UpdateShieldCount(_powerUpData.ShieldCount);
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

        //Adds Segments
        AddSegments(add, true);
    }

    //Method called to get which of the current control buttons are being pressed down
    public bool[] GetControls() =>
        moveArr;

    //Property to get the players current shield count
    public int ShieldCount =>
        _powerUpData.ShieldCount;

    //Returns the default movement type for the platform
    public static MovementType DefaultMovementType(Gamemanager.Platform platform) =>
        (platform == Gamemanager.Platform.Windows) ?
            MovementType.Keyboard : MovementType.Buttons;

    #region Use / Control Powerup Methods
    //Method called when the player uses a shield instead of dying
    public void UseShield()
    {
        //Removes a shield, updates the UI, and plays the sound
        _powerUpData.RemoveShields(1);

        if (Serializer.Instance.activeData.settings.soundEnabled)
        {
            audioSource.PlayOneShot(shieldSound);
        }
    }

    //Method called to use the slowdown powerup
    public void UseSlowdown()
    {
        if (_powerUpData.CanUseSlowDown && _powerUpData.SlowdownPercentage != 0)
        {
            StartCoroutine(CountSlowdown());
        }
    }

    //Coroutine used to wait call the powerup for a certain time and wait for reuse
    private IEnumerator CountSlowdown()
    {
        _powerUpData.CanUseSlowDown = false;
        float time = 0;

        Generator.SetRelativeSpeed(_powerUpData.SlowdownPercentage > 1 ? 0.01f : 1 - _powerUpData.SlowdownPercentage);

        Game_UI.Instance.SlowDownCover.gameObject.SetActive(true);
        Game_UI.Instance.SlowDownCover.offsetMax = new Vector2(0, 0);

        if (Serializer.Instance.activeData.settings.soundEnabled)
        {
            audioSource.clip = slowdownTheme;
            audioSource.loop = true;
            audioSource.Play();
        }

        while (time < PowerUpData.SlowdownUseTime)
        {
            time += Time.deltaTime;
            Log(Time.deltaTime / PowerUpData.SlowdownUseTime);
            Game_UI.Instance.SlowDownCover.offsetMax -= new Vector2(0, 100 * Time.deltaTime / PowerUpData.SlowdownUseTime);
            yield return new WaitForEndOfFrame();

            yield return new WaitUntil(() => !isAtFinish);
        }

        audioSource.Stop();
        audioSource.clip = null;
        audioSource.loop = false;

        Generator.SetRelativeSpeed(1);
        time = 0;

        Game_UI.Instance.SlowDownCover.offsetMax = new Vector2(0, 0);

        while (time < PowerUpData.SlowdownTime)
        {
            time += Time.deltaTime;
            Game_UI.Instance.SlowDownCover.offsetMax -= new Vector2(0, 100 * Time.deltaTime / PowerUpData.SlowdownTime);
            yield return new WaitForEndOfFrame();

            yield return new WaitUntil(() => !isAtFinish);
        }

        Game_UI.Instance.SlowDownCover.offsetMax = new Vector2(0, -100);

        _powerUpData.CanUseSlowDown = true;

        yield return null;
    }

    public void UseShock()
    {
        _powerUpData.RemoveShocks(1);

        Reference.gen.DestoryDamageObjects();

        if (Serializer.Instance.activeData.settings.soundEnabled)
        {
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
        if (_powerUpData.SlowdownPercentage == 0)
        {
            //Increases the slow down percentage if it was at 0
            _powerUpData.SlowdownPercentage += percentage;
            Game_UI.Instance.EnableSlowDown(true);
        }
        else
        {
            //Multiply the slow down percentage if it was above 0
            _powerUpData.SlowdownPercentage = 1 - ((1 - _powerUpData.SlowdownPercentage) * (1 - percentage));
        }
    }

    //Method called to decrease the slowed percentage of the slowdown powerup
    public void DecreaseSlowDownPercentage(float percentage, int cardIndex)
    {
        if (Reference.cardTypes[cardIndex].level == 0)
        {
            Log("Level is 0, disabling UI...");
            //Fully disable the slowdown powerup
            _powerUpData.SlowdownPercentage = 0;
            Game_UI.Instance.EnableSlowDown(false, false, true);
        }
        else
        {
            Log("Level is not 0, dividing...");
            //Divide the slow down percentage if it was above 0
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
