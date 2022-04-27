using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Class to control the finish line object
public class FinishObject : Object
{
    //Stores if the object is intiated from a data object
    private bool fromData = false;

    //Stores cards that are going to be used 
    private Card[] selectedCards;

    //Prefabs for the tiles that make up the finish
    [SerializeField] private GameObject tilePrefab;

    //Stores the index from the generator at which the finish was spawned
    [HideInInspector] public int spawnIndex;

    //Stores the collider
    private BoxCollider2D _collider;

    //Method called on object instantiation
    private new void Awake()
    {
        base.Awake();

        //Assigns the collider
        _collider = GetComponent<BoxCollider2D>();

        //Generates the tiles
        Generate(true);
    }

    //Method to shuffle the cards
    private int[] ShuffleCards()
    {
        //Creates a new int array and assigns each value to its index
        int[] vals = new int[Reference.cardTypes.Length];

        //Loops through each value
        for (int i = 0; i < vals.Length; i++)
        {
            //Sets it to its index
            vals[i] = i;
        }

        //Randomly swaps each index with a different index
        for (int i = 0; i < Reference.cardTypes.Length; i++)
        {
            //Get a random index
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
        //Gets the boundes distance from generator
        float boundsDistance = Generator.GetBoundsDistance();

        //Gets the ceiling value of this distance
        float ceilBoundsDistance = Mathf.Ceil(boundsDistance);

        //Triggers if this is the first generation
        if (baseGeneration)
        {
            //Loops for -(Mathf.Ceil(Generator.GetBoundsDistance()) + 2) tiles
            for (int i = 0; i >= -(ceilBoundsDistance + 2); i--)
            {
                //Loops for 2 tiles
                for (int j = 0; j < 2; j++)
                {
                    //Instantiates the tile object from the prefab
                    GameObject tile = Instantiate(tilePrefab, transform);
                    //Assigns the correct position to the tile object
                    tile.transform.localPosition = new Vector3(j, i);
                }
            }
        }
        //Else add to current tiles
        else
        {
            //Loop for 3*amount tiles
            for (int i = 0; i < 3 * amount; i++)
            {
                //Loop for 2 tiles
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
            Reference.player.OnEnterFinish();

            //Randomizes the cards if the finish was not loaded after respawn
            if (!fromData)
            {
                //Gets shuffeled values
                int[] vals = ShuffleCards();

                //Saves selected cards from first 3 results from the shuffled values
                selectedCards = new Card[] { Reference.cardTypes[vals[0]], Reference.cardTypes[vals[1]], Reference.cardTypes[vals[2]] };
            }

            //Tells the generator object the player has entered the finish
            Reference.gen.OnPlayerEnterFinish(this);

            //Shows the cards
            Finish_UI.ShowInstance();
            Finish_UI.Instance.SetCardData(selectedCards);
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
