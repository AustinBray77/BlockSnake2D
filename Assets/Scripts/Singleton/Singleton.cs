using UnityEngine;

public class Singleton<T> : BaseBehaviour where T : Component
{
    public static T Instance { get; private set; }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void InitializeType()
    {
        Instance = new GameObject($"#{nameof(T)}").AddComponent<T>();
        DontDestroyOnLoad(Instance);
    }
}