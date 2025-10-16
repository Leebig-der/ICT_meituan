
using UnityEngine;
public static class RuntimeBootstrap
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Init()
    {
        if (Object.FindObjectOfType<GameManager>() == null)
        {
            var go = new GameObject("Game");
            go.AddComponent<GameManager>();
        }
    }
}
