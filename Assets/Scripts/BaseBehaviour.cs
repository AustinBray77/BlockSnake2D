using UnityEngine;
using UnityEditor;

public class BaseBehaviour : MonoBehaviour, ILoggerService
{
    public bool LogData;

    public void Log(object data, bool forceLog = false)
    {
        if (LogData || forceLog) Debug.Log(data);
    }

    public void LogWarning(object data, bool forceLog = false)
    {
        if (LogData || forceLog) Debug.LogWarning(data);
    }

    public void LogError(object data, bool forceLog = false)
    {
        if (LogData || forceLog) Debug.LogError(data);
    }
}