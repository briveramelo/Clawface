using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using MEC;

public class SpawnManager : RoutineRunner
{
    internal static bool spawnersLocked = false;
    
    int currentSpawner = 0;
    int waveNumber = 0;
    float time = 0.0f;

    private float levelTime;

    private bool started = false;
    bool spawnClear = true;
    bool LevelClear = false;

    private Spawner spawner;    

    public List<SpawnerUnit> spawners = new List<SpawnerUnit>();
    public int lastWave = 0;

    void OnDestroy() {        
        if (EventSystem.Instance) {
            EventSystem.Instance.UnRegisterEvent(Strings.Events.WEAPONS_SELECT_FROM_STAGE_OVER, TerminateSpawnerAndEnemies);
            EventSystem.Instance.UnRegisterEvent(Strings.Events.CALL_NEXT_WAVE, CallNextSpawner);
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
        EventSystem.Instance.RegisterEvent(Strings.Events.WEAPONS_SELECT_FROM_STAGE_OVER, TerminateSpawnerAndEnemies);
        EventSystem.Instance.RegisterEvent(Strings.Events.CALL_NEXT_WAVE, CallNextSpawner);
        if (spawners.Count == 0)
        {
            Debug.Log("SpawnManager is Empty");
        }

        if(lastWave > spawners.Count)
        {
            Debug.Assert(false, "Last wave cannot be greater than the number of waves");
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
        if(lastWave > 0 && currentSpawner > lastWave - 1)
        {            
            if(currentSpawner >= spawners.Count)
            {
                currentSpawner = lastWave;
            }
        }

        if(currentSpawner < spawners.Count)
        {
            //wave complete      
//            Debug.Log("wave complete");
            EventSystem.Instance.TriggerEvent(Strings.Events.WAVE_COMPLETE, waveNumber);
            if(waveNumber == 0)
            {
                //SFXManager.Instance.Play(SFXType.AnnounceLevelStart, Vector3.zero);
            }
            waveNumber++;
            time = spawners[currentSpawner].Time;
            spawner = spawners[currentSpawner++].Prefab.GetComponent<Spawner>();
            AnalyticsManager.Instance.IncrementWave();
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
