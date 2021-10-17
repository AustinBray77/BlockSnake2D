using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : MonoBehaviour
{
    [SerializeField] private TMP_Text scoreText; 
    [SerializeField] private TMP_Text gearText; 
    [SerializeField] private Segment segmentAfter;
    [SerializeField] private Segment segmentLast;
    [SerializeField] private GameObject segmentPrefab;
    [SerializeField] private Image fadePanel;

    private Rigidbody2D rb;

    [HideInInspector] public float turnDelay;
    [HideInInspector] public float speed;
    [HideInInspector] public float rotSpeed;
    [HideInInspector] public Card lastUpgraded;

    private bool[] moveArr = new bool[] { false, false };

    public static float increaseFactor = 1.5f;
    public static bool isDead, isAtFinish;
    public static int score, scoreAtLastFinish, segmentCountAtLastFinish, backupAtLastFinish;
    public static float xBoundAtLastFinish, yBoundAtLastFinish;

    public static int xStartBounds = 30, yStartBounds = 12;
    
    public static Skin activeSkin => Shop_UI.skins[Serializer.activeData.activeSkin];

    public static Level level;
    public int gearCount;

    private int camBackup;

    private void Start()
    {
        StartCoroutine(AnimationPlus.FadeToColor(fadePanel, new Color(0, 0, 0, 0), 1f, false));

        rb = GetComponent<Rigidbody2D>();
        isDead = false;
        isAtFinish = false;
        score = 0;
        scoreAtLastFinish = 0;
        camBackup = 0;
        turnDelay = 0.2f;
        speed = 100000;
        rotSpeed = 10;
        gearCount = Serializer.activeData.gearCount;
        level = Serializer.activeData.level;
        gearText.text = gearCount.ToString();

        GetComponent<SpriteRenderer>().sprite = activeSkin.frontSprite;
        segmentPrefab.GetComponent<SpriteRenderer>().sprite = activeSkin.segmentSprite;
    }

    private void Update()
    {
        if (isAtFinish || isDead)
            rb.velocity = new Vector2(0, 0);

        rb.angularVelocity = 0;

        if (Input.GetKey(KeyCode.W) || moveArr[0])
        {
            Move(1);
        }
        if(Input.GetKey(KeyCode.S) || moveArr[1])
        {
            Move(-1);
        }

        if (transform.rotation.eulerAngles.z != 0 && !isAtFinish && !isDead)
        {
            if(transform.rotation.eulerAngles.z >= 270)
            {
                rb.velocity = new Vector2(0, 0);
                rb.AddForce(Vector2.up * Time.deltaTime * -speed * ((transform.rotation.eulerAngles.z - 360) * -1 / 90));
            }
            else
            {
                rb.velocity = new Vector2(0, 0);
                rb.AddForce(Vector2.up * Time.deltaTime * speed * (transform.rotation.eulerAngles.z / 90));
            }
        }

        if (segmentAfter != null && !isDead && !isAtFinish)
            StartCoroutine(MoveNext(transform.position - (transform.right * 1.5f), transform.rotation.eulerAngles));
    }

    public void OnMovePointerDown(int button)
    {
        moveArr[button] = true;
    }

    public void OnMovePointerUp(int button)
    {
        moveArr[button] = false;
    }

    public void Move(int direction)
    {
        float nextZ = transform.rotation.eulerAngles.z + (rotSpeed * direction * Time.deltaTime);
        transform.rotation = Quaternion.Euler(0, 0, 
            nextZ < 270 && nextZ > 90 ? 
                direction == -1 ? 
                    270 : 90 
                        : nextZ);
    }

    public IEnumerator MoveNext(Vector3 position, Vector3 rotation)
    {
        float time = 0;

        while(time < turnDelay)
        {
            time += Time.fixedDeltaTime;

            yield return new WaitForSeconds(Time.fixedDeltaTime);

            if (isAtFinish)
                yield return new WaitUntil(() => { return !isAtFinish; });

            if (segmentAfter == null)
                yield return 0;
        }

        if (segmentAfter != null)
            segmentAfter.SetMovement(position, rotation, turnDelay);

        yield return 0;
    }

    public void KillPlayer()
    {
        StopAllSegmentCoroutines();

        if(Serializer.activeData.highScore < score)
        {
            Serializer.activeData.SetHighScore(score);
        }

        Serializer.activeData.SetGearCount(gearCount);

        //Temp set
        Serializer.activeData.SetLevel(level);

        isDead = true;
        Refrence.gen.OnPlayerDeath();
        Refrence.gameUI.SetActive(false);
        Refrence.deathUI.SetActive(true);
        Refrence.deathUI.GetComponent<Death_UI>().OnLoad();
    }

    public void Respawn()
    {
        lastUpgraded.RemoveLevel();
        MoveWhole(new Vector3(0, 0), new Vector3(0, 0, 0));

        score = scoreAtLastFinish;

        Refrence.cam.orthographicSize = yBoundAtLastFinish;
        Refrence.wallTop.transform.position = new Vector3(0, yBoundAtLastFinish - 0.5f);
        Refrence.wallBottom.transform.position = new Vector3(0, -yBoundAtLastFinish + 0.5f);
        Refrence.gen.transform.position = new Vector3(xBoundAtLastFinish, 0);
        Generator.SetBounds(yBoundAtLastFinish - 2);
        Refrence.des.transform.position = new Vector3(-xBoundAtLastFinish, 0);

        SetBackups(backupAtLastFinish);

        if (CountSegments() - segmentCountAtLastFinish > 0)
            RemoveSegments(CountSegments() - segmentCountAtLastFinish);
        else if (CountSegments() - segmentCountAtLastFinish < 0)
            AddSegments((CountSegments() - segmentCountAtLastFinish) * -1);

        Refrence.gameUI.SetActive(true);
        scoreText.text = score.ToString();
        isDead = false;
    }

    public void OnEnterFinish()
    {
        segmentCountAtLastFinish = CountSegments();
        xBoundAtLastFinish = Refrence.gen.transform.position.x;
        yBoundAtLastFinish = Refrence.cam.orthographicSize;
        backupAtLastFinish = camBackup;
    }

    public void AddGears(int amount)
    {
        gearCount += amount;

        if (gearCount > 9999)
            gearCount = 9999;

        level.AddXP(amount);

        gearText.text = gearCount.ToString();
    }

    public void AddScore(int add)
    {
        score += add;
        scoreText.text = score.ToString();
        level.AddXP(add);

        AddSegments(add);
    }

    public void AddSegments(int add)
    {
        for (int i = 0; i < add; i++)
        {
            if (segmentLast != null)
            {
                GameObject newSegment = Instantiate(segmentPrefab, segmentLast.transform.position - new Vector3(1.5f, 0f), segmentLast.transform.rotation);

                segmentLast.segmentAfter = newSegment.GetComponent<Segment>();
                segmentLast = newSegment.GetComponent<Segment>();
            }
            else
            {
                GameObject newSegment = Instantiate(segmentPrefab, transform.position - new Vector3(1.5f, 0f), transform.rotation);

                segmentAfter = newSegment.GetComponent<Segment>();
                segmentLast = newSegment.GetComponent<Segment>();
            }

            Object.speed *= 1.01f;
        }

        if (camBackup != 0)
            if (camBackup + add > 0)
            {
                Refrence.cam.orthographicSize += increaseFactor * add;
                camBackup = 0;
            }
            else
                camBackup += add;
        else
            Refrence.cam.orthographicSize += increaseFactor * add;

        Refrence.gen.OnSegmentChange(add);
        Refrence.des.OnSegmentChange(add);
        Refrence.wallTop.OnSegmentChange(add);
        Refrence.wallBottom.OnSegmentChange(add);
    }

    public void SetBackups(int amount)
    {
        camBackup = amount;
        Refrence.gen.backup = amount;
        Refrence.des.backup = amount;
        Refrence.wallTop.backup = amount;
        Refrence.wallBottom.backup = amount;
    }

    public void RemoveSegments(int remove)
    {
        Segment cur = segmentAfter;
        int amountRemoved = 0;
        List<Segment> segmentList = new List<Segment>();

        while(cur != null)
        {
            segmentList.Add(cur);
            cur = cur.segmentAfter;
        }

        for(int i = Mathf.Max(0, segmentList.Count - remove); i < segmentList.Count; i++, amountRemoved++)
        {
            Destroy(segmentList[i].gameObject);
        }

        if (segmentList.Count - remove - 1 >= 0)
        {
            segmentLast = segmentList[segmentList.Count - remove - 1];
            segmentLast.segmentAfter = null;
        }
        else
        {
            segmentLast = null;
            segmentAfter = null;
        }
    }

    private void MoveWhole(Vector3 position, Vector3 rotation, Segment segment = null)
    {
        if (segment == null)
        {
            transform.position = position;
            transform.rotation = Quaternion.Euler(rotation);

            if(segmentAfter != null)
            {
                MoveWhole(position - transform.right * 1.5f, rotation, segmentAfter);
            }
        } else
        {
            segment.transform.position = position;
            segment.transform.rotation = Quaternion.Euler(rotation);

            if (segment.segmentAfter != null)
            {
                MoveWhole(position - transform.right * 1.5f, rotation, segment.segmentAfter);
            }
        }
    }

    private void StopAllSegmentCoroutines(Segment segment = null)
    {
        if(segment == null)
        {
            StopAllCoroutines();

            if(segmentAfter != null)
            {
                StopAllSegmentCoroutines(segmentAfter);
            }
        } else
        {
            segment.StopAllCoroutines();

            if(segment.segmentAfter != null)
            {
                StopAllSegmentCoroutines(segment.segmentAfter);
            }
        }
    }

    private int CountSegments()
    {
        int output = 0;

        var cur = segmentAfter;

        while(cur != null)
        {
            output++;
            cur = cur.segmentAfter;
        }

        return output;
    }
}
