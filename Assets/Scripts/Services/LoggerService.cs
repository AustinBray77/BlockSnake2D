using UnityEngine;

//Class to implement the logging to the debug console
public class DebugLogger : ILoggerService
{
    //Variable to store whether the logger should acutally log
    private bool _logData = true;

    //Method to log a default message to the debug console
    public void Log(object data, bool forceLog = false)
    {
        //Trigger if the data should be logged & log the data to the debug console
        if (_logData || forceLog) Debug.Log(data);
    }

    //Method to log a warning message to the debug console
    public void LogWarning(object data, bool forceLog = false)
    {
        //Trigger if the data should be logged & log the data to the debug console
        if (_logData || forceLog) Debug.LogWarning(data);
    }

    //Method to log an error message to the debug console
    public void LogError(object data, bool forceLog = false)
    {
        //Trigger if the data should be logged & log the data to the debug console
        if (_logData || forceLog) Debug.LogError(data);
    }
}