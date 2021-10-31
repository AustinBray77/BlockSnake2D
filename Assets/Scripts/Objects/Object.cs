using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Inherited class to control all objects which move across the screen
//Requires a 2D collider for collision, and a sprite renderer for renderering the objects sprite
[RequireComponent(typeof(Collider2D), typeof(SpriteRenderer))]
public abstract class Object : MonoBehaviour
{
    public static float speed = 5;
    public static int objectLayer = 3;

    //Stores the max number objects of this type which can occur on screen
    public int maxOnScreen;

    //Method called when the object is instantiated
    private void Awake()
    {
        //Assigns the gameobjects tag and sorting layer to the appropriate values
        gameObject.tag = "Object";
        GetComponent<SpriteRenderer>().sortingOrder = objectLayer;

        //Allows for inherited classes to still call on instatiation
        ObjAwake();
    }

    //Method called on each frame
    private void FixedUpdate()
    {
        //If the player isn't dead or at finish the object moves to the left at the assigned speed variable
        if (!Player.isDead && !Player.isAtFinish)
        {
            transform.position += -transform.right * speed * Time.deltaTime;
        }

        //Allows for inherited classes to still call on each frame
        ObjFixedUpdate();
    }

    //Inhertied functions
    internal virtual void ObjAwake() { }
    internal virtual void ObjFixedUpdate() { }
}
