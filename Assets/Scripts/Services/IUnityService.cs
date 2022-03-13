using UnityEngine;

public interface IUnityService
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public void InitializeUnityService();
}