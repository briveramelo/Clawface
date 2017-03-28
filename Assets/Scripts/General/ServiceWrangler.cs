using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServiceWrangler : Singleton<ServiceWrangler> {

    protected ServiceWrangler() { }
    
    [SerializeField] private GameObject audioManager, bulletPool, healthBar, modUIManager, vfxManager, inputManager, hitstopManager, playerTeleporter, pauser;
    private static Dictionary<string, PrefabBool> singletonPrefabRegistry;

    protected override void Awake() {
        singletonPrefabRegistry = new Dictionary<string, PrefabBool>()
        {
            { typeof(AudioManager).ToString(),          new PrefabBool(ref audioManager) },
            { typeof(ObjectPool).ToString(),            new PrefabBool(ref bulletPool) },
            { typeof(HealthBar).ToString(),             new PrefabBool(ref healthBar) },
            { typeof(ModUIManager).ToString(),          new PrefabBool(ref modUIManager) },
            { typeof(VFXManager).ToString(),            new PrefabBool(ref vfxManager) },
            { typeof(InputManager).ToString(),          new PrefabBool(ref inputManager) },
            { typeof(HitstopManager).ToString(),        new PrefabBool(ref hitstopManager) },
            { typeof(DEBUG_PlayerTeleporter).ToString(),new PrefabBool(ref playerTeleporter) },
            { typeof(Pauser).ToString(),new PrefabBool(ref pauser) }
        };
        base.Awake();
    }

    private void Start() {
        foreach (KeyValuePair<string, PrefabBool> singletonRegistered in singletonPrefabRegistry) {
            if (!singletonRegistered.Value.isRegistered) {
                GameObject singletonGameObject = Instantiate(singletonRegistered.Value.prefab, null, true) as GameObject;
                singletonGameObject.transform.position = Vector3.zero;
                singletonGameObject.transform.rotation = Quaternion.identity;

                string debugMessage = singletonRegistered.Key + " required Loading. Place this prefab in your scene";
                Debug.LogFormat("<color=#ffff00>" + debugMessage + "</color>");
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
        else if (typeString != (typeof(ServiceWrangler)).ToString()) {
            string debugMessage = typeString + " attempting duplicate or unprepared service registry. Add this singleton to the singletonPrefabRegistry";
            Debug.LogFormat("<color=#ffff00>" + debugMessage + "</color>");
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
