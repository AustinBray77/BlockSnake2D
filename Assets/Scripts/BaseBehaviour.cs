using UnityEngine;
using UnityEditor;

//Base class for all object attached scripts to derive from
public class BaseBehaviour : MonoBehaviour, ILoggerService
{
    //Stores whether the data should be logged
    public bool LogData;

    //Method to log a default message to the debug console
    public void Log(object data, bool forceLog = false)
    {
        //Trigger if the data should be logged & log the data to the debug console
        if (LogData || forceLog) Debug.Log(data);
    }

    //Method to log a warning message to the debug console
    public void LogWarning(object data, bool forceLog = false)
    {
        //Trigger if the data should be logged & log the data to the debug console
        if (LogData || forceLog) Debug.LogWarning(data);
    }

    //Method to log an error message to the debug console
    public void LogError(object data, bool forceLog = false)
    {
        //Trigger if the data should be logged & log the data to the debug console
        if (LogData || forceLog) Debug.LogError(data);
    }
}