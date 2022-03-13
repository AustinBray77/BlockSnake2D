using UnityEngine;
using UnityEditor;

public class BaseBehaviour : MonoBehaviour, ILoggerService
{
    public bool shouldLogData;

    public void Log(object data)
    {
        if (shouldLogData) Debug.Log(data);
    }

    public void LogWarning(object data)
    {
        if (shouldLogData) Debug.LogWarning(data);
    }

    public void LogError(object data)
    {
        if (shouldLogData) Debug.LogError(data);
    }
}