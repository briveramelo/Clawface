using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServiceWrangler : Singleton<ServiceWrangler> {

    protected ServiceWrangler() { }

    [SerializeField]
    private GameObject sfxManager, objectPool, inputManager, hitstopManager,
        menuManager, analyticsManager, musicManager, scoreManager, respawnPoint,
        eventSystem, achievementManager, platformManager, goreManager,
        saveState, aiManager, enemyStatsManager, leaderBoards, menuMusicManager,
        steamWorkshop, settingsManager;

    private Dictionary<string, PrefabBool> singletonPrefabRegistry;
    private Dictionary<string, PrefabBool> SingletonPrefabRegistry {
        get {
            if (singletonPrefabRegistry==null) {
                singletonPrefabRegistry = new Dictionary<string, PrefabBool>(){
                    { typeof(SFXManager).ToString(),          new PrefabBool(ref sfxManager) },
                    { typeof(ObjectPool).ToString(),            new PrefabBool(ref objectPool) },
                    { typeof(InputManager).ToString(),          new PrefabBool(ref inputManager) },
                    { typeof(HitstopManager).ToString(),        new PrefabBool(ref hitstopManager) },
                    { typeof(MenuManager).ToString(),                new PrefabBool(ref menuManager) },
                    { typeof(AnalyticsManager).ToString(),      new PrefabBool(ref analyticsManager) },
                    { typeof(MusicManager).ToString(),       new PrefabBool(ref musicManager) },
                    { typeof(ScoreManager).ToString(),       new PrefabBool(ref scoreManager) },
                    { typeof(RespawnPoint).ToString(),       new PrefabBool(ref respawnPoint) },
                    { typeof(EventSystem).ToString(),       new PrefabBool(ref eventSystem) },
                    { typeof(AchievementManager).ToString(),       new PrefabBool(ref achievementManager) },
                    { typeof(PlatformManager).ToString(),       new PrefabBool(ref platformManager) },
                    { typeof(GoreManager).ToString(),       new PrefabBool(ref goreManager)},
                    { typeof(SaveState).ToString(),       new PrefabBool(ref saveState) },
                    { typeof(AIManager).ToString(),       new PrefabBool(ref aiManager) },
                    { typeof(EnemyStatsManager).ToString(),       new PrefabBool(ref enemyStatsManager) },
                    { typeof(LeaderBoards).ToString(),       new PrefabBool(ref leaderBoards) },
                    { typeof(MenuMusicManager).ToString(),       new PrefabBool(ref menuMusicManager) },
                    { typeof(SteamWorkshop).ToString(),       new PrefabBool(ref steamWorkshop) },
                    { typeof(SettingsManager).ToString(), new PrefabBool(ref settingsManager) }
                };
            }
            return singletonPrefabRegistry;
        }
    }

    private void OnEnable () {
        foreach (KeyValuePair<string, PrefabBool> singletonRegistered in SingletonPrefabRegistry) {
            if (!singletonRegistered.Value.isRegistered) {                
                GameObject singletonGameObject = Instantiate(singletonRegistered.Value.prefab, gameObject.transform, true) as GameObject;
                singletonGameObject.transform.position = Vector3.zero;
                singletonGameObject.transform.rotation = Quaternion.identity;

                string debugMessage = singletonRegistered.Key + " required Loading. Place this prefab in your scene";
                //Debug.LogFormat("<color=#ffff00>" + debugMessage + "</color>");
            }
        }
        Application.targetFrameRate = 60;
    }


    public void RegisterSingleton<T> (T singleton) where T : MonoBehaviour
    {
        string typeString = typeof(T).ToString();

        if (SingletonPrefabRegistry.ContainsKey(typeString) && !SingletonPrefabRegistry[typeString].isRegistered){
            SingletonPrefabRegistry[typeString].isRegistered = true;
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
