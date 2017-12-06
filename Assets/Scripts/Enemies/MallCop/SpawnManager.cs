using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using MovementEffects;

public class SpawnManager : RoutineRunner
{
    internal static bool spawnersLocked = false;
    
    int currentSpawner = 0;
    float time = 0.0f;

    private float levelTime;

    private bool started = false;
    bool spawnClear = true;
    bool LevelClear = false;

    private Spawner spawner;

    public List<SpawnerUnit> spawners = new List<SpawnerUnit>();    
    
    void OnDestroy() {        
        if (EventSystem.Instance) {
            EventSystem.Instance.UnRegisterEvent(Strings.Events.WEAPONSSELECT_FROM_STAGEOVER, TerminateSpawnerAndEnemies);
            EventSystem.Instance.UnRegisterEvent(Strings.Events.CALL_NEXTWAVEENEMIES, CallNextSpawner);
        }
    }

    void TerminateSpawnerAndEnemies(params object[] items) {
        Timing.KillCoroutines(coroutineName);
        FindObjectsOfType<EnemyBase>().ToList().ForEach(enemy => { enemy.OnDeath(); });
        gameObject.SetActive(false);
        gameObject.SetActive(true);
    }

    public void Trigger()
    {        
        Start();
    }

    void Start()
    {        
        EventSystem.Instance.RegisterEvent(Strings.Events.WEAPONSSELECT_FROM_STAGEOVER, TerminateSpawnerAndEnemies);
        EventSystem.Instance.RegisterEvent(Strings.Events.CALL_NEXTWAVEENEMIES, CallNextSpawner);
        if (spawners.Count == 0)
        {
            Debug.Log("SpawnManager is Empty");
        }
    }

    private void Update()
    {
        if (!started && !spawnersLocked)
        {
            Timing.RunCoroutine(WaitAndSpawn(time), coroutineName);
            started = true;
        }

        levelTime += Time.deltaTime;

        if(currentSpawner == spawners.Count && LevelClear == true)
        {
            LevelClear = false;
            ScoreManager.Instance.UpdateHighScore(SceneManager.GetActiveScene().name, ScoreManager.Instance.GetScore());
            EventSystem.Instance.TriggerEvent(Strings.Events.LEVEL_COMPLETED, 
                SceneManager.GetActiveScene().name, ScoreManager.Instance.GetScore(), ModManager.leftArmOnLoad.ToString(), ModManager.rightArmOnLoad.ToString() );
        }
    }

    public void CallNextSpawner(params object[] parameters)
    {
        Timing.RunCoroutine(WaitAndSpawn(time), coroutineName);
    } 

    IEnumerator<float> WaitAndSpawn(float waitTime)
    {
        if(currentSpawner < spawners.Count)
        {
            time = spawners[currentSpawner].Time;
            spawner = spawners[currentSpawner++].Prefab.GetComponent<Spawner>();
            AnalyticsManager.Instance.SetCurrentWave(currentSpawner);
            yield return Timing.WaitForSeconds(waitTime);
            spawner.Activate();
        }
        else
        {
            LevelClear = true;
            AnalyticsManager.Instance.SetCurrentLevelTime(levelTime);
        }
    }
}

[System.Serializable]
public class SpawnerUnit
{
    public GameObject Prefab;
    public float Time;
}
