using UnityEngine;
using System;
using System.Collections.Generic;

//Container for fields and properties all do not destroy singletons should have access too
public class SingletonDD : BaseBehaviour
{
    //Stores all of the instantiated singletons
    protected static Dictionary<Type, object> s_singletons = new Dictionary<Type, object>();
}

//Do Not Destroy Singleton class for instantiation by objects
public class SingletonDD<T> : SingletonDD where T : Component
{
    //Get-only Property for the singleton instance
    public static T Instance
    {
        get
        {
            //Returns the instance from the singleton dictionary
            return (T)s_singletons[typeof(T)];
        }
    }

    //Method called when the object is instantiated
    protected void Awake()
    {
        //Triggers if there is no current instance of the singleton
        if (!s_singletons.ContainsKey(typeof(T)))
        {
            //Sets the object to the first layer of the heirachy
            transform.SetParent(null);

            //Sets the object to not be destroyed on scene loads
            DontDestroyOnLoad(this.gameObject);

            //Adds the object to the singleton dictionary
            s_singletons.Add(typeof(T), this);
        }
        //Else triggers if the current instantiation is not this object
        else if (Instance != this)
        {
            //Destroy the object
            Destroy(this.gameObject);
        }
    }

    //Method called when the object is destroyed
    private void OnDestroy()
    {
        //Triggers if the object dictionary does not contain an instance of this type
        if (!s_singletons.ContainsKey(typeof(T)))
        {
            //Return as there is nothing to do
            return;
        }

        //Triggers if the current instance is equal to the one saved
        if (Instance == this)
        {
            //Remove this instance from the dictionary
            s_singletons.Remove(typeof(T));
        }
    }
}