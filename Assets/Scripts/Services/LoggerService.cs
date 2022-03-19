using UnityEngine;

public class LoggerService : ILoggerService
{
    private bool _logData = true;

    public void Log(object data, bool forceLog = false)
    {
        if (_logData || forceLog) Debug.Log(data);
    }

    public void LogWarning(object data, bool forceLog = false)
    {
        if (_logData) Debug.LogWarning(data);
    }

    public void LogError(object data, bool forceLog = false)
    {
        if (_logData) Debug.LogError(data);
    }
}