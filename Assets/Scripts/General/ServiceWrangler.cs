﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServiceWrangler : Singleton<ServiceWrangler> {

    protected ServiceWrangler() { }

    [SerializeField] private GameObject audioManager, bulletPool, healthBar, modUIManager;
    private static Dictionary<string, PrefabBool> singletonPrefabRegistry;

    protected override void Awake() {
        singletonPrefabRegistry = new Dictionary<string, PrefabBool>()
        {
            { typeof(AudioManager).ToString(),          new PrefabBool(ref audioManager) },
            { typeof(ObjectPool).ToString(),            new PrefabBool(ref bulletPool) },
            { typeof(HealthBar).ToString(),             new PrefabBool(ref healthBar) },
            { typeof(ModUIManager).ToString(),          new PrefabBool(ref modUIManager) }
        };
        base.Awake();
    }

    private void Start() {
        foreach (KeyValuePair<string, PrefabBool> singletonRegistered in singletonPrefabRegistry) {
            if (!singletonRegistered.Value.isRegistered) {
                GameObject singletonGameObject = Instantiate(singletonRegistered.Value.prefab, null, true) as GameObject;
                singletonGameObject.transform.position = Vector3.zero;
                singletonGameObject.transform.rotation = Quaternion.identity;
                Debug.LogWarning(singletonRegistered.Key + " required Loading");
            }
        }
    }


    public void RegisterSingleton<T> (T singleton) where T : MonoBehaviour
    {
        string typeString = typeof(T).ToString();

        if (singletonPrefabRegistry.ContainsKey(typeString) && !singletonPrefabRegistry[typeString].isRegistered)
        {
            singletonPrefabRegistry[typeString].isRegistered = true;
        }
        else {
            Debug.LogWarning(typeString + " attempting duplicate or unprepared service registry");
        }
    }

    class PrefabBool {
        public GameObject prefab;
        public bool isRegistered;
        public PrefabBool(ref GameObject prefab) {
            this.prefab = prefab;
        }
    }
}
