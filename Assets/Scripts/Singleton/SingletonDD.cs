using UnityEngine;
using System;
using System.Collections.Generic;

public class SingletonDD : BaseBehaviour
{
    protected static Dictionary<Type, object> s_singletons = new Dictionary<Type, object>();
}

public class SingletonDD<T> : SingletonDD where T : Component
{
    //private static Dictionary<Type, object> s_singletons = new Dictionary<Type, object>();

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
            transform.SetParent(null);
            DontDestroyOnLoad(this.gameObject);
            s_singletons.Add(typeof(T), this);
        }
        else if ((T)s_singletons[typeof(T)] != this)
        {
            Log(typeof(T) + " type already exists!");
            Destroy(this.gameObject);
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
        if ((T)s_singletons[typeof(T)] == this)
        {
            Log(typeof(T) + " Destroyed...");
            s_singletons.Remove(typeof(T));
        }
    }
}