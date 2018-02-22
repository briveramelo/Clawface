using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Turing.VFX;

public class PLESpawn : PLEItem {

    #region Private Fields
    private int currentSpawnAmount;
    private float spawnHeightOffset = 50.0f;
    #endregion
    
    #region Public Fields
    public float spawnFrequency = 0.5f;
    public int totalSpawnAmount = 2;
    public PoolObjectType spawnType;
    public int spawnCount;

    #endregion

    #region Unity Lifecycle

    private void Awake()
    {
        currentSpawnAmount = totalSpawnAmount;
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
        currentSpawnAmount--;

        if(currentSpawnAmount <= 0)
        {
            //report that enemies are all dead to wave system
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
        GameObject newSpawnObj = ObjectPool.Instance.GetObject(spawnType);
        
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

    #endregion



}
