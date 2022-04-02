using UnityEngine;
using System;
using System.Collections.Generic;

public class Singleton : BaseBehaviour
{
    protected static Dictionary<Type, object> s_singletons = new Dictionary<Type, object>();
}

public class Singleton<T> : Singleton where T : Component
{
    public static T Instance
    {
        get
        {
            return (T)s_singletons[typeof(T)];
        }
    }

    protected void Awake()
    {
        if (!s_singletons.ContainsKey(typeof(T)))
        {
            s_singletons.Add(typeof(T), this);
        }
        else if ((T)s_singletons[typeof(T)] != this)
        {
            Destroy(this);
        }
    }

    /*
    private void Update()
    {
        string output = "";

        foreach (var key in s_singletons.Keys)
        {
            output += key.ToString() + " ";
        }

        Log(output, true);
    }*/

    private void OnDestroy()
    {
        if (!s_singletons.ContainsKey(typeof(T)))
            return;

        if ((T)s_singletons[typeof(T)] == this)
        {
            s_singletons.Remove(typeof(T));
        }
    }
}