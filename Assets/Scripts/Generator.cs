using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Generator : MonoBehaviour
{
    [SerializeField] private GameObject topWall, bottomWall;
    [SerializeField] private ScoreObject[] scorePrefabs;
    [SerializeField] private DamageObject[] damagePrefabs;
    [SerializeField] private GameObject finishPrefab;
    [SerializeField] private GameObject frontStarPrefab, middleStarPrefab, backStarPrefab;

    private int[] scoreCounts, damageCounts;
    private int[] scoreCountsAtLastFinish, damageCountsAtLastFinish;
    private int counterAtLastFinishSpawn;

    public int backup;
    public float spawnSpeed;
    public Finish_Data lastFinish;

    private static float bounds;
    private List<FinishObject> finishesOnScreen;

    private void Start()
    {
        Object.speed = 5;
        spawnSpeed = 4;
        bounds = 10;
        backup = 0;

        finishesOnScreen = new List<FinishObject>();
        lastFinish = null;

        scoreCounts = new int[scorePrefabs.Length];
        scoreCounts[0] = 1;

        damageCounts = new int[damagePrefabs.Length];

        counterAtLastFinishSpawn = 0;

        StartCoroutine(Generate(false));
        StartParallax();
    }

    public void Respawn()
    {
        DestroyAllObjects();

        if (lastFinish == null)
        {
            Start();
            return;
        }

        scoreCounts = scoreCountsAtLastFinish;
        damageCounts = damageCountsAtLastFinish;

        GameObject nextFinish = Instantiate(finishPrefab, new Vector3(transform.position.x, 0), transform.rotation);
        nextFinish.GetComponent<FinishObject>().SetData(lastFinish);
        StartCoroutine(Generate(true));
        StartParallax();
    }

    private void StartParallax()
    {
        StartCoroutine(GenerateParallaxFront());
        StartCoroutine(GenerateParallaxMiddle());
        StartCoroutine(GenerateParallaxBack());
    }

    public void OnSegmentChange(int amount)
    {
        if (backup != 0)
        {
            if (backup + amount > 0)
            {
                transform.position += new Vector3(Player.increaseFactor * 2 * (backup + amount), 0);

                bounds += Player.increaseFactor * (backup + amount);

                for (int i = 0; i < (backup + amount); i++)
                    spawnSpeed /= 1.01f;

                backup = 0;
            }
            else
            {
                backup += amount;
            }
        }
        else
        {
            transform.position += new Vector3(Player.increaseFactor * 2 * amount, 0);

            bounds += Player.increaseFactor * amount;

            for (int i = 0; i < amount; i++)
                spawnSpeed /= 1.01f;
        }

        if (finishesOnScreen.Count >= 1)
            foreach (FinishObject finish in finishesOnScreen)
                finish.Generate();
    }

    public void OnPlayerEnterFinish(FinishObject obj)
    {
        scoreCountsAtLastFinish = scoreCounts;
        damageCountsAtLastFinish = damageCounts;

        counterAtLastFinishSpawn = obj.spawnIndex + 1;

        lastFinish = obj.ToData(); 
    }

    public void OnPlayerDeath()
    {
        StopAllCoroutines();

        finishesOnScreen = new List<FinishObject>();
    }

    public void OnFinishDestroyed()
    {
        finishesOnScreen.RemoveAt(0);
    }

    private IEnumerator Generate(bool waitFirst)
    {
        float time = 0;

        while (time < spawnSpeed && waitFirst)
        {
            time += Time.fixedDeltaTime;

            yield return new WaitForSeconds(Time.fixedDeltaTime);

            yield return new WaitUntil(() => { return !Player.isAtFinish; });
        }

        for (int i = counterAtLastFinishSpawn; true; i++)
        {
            List<Vector3Int> positions = new List<Vector3Int>();

            if (i % 10 == 0 && i > 0)
            {
                Vector3Int pos = GenPosition(positions);
                positions.Add(pos);
                GameObject fObj = Instantiate(finishPrefab, pos, transform.rotation);
                finishesOnScreen.Add(fObj.GetComponent<FinishObject>());
                finishesOnScreen[0].spawnIndex = i;

                scoreCounts[0] += scoreCounts[0] == scorePrefabs[0].maxOnScreen ? 0 : 1;
                damageCounts[0] += damageCounts[0] == damagePrefabs[0].maxOnScreen ? 0 : 1;

                if (i % 20 == 0)
                {
                    damageCounts[1] += damageCounts[1] == damagePrefabs[1].maxOnScreen ? 0 : 1;
                }
                if(i % 30 == 0)
                {
                    scoreCounts[1] += scoreCounts[1] == scorePrefabs[1].maxOnScreen ? 0 : 1;
                }
            }
            else
            {
                for(int j = 0; j < scoreCounts.Length; j++)
                {
                    for(int k = 0; k < Mathf.Min(scoreCounts[j], 5); k++)
                    {
                        Vector3Int pos = GenPosition(positions);
                        positions.Add(pos);
                        SpawnObject(scorePrefabs[j], pos);
                    }
                }

                for (int j = 0; j < damageCounts.Length; j++)
                {
                    for (int k = 0; k < Mathf.Min(damageCounts[j], 5); k++)
                    {
                        Vector3Int pos = GenPosition(positions);
                        positions.Add(pos);
                        SpawnObject(damagePrefabs[j], pos);
                    }
                }
            }

            time = 0;

            while (time < spawnSpeed)
            {
                time += Time.fixedDeltaTime;

                yield return new WaitForSeconds(Time.fixedDeltaTime);

                yield return new WaitUntil(() => { return !Player.isAtFinish; });
            }
        }
    }

    private IEnumerator GenerateParallaxFront()
    {
        while (true)
        {
            float time = 0, wait = Random.Range(3f, 4f);

            while (time < wait)
            {
                time += Time.fixedDeltaTime;

                yield return new WaitForSeconds(Time.fixedDeltaTime);

                yield return new WaitUntil(() => { return !Player.isAtFinish; });
            }

            Instantiate(frontStarPrefab, GenPosition(new List<Vector3Int>()), frontStarPrefab.transform.rotation);
        }
    }

    private IEnumerator GenerateParallaxMiddle()
    {
        while (true)
        {
            float time = 0, wait = Random.Range(4f, 6f);

            while (time < wait)
            {
                time += Time.fixedDeltaTime;

                yield return new WaitForSeconds(Time.fixedDeltaTime);

                yield return new WaitUntil(() => { return !Player.isAtFinish; });
            }

            Instantiate(middleStarPrefab, GenPosition(new List<Vector3Int>()), frontStarPrefab.transform.rotation);
        }
    }

    private IEnumerator GenerateParallaxBack()
    {
        while (true)
        {
            float time = 0, wait = Random.Range(6f, 10f);

            while (time < wait)
            {
                time += Time.fixedDeltaTime;

                yield return new WaitForSeconds(Time.fixedDeltaTime);

                yield return new WaitUntil(() => { return !Player.isAtFinish; });
            }

            Instantiate(backStarPrefab, GenPosition(new List<Vector3Int>()), frontStarPrefab.transform.rotation);
        }
    }

    public Vector3Int GenPosition(List<Vector3Int> takenPositions)
    {
        Vector3Int pos = new Vector3Int((int)transform.position.x, (int)Random.Range(-bounds, bounds), 0);

        while(takenPositions.Contains(pos))
            pos = new Vector3Int((int)transform.position.x, (int)Random.Range(-bounds, bounds), 0);

        return pos;
    }

    public void DestroyAllObjects()
    {
        GameObject[] objs = GameObject.FindGameObjectsWithTag("Object");

        foreach(GameObject obj in objs)
        {
            Destroy(obj);
        }
    }

    public GameObject SpawnObject(Object obj, Vector3 position)
    {
        return Instantiate(obj.gameObject, position, transform.rotation);
    }

    public static float GetBoundsDistance() =>
        2 * bounds;

    public static void SetBounds(float val) =>
        bounds = val;
}
