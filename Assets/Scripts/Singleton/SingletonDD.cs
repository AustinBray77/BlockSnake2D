using UnityEngine;
using System;
using System.Collections.Generic;

public class SingletonDD<T> : BaseBehaviour where T : Component
{
    private static Dictionary<Type, object> s_singletons = new Dictionary<Type, object>();

    public static T Instance
    {
        get
        {
            return (T)s_singletons[typeof(T)];
        }
    }

    private void Awake()
    {
        if (!s_singletons.ContainsKey(typeof(T)))
        {
            transform.SetParent(null);
            s_singletons.Add(typeof(T), this);
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    private void OnDestroy()
    {
        if (s_singletons.ContainsKey(typeof(T)))
        {
            s_singletons.Remove(typeof(T));
        }
    }
}