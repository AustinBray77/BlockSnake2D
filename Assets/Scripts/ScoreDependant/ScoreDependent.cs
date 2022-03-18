using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ScoreDependent : BaseBehaviour
{
    private static List<ScoreDependent> s_objects = new List<ScoreDependent>();

    protected void Awake()
    {
        s_objects.Add(this);
    }

    private void OnDestroy()
    {
        s_objects.Remove(this);
    }

    public static void OnScoreChange(int amount, bool useAnimation)
    {
        foreach (ScoreDependent obj in s_objects)
        {
            obj.OnSegmentChange(amount, useAnimation);
        }
    }

    public abstract void OnSegmentChange(int amount, bool useAnimation);
}
