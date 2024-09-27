using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;
    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                string typename = typeof(T).Name;
                Debug.LogWarning($"{typename} 가 강제로 생성됨");
                GameObject emptySingleton = new GameObject($"Generated {typename}");
                instance = emptySingleton.AddComponent<T>();
                DontDestroyOnLoad(emptySingleton);
            }

            return instance;
        }
    }

    protected void AwakeSingleton(T self)
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = self;
        DontDestroyOnLoad(gameObject);
    }
}
