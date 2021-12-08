using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Class to control the generator obejct which generates objects
public class Generator : MonoBehaviour
{
    //Refrence to the top and bottom wall
    [SerializeField] private GameObject topWall, bottomWall;

    //Refrences to the prefabs that are instantiated
    [SerializeField] private ScoreObject[] scorePrefabs;
    [SerializeField] private DamageObject[] damagePrefabs;
    [SerializeField] private GearObject gearPrefab;
    [SerializeField] private GameObject finishPrefab;
    [SerializeField] private GameObject frontStarPrefab, middleStarPrefab, backStarPrefab;

    //Stores the amount of eacch object to be spawned, what they were at the last finish, and the counter at the last finish
    private int[] scoreCounts, damageCounts;
    private int[] scoreCountsAtLastFinish, damageCountsAtLastFinish;
    private int counterAtLastFinishSpawn;

    //Stores the offset of how many iterations it should wait before moving again, { backup <= 0 }
    public int backup;

    //Stores the spawn speed and the data of the last finish the player hit (used for respawning)
    public float spawnSpeed;
    public Finish_Data lastFinish;

    //Stores the bounds of the generator and the finish objeccts which are on screen
    private static float bounds;
    private List<FinishObject> finishesOnScreen;

    //Method called on scene load
    private void Start()
    {
        //Returns if currently in the tutorial
        if (Gamemode.inLevel("Tutorial"))
            return;

        //Initializes the generator
        Initialize();
    }

    //Method called to initialize the generator
    public void Initialize()
    {
        //Assigns the base data for all of the properties
        Object.speed = 5;
        spawnSpeed = 2;
        bounds = 10;
        backup = 0;
        finishesOnScreen = new List<FinishObject>();
        lastFinish = null;
        scoreCounts = new int[scorePrefabs.Length];
        scoreCounts[0] = 1;
        damageCounts = new int[damagePrefabs.Length];
        counterAtLastFinishSpawn = 0;

        //Starts the object generation and parallax generation
        StartCoroutine(Generate(false));
        StartParallax();
    }

    //Method called when the player respawns
    public void Respawn()
    {
        //Destroys all objects on screen
        DestroyAllObjects();

        //Calls the base start function if the player never reached a finish / check point.
        if (lastFinish == null)
        {
            Start();
            return;
        }

        //Resets the damage and score counts
        scoreCounts = scoreCountsAtLastFinish;
        damageCounts = damageCountsAtLastFinish;

        //Creates the new finish for the player to spawn into
        GameObject nextFinish = Instantiate(finishPrefab, new Vector3(transform.position.x, 0), transform.rotation);
        nextFinish.GetComponent<FinishObject>().SetData(lastFinish);

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
    public void OnSegmentChange(int amount)
    {
        //Triggers if the destroyer should wait an amount of iterations
        if (backup != 0)
        {
            //Triggers if that iteration amount is less than the amount of segments added
            if (backup + amount > 0)
            {
                //Changes the position and bounds so the generator is just of to the right
                transform.position += new Vector3(Player.increaseFactor * 2 * (backup + amount), 0);
                bounds += Player.increaseFactor * (backup + amount);

                //Decreases the delay between spawns
                for (int i = 0; i < (backup + amount); i++)
                {
                    spawnSpeed /= 1.01f;
                }

                //Resets backup
                backup = 0;
            }
            //Else just uniterates backup by the amount
            else
            {
                backup += amount;
            }
        }
        //Else just changes the position and bounds so the generator is just of to the right
        else
        {
            //Changes the position and bounds so the generator is just of to the right
            transform.position += new Vector3(Player.increaseFactor * 2 * amount, 0);
            bounds += Player.increaseFactor * amount;

            //Decreases the delay between spawns
            for (int i = 0; i < amount; i++)
            {
                spawnSpeed /= 1.01f;
            }
        }

        //Triggers if there is a finish on screen
        if (finishesOnScreen.Count >= 1)
        {
            //Generates all of the finishes as the bounds have changed
            foreach (FinishObject finish in finishesOnScreen)
            {
                finish.Generate();
            }
        }
    }

    //Method called when the player enters a finish
    public void OnPlayerEnterFinish(FinishObject obj)
    {
        //Saves the score counts, damage counts, counter, and finish object data
        scoreCountsAtLastFinish = scoreCounts;
        damageCountsAtLastFinish = damageCounts;
        counterAtLastFinishSpawn = obj.spawnIndex + 1;
        lastFinish = obj.ToData(); 
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
        while (time < spawnSpeed && waitFirst)
        {
            //Adds a set amount of time to time and waits the time
            time += Time.fixedDeltaTime;
            yield return new WaitForSeconds(Time.fixedDeltaTime);

            //If the player is at a finish, wait until they arent
            yield return new WaitUntil(() => { return !Player.isAtFinish; });
        }

        //Loops until the coroutine is cancelled (when the player dies)
        for (int i = counterAtLastFinishSpawn; true; i++)
        {
            List<Vector3Int> positions = new List<Vector3Int>();

            //Triggers if on a 10th iteration
            if (i % 10 == 0 && i > 0)
            {
                //Spawns a finish object
                Vector3Int pos = GenPosition(positions);
                positions.Add(pos);
                GameObject fObj = Instantiate(finishPrefab, pos, transform.rotation);
                finishesOnScreen.Add(fObj.GetComponent<FinishObject>());
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
                if(i % 30 == 0)
                {
                    //Adds 1 to the impostor and orange counts if they are less than their max
                    damageCounts[2] += damageCounts[2] == damagePrefabs[2].maxOnScreen ? 0 : 1;
                    scoreCounts[1] += scoreCounts[1] == scorePrefabs[1].maxOnScreen ? 0 : 1;
                }
            }
            else
            {
                //Triggers if i is greater than 10 and on a 2nd iteration and not in the tutorial
                if(i > 10 && i % 2 == 0 && !Gamemode.inLevel("Tutorial"))
                {
                    //Spawns a gear
                    Vector3Int pos = GenPosition(positions);
                    positions.Add(pos);
                    SpawnObject(gearPrefab, pos);
                }

                //Loops through each score count
                for(int j = 0; j < scoreCounts.Length; j++)
                {
                    //Loops for the score count
                    for(int k = 0; k < scoreCounts[j]; k++)
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
            while (time < spawnSpeed)
            {
                //Adds a set amount of time to time and waits the time
                time += Time.deltaTime;
                yield return new WaitForEndOfFrame();

                //If the player is at a finish, wait until they arent
                yield return new WaitUntil(() => { return !Player.isAtFinish; });
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
            float time = 0, wait = Random.Range(3f, 4f);

            //Waits for the current wait time
            while (time < wait)
            {
                //Adds a set amount of time to time and waits the time
                time += Time.deltaTime;
                yield return new WaitForEndOfFrame();

                //If the player is at a finish, wait until they arent
                yield return new WaitUntil(() => { return !Player.isAtFinish; });
            }

            //Spawns the front parallax object
            Instantiate(frontStarPrefab, GenPosition(new List<Vector3Int>()), frontStarPrefab.transform.rotation);
        }
    }

    //Enumerator for generating the middle layer of the background parallax
    private IEnumerator GenerateParallaxMiddle()
    {
        //Loops until the coroutine is cancelled (when the player dies)
        while (true)
        {
            //Variables for counting the time waited and the total time to wait
            float time = 0, wait = Random.Range(4f, 6f);

            //Waits for the current wait time
            while (time < wait)
            {
                //Adds a set amount of time to time and waits the time
                time += Time.deltaTime;
                yield return new WaitForEndOfFrame();

                //If the player is at a finish, wait until they arent
                yield return new WaitUntil(() => { return !Player.isAtFinish; });
            }

            //Spawns the middle parallax object
            Instantiate(middleStarPrefab, GenPosition(new List<Vector3Int>()), frontStarPrefab.transform.rotation);
        }
    }

    //Enumerator for generating the back layer of the background parallax
    private IEnumerator GenerateParallaxBack()
    {
        //Loops until the coroutine is cancelled (when the player dies)
        while (true)
        {
            //Variables for counting the time waited and the total time to wait
            float time = 0, wait = Random.Range(6f, 10f);

            //Waits for the current wait time
            while (time < wait)
            {
                //Adds a set amount of time to time and waits the time
                time += Time.deltaTime;
                yield return new WaitForEndOfFrame();

                //If the player is at a finish, wait until they arent
                yield return new WaitUntil(() => { return !Player.isAtFinish; });
            }

            //Spawns the back parallax object
            Instantiate(backStarPrefab, GenPosition(new List<Vector3Int>()), frontStarPrefab.transform.rotation);
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
        foreach(GameObject obj in objs)
        {
            Destroy(obj);
        }
    }

    //Method called to spawn an object at a given position
    public GameObject SpawnObject(Object obj, Vector3 position)
    {
        return Instantiate(obj.gameObject, position, transform.rotation);
    }

    //Method called to get the bounds distance of the generator
    public static float GetBoundsDistance() =>
        2 * bounds;

    //Method called to set the bounds of the generator
    public static void SetBounds(float val) =>
        bounds = val;
}
