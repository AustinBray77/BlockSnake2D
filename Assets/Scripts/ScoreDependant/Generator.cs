using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//Class to control the generator obejct which generates objects
public class Generator : ScoreDependent
{
    private const int _objectLayer = 5;

    //Refrence to the top and bottom wall
    [SerializeField] private GameObject topWall, bottomWall;

    //Refrences to the prefabs that are instantiated
    [SerializeField] private ScoreObject[] scorePrefabs;
    [SerializeField] private DamageObject[] damagePrefabs;
    [SerializeField] private GameObject finishPrefab, parallaxPrefab;
    [SerializeField] private Sprite[] smallParallaxSprites, mediumParallaxSprites, largeParallaxSprites;

    //Stores the amount of eacch object to be spawned, what they were at the last finish, and the counter at the last finish
    private int[] scoreCounts, damageCounts;
    private int[] scoreCountsAtLastFinish, damageCountsAtLastFinish;
    private int counterAtLastFinishEnter;

    //Stores the spawn speed and the data of the last finish the player hit (used for respawning)
    private float spawnDelay;
    private float speedAtLastFinish;
    private float spawnSpeedAtLastFinish;
    public Finish_Data lastFinish;

    //Stores the bounds of the generator and the finish objeccts which are on screen
    private static float bounds;
    private List<FinishObject> finishesOnScreen;

    //Stores the relative speed the gen should operate at
    private static float relativeSpeed;

    //Stores each object on screen
    private List<Object> objects;
    private List<GameObject> smallPO, mediumPO, largePO;

    //Stores position of the destroyer
    float destroyerPosition;

    #region Unity Methods

    //Method called on scene load
    private void Start()
    {
        Log("Rest of start called");

        //Sets defaults for object lists
        objects = new List<Object>();
        smallPO = new List<GameObject>();
        mediumPO = new List<GameObject>();
        largePO = new List<GameObject>();

        //Trigger if not in the tutorial
        if (!Gamemanager.InLevel("Tutorial"))
        {
            //Initializes the generator
            Initialize();
        }
    }

    //Method called each frame
    private void Update()
    {
        //Trigger if the player is not dead or at finish
        if (!Player.isDead && !Player.isAtFinish)
        {
            //Trigger if there are objects on screen
            if (objects.Count >= 1)
            {
                //Loop while the first object in the list (most to the left) is past the destroy position
                while (objects[0].transform.position.x <= destroyerPosition)
                {
                    //Destroy the object
                    DestroyObject(0);

                    //If there are no objects left, break
                    if (objects.Count <= 0)
                    {
                        break;
                    }
                }
            }

            //Trigger if there are small parallax objects on screen
            if (smallPO.Count >= 1)
            {
                //Loop while the first object in the list (most to the left) is past the destroy position
                while (smallPO[0].transform.position.x <= destroyerPosition)
                {
                    //Destroy the game object object
                    Destroy(smallPO[0].gameObject);

                    //Remove it from the list
                    smallPO.RemoveAt(0);

                    //If there are no objects left, break
                    if (smallPO.Count <= 0)
                    {
                        break;
                    }
                }

                //Loop through each object
                for (int i = 0; i < smallPO.Count; i++)
                {
                    //Move it by a small amount to create parallax effect
                    smallPO[i].transform.position += new Vector3(Object.speed * 0.6f * Time.deltaTime * relativeSpeed, 0);
                }
            }

            //Trigger if there are medium parallax objects on screen
            if (mediumPO.Count >= 1)
            {
                //Loop while the first object in the list (most to the left) is past the destroy position
                while (mediumPO[0].transform.position.x <= destroyerPosition)
                {
                    //Destroy the game object object
                    Destroy(mediumPO[0].gameObject);

                    //Remove it from the list
                    mediumPO.RemoveAt(0);

                    //If there are no objects left, break
                    if (mediumPO.Count <= 0)
                    {
                        break;
                    }
                }

                //Loop through each object
                for (int i = 0; i < mediumPO.Count; i++)
                {
                    //Move it by a medium amount to create parallax effect
                    mediumPO[i].transform.position += new Vector3(Object.speed * 0.75f * Time.deltaTime * relativeSpeed, 0);
                }
            }

            //Trigger if there are large parallax objects on screen
            if (largePO.Count >= 1)
            {
                //Loop while the first object in the list (most to the left) is past the destroy position
                while (largePO[0].transform.position.x <= destroyerPosition)
                {
                    //Destroy the game object object
                    Destroy(largePO[0].gameObject);

                    //Remove it from the list
                    largePO.RemoveAt(0);

                    //If there are no objects left, break
                    if (largePO.Count <= 0)
                    {
                        break;
                    }
                }

                //Loop through each object
                for (int i = 0; i < largePO.Count; i++)
                {
                    //Move it by a large amount to create parallax effect
                    largePO[i].transform.position += new Vector3(Object.speed * 0.85f * Time.deltaTime * relativeSpeed, 0);
                }
            }

            //Move the generator and the destroyer by the object speed
            transform.position += new Vector3(Object.speed * Time.deltaTime * relativeSpeed, 0);
            destroyerPosition += Object.speed * Time.deltaTime * relativeSpeed;
        }
    }

    #endregion

    //Method called to initialize the generator
    public void Initialize()
    {
        //Trigger if in tutorial
        if (Gamemanager.InLevel("Tutorial"))
        {
            //Set generation and spawn delay speed
            Object.speed = 5f;
            spawnDelay = 2f;
        }
        else
        {
            //Else switch the current game mode
            switch (Gamemanager.Instance.CurrentMode)
            {
                //Case normal gamemode
                case Gamemanager.Mode.Normal:
                    //Set generation and spawn delay speed
                    Object.speed = 6f;
                    spawnDelay = 1.7f;
                    break;

                //Case fast game mode
                case Gamemanager.Mode.Fast:
                    //Set generation and spawn delay speed
                    Object.speed = 8f;
                    spawnDelay = 1.4f;
                    break;
            }
        }

        /* Set starting values */
        bounds = 10;
        finishesOnScreen = new List<FinishObject>();
        lastFinish = null;
        scoreCounts = new int[scorePrefabs.Length];
        //First score object is always unlocked
        scoreCounts[0] = 1;
        damageCounts = new int[damagePrefabs.Length];
        counterAtLastFinishEnter = 0;
        relativeSpeed = 1;
        destroyerPosition = -transform.position.x;

        //Starts the object generation and parallax generation
        StartCoroutine(Generate(false));
        StartParallax();
    }

    //Method called when the player respawns
    public void Respawn()
    {
        //Destroys all objects on screen
        StopAllCoroutines();
        DestroyAllObjects();

        //Sets score counts to the clones of ones at last finish
        scoreCounts = (int[])scoreCountsAtLastFinish.Clone();
        damageCounts = (int[])damageCountsAtLastFinish.Clone();

        //Resets all object lists
        objects = new List<Object>();
        smallPO = new List<GameObject>();
        mediumPO = new List<GameObject>();
        largePO = new List<GameObject>();
        finishesOnScreen = new List<FinishObject>();

        //Sets speed and spawn delay to values at last finish
        Object.speed = speedAtLastFinish;
        spawnDelay = spawnSpeedAtLastFinish;

        //Instantiates new finish
        GameObject nextFinish = Instantiate(finishPrefab, new Vector3(transform.position.x, 0), transform.rotation);

        //Gets finish object component from game object and sets it from previously saved data
        FinishObject finishObject = nextFinish.GetComponent<FinishObject>();
        finishObject.SetData(lastFinish);

        //Adds finish to objects lists
        finishesOnScreen.Add(finishObject);
        objects.Add(finishObject);

        //Starts the object generation and parallax generation
        StartCoroutine(Generate(true));
        StartParallax();
    }

    //Method called to start the parallax generation
    private void StartParallax()
    {
        //Starts the couroutines for the front, middle, and back parallax
        StartCoroutine(GenerateParallaxFront());
        StartCoroutine(GenerateParallaxMiddle());
        StartCoroutine(GenerateParallaxBack());
    }

    //Method called when segments are added or removed from the player, used for controling the scene scale
    public override void OnSegmentChange(int amount, bool useAnimation)
    {
        //Moves the gen more to the left and destroyer more to the right relative to the camera
        transform.position += new Vector3(Player.increaseFactor * Reference.cam.aspect * amount, 0);
        destroyerPosition -= Player.increaseFactor * Reference.cam.aspect * amount;

        //Increases the bounds as the distance between the gen and destroyer has increased
        bounds += Player.increaseFactor * amount;

        //Loops for the amount of segments added
        for (int i = 0; i < amount; i++)
        {
            //Decrease the spawn delay
            spawnDelay /= 1.01f;
        }

        //Triggers if there is a finish on screen
        if (finishesOnScreen.Count >= 1)
        {
            //Loop for each finish on screen
            foreach (FinishObject finish in finishesOnScreen)
            {
                //Regenerate finish as the bounds have changed
                finish.Generate(false, amount);
            }
        }

        //Converts the increase factor to a vector
        Vector3 increaseAmount = new Vector3(Player.increaseFactor * amount, Player.increaseFactor * amount);

        //Gets the amount of time the animations should take
        float calculatedTime = UI.fadeTime * amount;

        /* Keeps semi constant size for parallax objects on screen - accounts for fov changes */

        //Loops for each small parallax object on screen
        foreach (GameObject obj in smallPO)
        {
            //Animate the object to increase in size by 1/10 of the increase amount
            obj.transform.LeanScale(obj.transform.localScale + increaseAmount / 10, calculatedTime);
        }

        //Loops for each medium parallax object on screen
        foreach (GameObject obj in mediumPO)
        {
            //Animate the object to increase in size by 1/2 of the increase amount
            obj.transform.LeanScale(obj.transform.localScale + increaseAmount / 2, calculatedTime);
        }

        //Loops for each large parallx object
        foreach (GameObject obj in largePO)
        {
            //Animate the object to increase in size by the increase amount
            obj.transform.LeanScale(obj.transform.localScale + increaseAmount, calculatedTime);
        }
    }

    //Method called when the player enters a finish
    public void OnPlayerEnterFinish(FinishObject obj)
    {
        //Deep Saves the counts for both object types
        scoreCountsAtLastFinish = (int[])scoreCounts.Clone();
        damageCountsAtLastFinish = (int[])damageCounts.Clone();

        //Sets the index that the generator is at
        counterAtLastFinishEnter = obj.spawnIndex + 1;

        //Assigns data from finish object
        lastFinish = obj.ToData();

        //Saves speed and spawn delay
        speedAtLastFinish = Object.speed;
        spawnSpeedAtLastFinish = spawnDelay;
    }

    //Method called when the player dies
    public void OnPlayerDeath()
    {
        //Stops all the current running coroutines and assigns finishes on screen to its default value
        StopAllCoroutines();
        finishesOnScreen = new List<FinishObject>();
    }

    //Method called when a finish is destroyed by the destroyer
    public void OnFinishDestroyed()
    {
        //Removes the finish from the finishes on screen
        finishesOnScreen.RemoveAt(0);
    }

    //Method for generating the objects (non parallax)
    private IEnumerator Generate(bool waitFirst)
    {
        //Isolated scope to isolate time variable
        {
            //Float to store the current time waited
            float time = 0;

            //Loops if the generator should wait before spawning, waits for the current time
            while (time < spawnDelay && waitFirst)
            {
                //Increments time by last frame time
                time += Time.deltaTime;

                //Waits for frame end
                yield return new WaitForEndOfFrame();

                //If the player is at a finish, wait until they arent
                yield return new WaitUntil(() => !Player.isAtFinish);
            }
        }

        //Loops until the coroutine is cancelled (when the player dies)
        for (int i = counterAtLastFinishEnter; true; i++)
        {
            //Saves all currently used positions
            List<Vector3Int> positions = new List<Vector3Int>();

            //Triggers if on a 10th iteration
            if (i % 10 == 0 && i > 0)
            {
                //Generate a random position
                Vector3Int pos = GenPosition(positions);

                //Add it to used positions
                positions.Add(pos);

                //Instantiate the finish gameobject
                GameObject fObj = Instantiate(finishPrefab, pos, transform.rotation);

                //Get the component from the game object
                FinishObject finishObject = fObj.GetComponent<FinishObject>();

                //Add it to object lists
                objects.Add(finishObject);
                finishesOnScreen.Add(finishObject);

                //Save index to the finish
                finishesOnScreen[0].spawnIndex = i;

                //Adds 1 to the apple and spike counts if they are less than their max
                scoreCounts[0] += scoreCounts[0] == scorePrefabs[0].maxOnScreen ? 0 : 1;
                damageCounts[0] += damageCounts[0] == damagePrefabs[0].maxOnScreen ? 0 : 1;

                //Triggers if on a 20th iteration
                if (i % 20 == 0)
                {
                    //Adds 1 to the diamond count if it less than its max
                    damageCounts[1] += damageCounts[1] == damagePrefabs[1].maxOnScreen ? 0 : 1;
                }
                //Triggers if on a 30th iteration
                if (i % 30 == 0)
                {
                    //Adds 1 to the impostor and orange counts if they are less than their max
                    damageCounts[2] += damageCounts[2] == damagePrefabs[2].maxOnScreen ? 0 : 1;
                    scoreCounts[1] += scoreCounts[1] == scorePrefabs[1].maxOnScreen ? 0 : 1;
                }
            }
            //Else if not on 10th iteration, generate normally
            else
            {
                //Loops through each score count
                for (int j = 0; j < scoreCounts.Length; j++)
                {
                    //Loops for the score counts value (amount to be spawned)
                    for (int k = 0; k < scoreCounts[j]; k++)
                    {
                        //Generate a random position
                        Vector3Int pos = GenPosition(positions);

                        //Adds the position to used positions
                        positions.Add(pos);

                        //Spawns the score object at the position
                        SpawnObject(scorePrefabs[j], pos);
                    }
                }

                //Loops through each damage count
                for (int j = 0; j < damageCounts.Length; j++)
                {
                    //Loops for the damage count
                    for (int k = 0; k < damageCounts[j]; k++)
                    {
                        //Generate a random position
                        Vector3Int pos = GenPosition(positions);

                        //Adds the position to used positions
                        positions.Add(pos);

                        //Spawns the damage object at the position
                        SpawnObject(damagePrefabs[j], pos);
                    }
                }
            }

            //Float to store the current time waited
            float time = 0;

            //Waits for the current wait time
            while (time < spawnDelay)
            {
                //Adds a set amount of time to time and waits the time
                time += Time.deltaTime * relativeSpeed;
                yield return new WaitForEndOfFrame();

                //If the player is at a finish, wait until they arent
                yield return new WaitUntil(() => !Player.isAtFinish);
            }
        }
    }

    //Enumerator for generating the front layer of the background parallax
    private IEnumerator GenerateParallaxFront()
    {
        //Loops until the coroutine is cancelled (when the player dies)
        while (true)
        {
            //Variables for counting the time waited and the total time to wait
            float time = 0, wait = Random.Range(16f / Object.speed, 80f / Object.speed);

            //Loops while the current time is less than the wait time
            while (time < wait)
            {
                //Increments the time counter by the last frame time and relative speed
                time += Time.deltaTime * relativeSpeed;

                //Waits for the end of the frame
                yield return new WaitForEndOfFrame();

                //If the player is at a finish, wait until they arent
                yield return new WaitUntil(() => !Player.isAtFinish);
            }

            //Spawns a random front parallax object at a random position
            SpawnParallax(Functions.RandomArrItem<Sprite>(smallParallaxSprites), GenPosition(new List<Vector3Int>()), 2);
        }
    }

    //Enumerator for generating the middle layer of the background parallax
    private IEnumerator GenerateParallaxMiddle()
    {
        //Loops until the coroutine is cancelled (when the player dies)
        while (true)
        {
            //Variables for counting the time waited and the total time to wait
            float time = 0, wait = Random.Range(320f / Object.speed, 480f / Object.speed);

            //Loops while the current time is less than the wait time
            while (time < wait)
            {
                //Increments the time counter by the last frame time and relative speed
                time += Time.deltaTime * relativeSpeed;

                //Waits for the end of the frame
                yield return new WaitForEndOfFrame();

                //If the player is at a finish, wait until they arent
                yield return new WaitUntil(() => !Player.isAtFinish);
            }

            //Spawns a random middle parallax object at a random position
            SpawnParallax(Functions.RandomArrItem<Sprite>(mediumParallaxSprites), GenPosition(new List<Vector3Int>()), 1);
        }
    }

    //Enumerator for generating the back layer of the background parallax
    private IEnumerator GenerateParallaxBack()
    {
        //Loops until the coroutine is cancelled (when the player dies)
        while (true)
        {
            //Variables for counting the time waited and the total time to wait
            float time = 0, wait = Random.Range(800f / Object.speed, 960f / Object.speed);

            //Loops while the current time is less than the wait time
            while (time < wait)
            {
                //Increments the time counter by the last frame time and relative speed
                time += Time.deltaTime * relativeSpeed;

                //Waits for the end of the frame
                yield return new WaitForEndOfFrame();

                //If the player is at a finish, wait until they arent
                yield return new WaitUntil(() => !Player.isAtFinish);
            }

            //Spawns a random back parallax object at a random position
            SpawnParallax(Functions.RandomArrItem<Sprite>(largeParallaxSprites), GenPosition(new List<Vector3Int>()), 0);
        }
    }

    //Method called to generate a position at which there are no objects
    public Vector3Int GenPosition(List<Vector3Int> takenPositions)
    {
        //Randomly generates a postion
        Vector3Int pos = new Vector3Int((int)transform.position.x - (int)Random.Range(0, 7.5f), (int)Random.Range(-bounds, bounds), 0);

        //Randomly generates a postion if the current one is taken (will be reworked as there is a possibility of stack overflow)
        while (takenPositions.Contains(pos))
        {
            pos = new Vector3Int((int)transform.position.x - (int)Random.Range(0, 7.5f), (int)Random.Range(-bounds, bounds), 0);
        }

        //Returns the new position
        return pos;
    }

    //Method called to destroy all objects on screen
    public void DestroyAllObjects()
    {
        //Finds all of the objects in the scene
        GameObject[] objs = GameObject.FindGameObjectsWithTag("Object");

        //Loops for each of the objects
        foreach (GameObject obj in objs)
        {
            //Destroys the object
            Destroy(obj);
        }

        //Finds all of the parallax objects
        GameObject[] paraObjs = GameObject.FindGameObjectsWithTag("Parallax");

        //Loops for each of the parallax objects
        foreach (GameObject obj in paraObjs)
        {
            //Destroys the object
            Destroy(obj);
        }
    }

    //Method called to spawn an object at a given position
    public GameObject SpawnObject(Object obj, Vector3 position)
    {
        //Instantiates the game object
        GameObject gameObj = Instantiate(obj.gameObject, position, obj.transform.rotation);

        //Sets the sprite layer on the object
        gameObj.GetComponent<SpriteRenderer>().sortingOrder = _objectLayer;

        //Adds the object component to the object list
        objects.Add(gameObj.GetComponent<Object>());

        //Returns the game object
        return gameObj;
    }

    //Method called to spawn a parallax object at a given position of a given type
    public GameObject SpawnParallax(Sprite sprite, Vector3 position, int type)
    {
        //Instantiates the prefab
        GameObject gameObj = Instantiate(parallaxPrefab, position, parallaxPrefab.transform.rotation);

        //Stoes the scale for the object (base is 1)
        float scale = 1;

        //Switch the type of the parallax object
        switch (type)
        {
            //Case large object
            case 0:
                //Adds the object to the object list
                largePO.Add(gameObj);

                //Sets the scale to be large
                scale = bounds + 2.5f;
                break;

            //Case medium object
            case 1:
                //Adds the object to the object list
                mediumPO.Add(gameObj);

                //Sets the scale to be medium
                scale = (bounds - 10) / 2 + 5;
                break;

            //Case small object
            case 2:
                //Adds the object to the object list
                smallPO.Add(gameObj);

                //Sets the scale to be small
                scale = (bounds - 10) / 10 + 1;
                break;
        }

        //Moves the position left by half of the scale
        gameObj.transform.position += new Vector3(scale / 2, 0f);

        //Assigns the scale from the float
        gameObj.transform.localScale = new Vector3(scale, scale);

        //Assigns the sprite and sprite layer
        SpriteRenderer sr = gameObj.GetComponent<SpriteRenderer>();
        sr.sprite = sprite;
        sr.sortingOrder = type;

        //Returns the instantiated object
        return gameObj;
    }

    //Method to destroy all damage objects on screen
    public void DestoryDamageObjects()
    {
        //Gets all damage objects in the scene
        GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("Damage");

        //Loops for each object
        foreach (GameObject obj in gameObjects)
        {
            //Removes the object from the object list
            objects.Remove(obj.GetComponent<Object>());

            //Destorys the object
            Destroy(obj);
        }
    }

    //Method to destroy an object at a given index
    public void DestroyObject(int index)
    {
        //Return if the index is out of bounds
        if (index < 0 || index >= objects.Count)
            return;

        //Trigger if the object is a finish object
        if (objects[index].gameObject.name.Contains("Finish"))
        {
            //Calls on finish destroyed
            OnFinishDestroyed();
        }

        //Destroy the object at the index
        Destroy(objects[index].gameObject);

        //Removes the object form the object list
        objects.RemoveAt(index);
    }

    #region Getters & Setters

    //Method called to get the bounds distance of the generator
    public static float GetBoundsDistance() =>
        2 * bounds;

    //Method called to set the bounds of the generator
    public static void SetBounds(float val) =>
        bounds = val;

    //Getter and Setter for relative speed
    public static float GetRelativeSpeed() =>
        relativeSpeed;

    public static void SetRelativeSpeed(float _relativeSpeed) =>
        relativeSpeed = _relativeSpeed;

    public void SetDestroyerPosition(float x) =>
        destroyerPosition = x;

    public void IncreaseSpawnDelay(float value, int cardIndex = 0) =>
        spawnDelay *= value;

    public void DecreaseSpawnDelay(float value, int cardIndex = 0) =>
        spawnDelay /= value;

    #endregion
}
