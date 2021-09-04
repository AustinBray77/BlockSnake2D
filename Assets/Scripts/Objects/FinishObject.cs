using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishObject : Object
{
    [SerializeField] private GameObject tilePrefab;
    private BoxCollider2D _collider;
    [HideInInspector] public Card[] selectedCards;
    private bool fromData = false;

    [HideInInspector] public int spawnIndex;

    internal override void ObjAwake()
    {
        _collider = GetComponent<BoxCollider2D>();
        Generate();
    }

    private int[] ShuffleCards()
    {
        int[] vals = new int[Refrence.cardTypes.Length];

        for(int i = 0; i < vals.Length; i++)
        {
            vals[i] = i;
        }

        for (int i = 0; i < Refrence.cardTypes.Length; i++)
        {
            int index = Random.Range(0, vals.Length);
            int holder = vals[i];
            vals[i] = vals[index];
            vals[index] = holder;
        }

        return vals;
    }

    public void Generate()
    {
        List<Transform> children = transform.GetComponentsInChildren<Transform>().ToList();
        
        while(children.Count > 0)
        {
            if(children[0] != transform)
                Destroy(children[0].gameObject);

            children.RemoveAt(0);
        }

        for(int i = 0; i >= -(Mathf.Ceil(Generator.GetBoundsDistance()) + 2); i--)
        {
            for(int j = 0; j < 2; j++)
            {
                GameObject tile = Instantiate(tilePrefab, transform);
                tile.transform.localPosition = new Vector3(j, i);
            }
        }

        transform.position = new Vector3(transform.position.x, Generator.GetBoundsDistance() / 2f + 1);
        _collider.size = new Vector3(2, Generator.GetBoundsDistance() + 2);
        _collider.offset = new Vector2(0.5f, -_collider.size.y / 2f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Refrence.player.OnEnterFinish();

            Refrence.finishUI.SetActive(true);

            if (!fromData)
            {
                int[] vals = ShuffleCards();

                selectedCards = new Card[] { Refrence.cardTypes[vals[0]], Refrence.cardTypes[vals[1]], Refrence.cardTypes[vals[2]] };
            }

            Refrence.gen.OnPlayerEnterFinish(this);

            Refrence.finishUI.GetComponent<Finish_UI>().Show(selectedCards, this);

            Player.isAtFinish = true;
            Player.scoreAtLastFinish = Player.score;
        }
    }

    public void SetData(Finish_Data data)
    {
        selectedCards = data.selectedCards;
        fromData = true;
    }

    public Finish_Data ToData()
        => new Finish_Data(selectedCards);

    public void Card_IncreaseSpeed(int cardIndex)
    {
        Refrence.player.speed *= Refrence.cardTypes[cardIndex].value;
        Refrence.finishUI.GetComponent<Finish_UI>().Hide();

        Refrence.player.lastUpgraded = Refrence.cardTypes[cardIndex];
    }

    public void Card_IncreaseRotSpeed(int cardIndex)
    {
        Refrence.player.rotSpeed *= Refrence.cardTypes[cardIndex].value;
        Refrence.finishUI.GetComponent<Finish_UI>().Hide();

        Refrence.player.lastUpgraded = Refrence.cardTypes[cardIndex];
    }

    public void Card_RemoveSegment(int cardIndex)
    {
        Refrence.player.RemoveSegments((int)Refrence.cardTypes[cardIndex].value * -1);
        Refrence.player.SetBackups(Refrence.gen.backup + (int)Refrence.cardTypes[cardIndex].value);
        Refrence.finishUI.GetComponent<Finish_UI>().Hide();

        Refrence.player.lastUpgraded = Refrence.cardTypes[cardIndex];
    }

    public void Card_DecreaseSpawnSpeed(int cardIndex)
    {
        Refrence.gen.spawnSpeed *= Refrence.cardTypes[cardIndex].value;
        Refrence.finishUI.GetComponent<Finish_UI>().Hide();

        Refrence.player.lastUpgraded = Refrence.cardTypes[cardIndex];
    }
}
