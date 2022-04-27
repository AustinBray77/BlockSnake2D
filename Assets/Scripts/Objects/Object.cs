using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Inherited class to control all objects which move across the screen
//Requires a 2D collider for collision, and a sprite renderer for renderering the objects sprite
[RequireComponent(typeof(SpriteRenderer), typeof(Collider2D))]
public abstract class Object : BaseBehaviour
{
    //Static to store speed and object layer
    public static float speed = 7.5f;
    public static int objectLayer = 3;

    //Stores the max number objects of this type which can occur on screen
    public int maxOnScreen;

    //Stores if the object is interactable
    protected bool interactable;

    //Method called when the object is instantiated
    protected void Awake()
    {
        //Assigns the gameobject tag
        gameObject.tag = "Object";

        //Sets the sprite layer
        GetComponent<SpriteRenderer>().sortingOrder = objectLayer;

        //Flags to be interactable
        interactable = true;

        //Allows for inherited classes to still call on instatiation
        //ObjAwake();
    }

    //Method to destroy the object
    public void ObjectDestroy()
    {
        //Sets to be uninteractable
        interactable = false;

        //Shrinks the object
        gameObject.LeanScale(new Vector3(0, 0, 0), 0.2f);
    }
}
