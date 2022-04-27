using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Class to control the objects dependant on scores and change when the score changes
public abstract class ScoreDependent : BaseBehaviour
{
    //Static list to store all children
    private static List<ScoreDependent> s_objects = new List<ScoreDependent>();

    //Method called on instantiation
    protected void Awake()
    {
        //Adds the object to the children list
        s_objects.Add(this);
    }

    //Method called on object destry
    private void OnDestroy()
    {
        //Removes the object from the children list
        s_objects.Remove(this);
    }

    //Method called to update all children when score changes by the amount of change
    public static void OnScoreChange(int amount, bool useAnimation)
    {
        //Loops for each child object
        foreach (ScoreDependent obj in s_objects)
        {
            //Tells the child that the segment/score amount has changed
            obj.OnSegmentChange(amount, useAnimation);
        }
    }

    //Overidable method for when the segment count changes
    public abstract void OnSegmentChange(int amount, bool useAnimation);
}
