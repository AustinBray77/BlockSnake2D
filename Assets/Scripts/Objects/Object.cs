using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Inherited class to control all objects which move across the screen
//Requires a 2D collider for collision, and a sprite renderer for renderering the objects sprite
[RequireComponent(typeof(SpriteRenderer), typeof(Collider2D))]
public abstract class Object : BaseBehaviour
{
    public static float speed = 7.5f;
    public static int objectLayer = 3;

    //Stores the max number objects of this type which can occur on screen
    public int maxOnScreen;

    //Bool to check if it is interactable
    internal bool interactable;

    //Method called when the object is instantiated
    internal void Awake()
    {
        //Assigns the gameobjects tag and sorting layer to the appropriate values
        gameObject.tag = "Object";
        GetComponent<SpriteRenderer>().sortingOrder = objectLayer;
        interactable = true;

        //Allows for inherited classes to still call on instatiation
        //ObjAwake();
    }

    /*
    //Method called on each frame
    private void Update()
    {
        //If the player isn't dead or at finish the object moves to the left at the assigned speed variable
        if (!Player.isDead && !Player.isAtFinish)
        {
            transform.position += new Vector3(-1 * speed * Time.deltaTime * Generator.GetRelativeSpeed(), 0);
        }

        //Allows for inherited classes to still call on each frame
        ObjUpdate();
    }*/

    //Method to destroy the object
    public void ObjectDestroy()
    {
        interactable = false;
        gameObject.LeanScale(new Vector3(0, 0, 0), 0.2f);
    }

    //Inhertied functions
    internal virtual void ObjAwake() { }
    internal virtual void ObjUpdate() { }
}
