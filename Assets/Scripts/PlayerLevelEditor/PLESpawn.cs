using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ModMan;

using Turing.VFX;
using System;

public class PLESpawn : PLEItem {

    #region Private Fields
    private int currentSpawnAmount;
    private float spawnHeightOffset = 50.0f;
    private List<GameObject> spawnedEnemies = new List<GameObject>();
    #endregion
    
    #region Public Fields
    public float spawnFrequency = 0.5f;
    public int totalSpawnAmount = 2;
    public SpawnType spawnType;
    [HideInInspector] public int registeredWave = -99;
    [HideInInspector] public bool allEnemiesDead = false;
    #endregion

    #region Unity Lifecycle

    private void OnEnable()
    {
        if(EventSystem.Instance)
        {
            EventSystem.Instance.RegisterEvent(Strings.Events.PLE_TEST_END, SetAmounts);
        }
       
    }

    private void OnDisable()
    {
        if(EventSystem.Instance)
        {
            EventSystem.Instance.UnRegisterEvent(Strings.Events.PLE_TEST_END, SetAmounts);
        }
    }

    private void Start()
    {
        SetAmounts(null);
    }

    #endregion

    #region Public Interface

    public void StartSpawning()
    {
        StartCoroutine(SpawnEnemies());
    }

    #endregion

    #region Private Interface

    private void ReportDeath()
    {
        if (!allEnemiesDead)
        {
            currentSpawnAmount--;

            if (currentSpawnAmount <= 0)
            {
                //report that enemies are all dead to wave system
                allEnemiesDead = true;
                Debug.Log("All enemies for Spawner: " + gameObject.name + " of type : " + spawnType + " are dead.");
                //EventSystem.Instance.TriggerEvent(Strings.Events.REPORT_DEATH, registeredWave, spawnType);
            }
        }
    }

    private IEnumerator SpawnEnemies()
    {
        for(int i = 0; i < totalSpawnAmount; i++)
        {
            GameObject newSpawnEffect = ObjectPool.Instance.GetObject(PoolObjectType.VFXEnemySpawn);
            if(newSpawnEffect)
            {
                newSpawnEffect.GetComponent<VFXOneOff>().Play(spawnFrequency);
                newSpawnEffect.transform.position = transform.position;
            }

            SpawnEnemy();

            yield return new WaitForSeconds(spawnFrequency);
        }
    }

    private void SpawnEnemy()
    {
        Vector3 spawnPos = new Vector3(0, transform.position.y + spawnHeightOffset, 0);
        GameObject newSpawnObj = ObjectPool.Instance.GetObject(spawnType.ToPoolObject());
        spawnPos = transform.TransformPoint(spawnPos);
        
        if(newSpawnObj)
        {
            newSpawnObj.transform.position = spawnPos;
            ISpawnable spawnable = newSpawnObj.GetComponentInChildren<ISpawnable>();
            if(!spawnable.HasWillBeenWritten())
            {
                spawnable.RegisterDeathEvent(ReportDeath);
            }

            //TODO: How to register enemies to let us know that they're dead
            EnemyBase enemyBase = newSpawnObj.GetComponent<EnemyBase>();

            if(enemyBase)
            {
                enemyBase.SpawnWithRagdoll(spawnPos);
            }

            currentSpawnAmount++;

            EventSystem.Instance.TriggerEvent(Strings.Events.ENEMY_SPAWNED, newSpawnObj);

        }
        else
        {
            Debug.LogFormat("<color=#ffff00>" + "NOT ENOUGH SPAWN-OBJECT" + "</color>");
        }
    }
    private void SetAmounts(object[] parameters)
    {
        currentSpawnAmount = totalSpawnAmount;
    }


    #endregion



}
