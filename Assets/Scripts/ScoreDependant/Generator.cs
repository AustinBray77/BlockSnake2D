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
    //[SerializeField] private GearObject gearPrefab;
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

    Vector3 constantPosition;
    float destroyerPosition;

    //Method called on scene load
    private void Start()
    {
        Log("Rest of start called");

        objects = new List<Object>();
        smallPO = new List<GameObject>();
        mediumPO = new List<GameObject>();
        largePO = new List<GameObject>();

        //Returns if currently in the tutorial
        if (Gamemanager.InLevel("Tutorial"))
            return;

        //Initializes the generator
        Initialize();
    }

    //Method called to initialize the generator
    public void Initialize()
    {
        //Assigns the base data for all of the properties
        if (Gamemanager.InLevel("Tutorial"))
        {
            Object.speed = 5f;
            spawnDelay = 2f;
        }
        else
        {
            switch (Gamemanager.Instance.CurrentMode)
            {
                case Gamemanager.Mode.Normal:
                    Object.speed = 6f;
                    spawnDelay = 1.7f;
                    break;
                case Gamemanager.Mode.Fast:
                    Object.speed = 8f;
                    spawnDelay = 1.4f;
                    break;
            }
        }

        bounds = 10;
        finishesOnScreen = new List<FinishObject>();
        lastFinish = null;
        scoreCounts = new int[scorePrefabs.Length];
        scoreCounts[0] = 1;
        damageCounts = new int[damagePrefabs.Length];
        counterAtLastFinishEnter = 0;
        relativeSpeed = 1;
        destroyerPosition = -transform.position.x;

        UpdateConstantPosition();

        //Starts the object generation and parallax generation
        StartCoroutine(Generate(false));
        StartParallax();
    }

    private void Update()
    {
        if (!Player.isDead && !Player.isAtFinish)
        {
            if (objects.Count >= 1)
            {
                while (objects[0].transform.position.x <= destroyerPosition)
                {
                    Destroy(objects[0].gameObject);

                    if (objects[0].gameObject.name.Contains("Finish"))
                    {
                        OnFinishDestroyed();
                    }

                    objects.RemoveAt(0);

                    if (objects.Count <= 0)
                    {
                        break;
                    }
                }
            }

            if (smallPO.Count >= 1)
            {
                while (smallPO[0].transform.position.x <= destroyerPosition)
                {
                    Destroy(smallPO[0].gameObject);

                    smallPO.RemoveAt(0);

                    if (smallPO.Count <= 0)
                    {
                        break;
                    }
                }

                for (int i = 0; i < smallPO.Count; i++)
                {
                    smallPO[i].transform.position += new Vector3(Object.speed * 0.6f * Time.deltaTime * relativeSpeed, 0);
                }
            }

            if (mediumPO.Count >= 1)
            {
                while (mediumPO[0].transform.position.x <= destroyerPosition)
                {
                    Destroy(mediumPO[0].gameObject);

                    mediumPO.RemoveAt(0);

                    if (mediumPO.Count <= 0)
                    {
                        break;
                    }
                }

                for (int i = 0; i < mediumPO.Count; i++)
                {
                    mediumPO[i].transform.position += new Vector3(Object.speed * 0.75f * Time.deltaTime * relativeSpeed, 0);
                }
            }

            if (largePO.Count >= 1)
            {
                while (largePO[0].transform.position.x <= destroyerPosition)
                {
                    Destroy(largePO[0].gameObject);

                    largePO.RemoveAt(0);

                    if (largePO.Count <= 0)
                    {
                        break;
                    }
                }

                for (int i = 0; i < largePO.Count; i++)
                {
                    largePO[i].transform.position += new Vector3(Object.speed * 0.85f * Time.deltaTime * relativeSpeed, 0);
                }
            }

            transform.position += new Vector3(Object.speed * Time.deltaTime * relativeSpeed, 0);
            destroyerPosition += Object.speed * Time.deltaTime * relativeSpeed;
        }
    }

    //Method called when the player respawns
    public void Respawn()
    {
        //Destroys all objects on screen
        StopAllCoroutines();
        DestroyAllObjects();

        //Resets the damage and score counts
        scoreCounts = (int[])scoreCountsAtLastFinish.Clone();
        damageCounts = (int[])damageCountsAtLastFinish.Clone();
        objects = new List<Object>();
        smallPO = new List<GameObject>();
        mediumPO = new List<GameObject>();
        largePO = new List<GameObject>();
        finishesOnScreen = new List<FinishObject>();
        Object.speed = speedAtLastFinish;
        spawnDelay = spawnSpeedAtLastFinish;

        //Creates the new finish for the player to spawn into
        GameObject nextFinish = Instantiate(finishPrefab, new Vector3(transform.position.x, 0), transform.rotation);
        FinishObject finishObject = nextFinish.GetComponent<FinishObject>();
        finishObject.SetData(lastFinish);
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
        //Changes the position and bounds so the generator is just of to the right
        transform.position += new Vector3(Player.increaseFactor * Reference.cam.aspect * amount, 0);
        destroyerPosition -= Player.increaseFactor * Reference.cam.aspect * amount;
        UpdateConstantPosition();
        bounds += Player.increaseFactor * amount;

        //Decreases the delay between spawns
        for (int i = 0; i < amount; i++)
        {
            spawnDelay /= 1.01f;
        }

        //Triggers if there is a finish on screen
        if (finishesOnScreen.Count >= 1)
        {
            //Generates all of the finishes as the bounds have changed
            foreach (FinishObject finish in finishesOnScreen)
            {
                finish.Generate(false, amount);
            }
        }

        Vector3 increaseAmount = new Vector3(Player.increaseFactor * amount, Player.increaseFactor * amount);
        float calculatedTime = UI.fadeTime * amount;

        foreach (GameObject obj in smallPO)
        {
            obj.transform.LeanScale(obj.transform.localScale + increaseAmount / 10, calculatedTime);
        }

        foreach (GameObject obj in mediumPO)
        {
            obj.transform.LeanScale(obj.transform.localScale + increaseAmount / 2, calculatedTime);
        }

        foreach (GameObject obj in largePO)
        {
            obj.transform.LeanScale(obj.transform.localScale + increaseAmount, calculatedTime);
        }
    }

    //Method called when the player enters a finish
    public void OnPlayerEnterFinish(FinishObject obj)
    {
        //Deep Saves the score counts, damage counts, counter, and finish object data
        scoreCountsAtLastFinish = (int[])scoreCounts.Clone();
        damageCountsAtLastFinish = (int[])damageCounts.Clone();
        counterAtLastFinishEnter = obj.spawnIndex + 1;
        lastFinish = obj.ToData();
        speedAtLastFinish = Object.speed;
        spawnSpeedAtLastFinish = spawnDelay;

        //Debugging
        //Log(counterAtLastFinishEnter);
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

    //Generation loop
    private IEnumerator Generate(bool waitFirst)
    {
        //Float to store the current time waited
        float time = 0;

        //Loops if the generator should wait before spawning, waits for the current time
        while (time < spawnDelay && waitFirst)
        {
            //Adds a set amount of time to time and waits the time
            time += Time.deltaTime;
            yield return new WaitForEndOfFrame();

            //If the player is at a finish, wait until they arent
            yield return new WaitUntil(() => !Player.isAtFinish);
        }

        //Loops until the coroutine is cancelled (when the player dies)
        for (int i = counterAtLastFinishEnter; true; i++)
        {
            List<Vector3Int> positions = new List<Vector3Int>();

            //Triggers if on a 10th iteration
            if (i % 10 == 0 && i > 0)
            {
                //Spawns a finish object
                Vector3Int pos = GenPosition(positions);
                positions.Add(pos);
                GameObject fObj = Instantiate(finishPrefab, pos, transform.rotation);
                FinishObject finishObject = fObj.GetComponent<FinishObject>();
                objects.Add(finishObject);
                finishesOnScreen.Add(finishObject);
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
            else
            {
                //Triggers if i is greater than 10 and on a 2nd iteration and not in the tutorial
                /*if (i > 10 && i % 2 == 0 && !Gamemode.inLevel("Tutorial"))
                {
                    //Spawns a gear
                    Vector3Int pos = GenPosition(positions);
                    positions.Add(pos);
                    SpawnObject(gearPrefab, pos);
                }*/

                //Loops through each score count
                for (int j = 0; j < scoreCounts.Length; j++)
                {
                    //Loops for the score count
                    for (int k = 0; k < scoreCounts[j]; k++)
                    {
                        //Spawns the score object
                        Vector3Int pos = GenPosition(positions);
                        positions.Add(pos);
                        SpawnObject(scorePrefabs[j], pos);
                    }
                }

                //Loops through each damage count
                for (int j = 0; j < damageCounts.Length; j++)
                {
                    //Loops for the damage count
                    for (int k = 0; k < damageCounts[j]; k++)
                    {
                        //Spawns the damage object
                        Vector3Int pos = GenPosition(positions);
                        positions.Add(pos);
                        SpawnObject(damagePrefabs[j], pos);
                    }
                }
            }

            //Resets the time counter
            time = 0;

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

            //Waits for the current wait time
            while (time < wait)
            {
                //Adds a set amount of time to time and waits the time
                time += Time.deltaTime * relativeSpeed;
                yield return new WaitForEndOfFrame();

                //If the player is at a finish, wait until they arent
                yield return new WaitUntil(() => !Player.isAtFinish);
            }

            //Spawns the front parallax object
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

            //Waits for the current wait time
            while (time < wait)
            {
                //Adds a set amount of time to time and waits the time
                time += Time.deltaTime * relativeSpeed;
                yield return new WaitForEndOfFrame();

                //If the player is at a finish, wait until they arent
                yield return new WaitUntil(() => !Player.isAtFinish);
            }

            //Spawns the middle parallax object
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

            //Waits for the current wait time
            while (time < wait)
            {
                //Adds a set amount of time to time and waits the time
                time += Time.deltaTime * relativeSpeed;
                yield return new WaitForEndOfFrame();

                //If the player is at a finish, wait until they arent
                yield return new WaitUntil(() => !Player.isAtFinish);
            }

            //Spawns the back parallax object
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
        //Finds all of the objects
        GameObject[] objs = GameObject.FindGameObjectsWithTag("Object");

        //Destroys all of the objectss
        foreach (GameObject obj in objs)
        {
            Destroy(obj);
        }

        //Finds all of the parallax objects
        GameObject[] paraObjs = GameObject.FindGameObjectsWithTag("Parallax");

        //Destroys all of the objectss
        foreach (GameObject obj in paraObjs)
        {
            Destroy(obj);
        }
    }

    //Method called to spawn an object at a given position
    public GameObject SpawnObject(Object obj, Vector3 position)
    {
        GameObject gameObj = Instantiate(obj.gameObject, position, obj.transform.rotation);
        gameObj.GetComponent<SpriteRenderer>().sortingOrder = _objectLayer;
        objects.Add(gameObj.GetComponent<Object>());
        return gameObj;
    }

    public void RemoveFrontObject()
    {
        objects.RemoveAt(0);
    }

    public GameObject SpawnParallax(Sprite sprite, Vector3 position, int type)
    {
        GameObject gameObj = Instantiate(parallaxPrefab, position, parallaxPrefab.transform.rotation);

        float scale = 1;

        switch (type)
        {
            case 0:
                largePO.Add(gameObj);
                scale = bounds + 2.5f;
                break;
            case 1:
                mediumPO.Add(gameObj);
                scale = (bounds - 10) / 2 + 5;
                break;
            case 2:
                smallPO.Add(gameObj);
                scale = (bounds - 10) / 10 + 1;
                break;
        }

        gameObj.transform.position += new Vector3(scale / 2, 0f);
        gameObj.transform.localScale = new Vector3(scale, scale);

        SpriteRenderer sr = gameObj.GetComponent<SpriteRenderer>();
        sr.sprite = sprite;
        sr.sortingOrder = type;

        return gameObj;
    }

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

    private void UpdateConstantPosition() =>
        constantPosition = transform.position;

    public void SetDestroyerPosition(float x) =>
        destroyerPosition = x;

    public void IncreaseSpawnDelay(float value, int cardIndex = 0) =>
        spawnDelay *= value;

    public void DecreaseSpawnDelay(float value, int cardIndex = 0) =>
        spawnDelay /= value;
}
