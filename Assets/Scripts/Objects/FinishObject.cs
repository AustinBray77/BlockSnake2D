using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Class to control the finish line object
public class FinishObject : Object
{
    //Instance variables
    private bool fromData = false;
    private Card[] selectedCards;

    [SerializeField] private GameObject tilePrefab;

    [HideInInspector] public int spawnIndex;

    private BoxCollider2D _collider;

    //Overrides the ObjAwake function, called when the object spawns
    internal override void ObjAwake()
    {
        _collider = GetComponent<BoxCollider2D>();
        Generate(true);
    }

    //Method to shuffle the cards
    private int[] ShuffleCards()
    {
        //Creates a new int array and assigns each value to its index
        int[] vals = new int[Refrence.cardTypes.Length];

        for (int i = 0; i < vals.Length; i++)
        {
            vals[i] = i;
        }

        //Randomly swaps each index with a different index
        for (int i = 0; i < Refrence.cardTypes.Length; i++)
        {
            int index = Random.Range(0, vals.Length);
            //Evil bit hack to swap variables
            vals[i] = vals[i] ^ vals[index] ^ (vals[index] = vals[i]);
        }

        //Returns the shuffled values
        return vals;
    }

    //Method to generate the finish object
    public void Generate(bool baseGeneration, int amount = 0)
    {
        float boundsDistance = Generator.GetBoundsDistance();
        float ceilBoundsDistance = Mathf.Ceil(boundsDistance);

        if (baseGeneration)
        {
            //Generates (2, -(Mathf.Ceil(Generator.GetBoundsDistance()) + 2)) tiles
            for (int i = 0; i >= -(ceilBoundsDistance + 2); i--)
            {
                for (int j = 0; j < 2; j++)
                {
                    //Instantiates the tile object from the prefab
                    GameObject tile = Instantiate(tilePrefab, transform);
                    //Assigns the correct position to the tile object
                    tile.transform.localPosition = new Vector3(j, i);
                }
            }
        }
        else
        {
            for (int i = 0; i < 3 * amount; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    //Instantiates the tile object from the prefab
                    GameObject tile = Instantiate(tilePrefab, transform);
                    //Assigns the correct position to the tile object
                    tile.transform.localPosition = new Vector3(j, i - ceilBoundsDistance - 2);
                }
            }
        }

        //Sets position to the middle of the screen (vertically), and sets the bounds of the collider
        transform.position = new Vector3(transform.position.x, boundsDistance / 2f + 1);
        _collider.size = new Vector3(2, boundsDistance + 2);
        _collider.offset = new Vector2(0.5f, -_collider.size.y / 2f);
    }

    //Method called when an another object collides with the object
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Triggers if the object collided with the player
        if (collision.gameObject.tag == "Player")
        {
            //Returns if the player already hit this finish
            if (Player.lastFinishHit == gameObject)
            {
                return;
            }

            //Sets that the player hit htis finish last
            Player.lastFinishHit = gameObject;

            //Tells the player object it has entered the finish
            Refrence.player.OnEnterFinish();

            //Randomizes the cards if the finish was not loaded after respawn
            if (!fromData)
            {
                int[] vals = ShuffleCards();

                selectedCards = new Card[] { Refrence.cardTypes[vals[0]], Refrence.cardTypes[vals[1]], Refrence.cardTypes[vals[2]] };
            }

            //Tells the generator object the player has entered the finish
            Refrence.gen.OnPlayerEnterFinish(this);

            //Shows the cards
            Refrence.finishUI.Show();
            Refrence.finishUI.SetCardData(selectedCards);
        }
    }

    //Sets the finish data to data from a previously encontered finish
    //Used for respawning
    public void SetData(Finish_Data data)
    {
        //Assigns selected cards and from data
        selectedCards = data.selectedCards;
        fromData = true;
    }

    //Converts FinishObject to Finish_Data
    public Finish_Data ToData()
        => new Finish_Data(selectedCards);
}
