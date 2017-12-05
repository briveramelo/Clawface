﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SpawnManager : MonoBehaviour
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

    void OnEnable()
    {
        if(EventSystem.Instance)
            EventSystem.Instance.RegisterEvent(Strings.Events.CALL_NEXTWAVEENEMIES, CallNextSpawner);
    }
    void OnDisable() {
        if (EventSystem.Instance) {
            EventSystem.Instance.UnRegisterEvent(Strings.Events.CALL_NEXTWAVEENEMIES, CallNextSpawner);
        }
    }

    public void Trigger()
    {
        OnEnable();
        Start();
    }

    void Start()
    {
        if (spawners.Count == 0)
        {
            Debug.Log("SpawnManager is Empty");
        }
    }

    private void Update()
    {
        if (!started && !spawnersLocked)
        {
            StartCoroutine(WaitAndSpawn(time));
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
        StartCoroutine(WaitAndSpawn(time));
    } 

    IEnumerator WaitAndSpawn(float waitTime)
    {
        if(currentSpawner < spawners.Count)
        {
            time = spawners[currentSpawner].Time;
            spawner = spawners[currentSpawner++].Prefab.GetComponent<Spawner>();
            AnalyticsManager.Instance.SetCurrentWave(currentSpawner);
            yield return new WaitForSeconds(waitTime);
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
