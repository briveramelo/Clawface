using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;

public static class InstantiateObjectHelper
{
    public static GameObject InstantiateObject (GameObject prefab)
    {
        GameObject instance=null;

        #if UNITY_EDITOR

        instance = PrefabUtility.InstantiatePrefab (prefab) as GameObject;

        #endif

        if (UnityEngine.Application.isPlaying)
        {
            instance = MonoBehaviour.Instantiate (prefab) as GameObject;
        }

        return instance;
    }
}
