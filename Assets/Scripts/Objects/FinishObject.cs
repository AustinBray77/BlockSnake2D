using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Class to control the finish line object
public class FinishObject : Object
{
    //Instance variables
    private BoxCollider2D _collider;
    private bool fromData = false;
    private Card[] selectedCards;

    [SerializeField] private GameObject tilePrefab;
    
    [HideInInspector] public int spawnIndex;

    //Overrides the ObjAwake function, called when the object spawns
    internal override void ObjAwake()
    {
        //Assigns the collider and generates the finish object
        _collider = GetComponent<BoxCollider2D>();
        Generate();
    }

    //Method to shuffle the cards
    private int[] ShuffleCards()
    {
        //Creates a new int array and assigns each value to its index
        int[] vals = new int[Refrence.cardTypes.Length];

        for(int i = 0; i < vals.Length; i++)
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
    public void Generate()
    {
        //Gets all child objects of the gameobject
        List<Transform> children = transform.GetComponentsInChildren<Transform>().ToList();
        
        //If there are child objects, they are destroyed
        while(children.Count > 0)
        {
            if(children[0] != transform)
                Destroy(children[0].gameObject);

            children.RemoveAt(0);
        }

        //Generates (2, -(Mathf.Ceil(Generator.GetBoundsDistance()) + 2)) tiles
        for (int i = 0; i >= -(Mathf.Ceil(Generator.GetBoundsDistance()) + 2); i--)
        {
            for(int j = 0; j < 2; j++)
            {
                //Instantiates the tile object from the prefab
                GameObject tile = Instantiate(tilePrefab, transform);
                //Assigns the correct position to the tile object
                tile.transform.localPosition = new Vector3(j, i);
            }
        }

        //Sets position to the middle of the screen (vertically), and sets the bounds of the collider
        transform.position = new Vector3(transform.position.x, Generator.GetBoundsDistance() / 2f + 1);
        _collider.size = new Vector3(2, Generator.GetBoundsDistance() + 2);
        _collider.offset = new Vector2(0.5f, -_collider.size.y / 2f);
    }

    //Method called when an another object collides with the object
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Triggers if the object collided with the player
        if (collision.gameObject.tag == "Player")
        {
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

    //Following functions are used for upgrades and are called when the user selects the upgrade.

    //Increases the players speed
    public void Card_IncreaseSpeed(int cardIndex)
    {
        //Increases the players speed by the current determind amount
        Refrence.player.speed *= Refrence.cardTypes[cardIndex].value;

        //Hides the Finish UI and sets the last upgraded card to the selected card
        Refrence.finishUI.GetComponent<Finish_UI>().Hide();
        Refrence.player.lastUpgraded = Refrence.cardTypes[cardIndex];
    }

    //Increases the players rotation speed
    public void Card_IncreaseRotSpeed(int cardIndex)
    {
        //Increases the players rotation speed by the current determind amount
        Refrence.player.rotSpeed *= Refrence.cardTypes[cardIndex].value;

        //Hides the Finish UI and sets the last upgraded card to the selected card
        Refrence.finishUI.GetComponent<Finish_UI>().Hide();
        Refrence.player.lastUpgraded = Refrence.cardTypes[cardIndex];
    }

    //Removes a segment from the player
    public void Card_RemoveSegment(int cardIndex)
    {
        //Removes the determind amount of segments
        Refrence.player.RemoveSegments((int)Refrence.cardTypes[cardIndex].value * -1);

        //Sets an offset so that when the player gains score the screen size does not change
        Refrence.player.SetBackups(Refrence.gen.backup + (int)Refrence.cardTypes[cardIndex].value);

        //Hides the Finish UI and sets the last upgraded card to the selected card
        Refrence.finishUI.GetComponent<Finish_UI>().Hide();
        Refrence.player.lastUpgraded = Refrence.cardTypes[cardIndex];
    }

    //Decreases the spawn speed of the objects
    public void Card_DecreaseSpawnSpeed(int cardIndex)
    {
        //Decreases the spawn speed by the current determind amount
        Refrence.gen.spawnSpeed *= Refrence.cardTypes[cardIndex].value;

        //Hides the Finish UI and sets the last upgraded card to the selected card
        Refrence.finishUI.GetComponent<Finish_UI>().Hide();
        Refrence.player.lastUpgraded = Refrence.cardTypes[cardIndex];
    }

    //Adds a shield to the player
    public void Card_AddShield(int cardIndex)
    {
        //Adds the shields to the player
        Refrence.player.AddShields((int)Refrence.cardTypes[cardIndex].value);

        //Hides the Finish UI and sets the last upgraded card to the selected card
        Refrence.finishUI.GetComponent<Finish_UI>().Hide();
        Refrence.player.lastUpgraded = Refrence.cardTypes[cardIndex];
    }
}
