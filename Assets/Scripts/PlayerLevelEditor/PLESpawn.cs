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
    private Renderer rend;
    private Vector3 actualSpawnPos;
    private Action onAllEnemiesDead;
    #endregion
    
    #region Public Fields
    [HideInInspector] public int registeredWave = -99;
    [HideInInspector] public bool allEnemiesDead = false;
    #endregion

    #region Serialized Unity Fields
    public float spawnFrequency = 0.5f;
    public int totalSpawnAmount = 1;
    public SpawnType spawnType;
    #endregion

    #region Unity Lifecycle

    private void Awake()
    {
        rend = GetComponent<Renderer>();
    }

    private void OnEnable()
    {
        EventSystem.Instance.RegisterEvent(Strings.Events.PLE_TEST_END, Reset);               
    }

    private void OnDisable()
    {
        if(EventSystem.Instance)
        {
            EventSystem.Instance.UnRegisterEvent(Strings.Events.PLE_TEST_END, Reset);
        }
    }

    private void Start()
    {
        Reset();
        actualSpawnPos = new Vector3(0, transform.position.y + spawnHeightOffset, 0);
        actualSpawnPos = transform.TransformPoint(actualSpawnPos);
    }

    #endregion

    #region Public Interface
    public void SetOnAllEnemiesDead(Action onAllEnemiesDead) {
        this.onAllEnemiesDead = onAllEnemiesDead;
    }

    public void StartSpawning()
    {
        if (spawnType != SpawnType.Keira) {
            StartCoroutine(SpawnEnemies());
        }
        else {
            gameObject.AddComponent<PlayerSpawner>();
        }
    }

    public SpawnData GetSpawnData()
    {
        return new SpawnData((int)spawnType, totalSpawnAmount, actualSpawnPos);
    }

    #endregion

    #region Private Interface

    private void ReportDeath()
    {
        if (!allEnemiesDead)
        {
            OnEnemyDeath();
        }
    }

    private IEnumerator SpawnEnemies()
    {
        allEnemiesDead = false;
        rend.enabled = false;
        currentSpawnAmount = totalSpawnAmount;
        for (int i = 0; i < totalSpawnAmount; i++)
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
        GameObject newSpawnObj = ObjectPool.Instance.GetObject(spawnType.ToPoolObject());
                
        if(newSpawnObj)
        {
            newSpawnObj.transform.position = actualSpawnPos;
            ISpawnable spawnable = newSpawnObj.GetComponentInChildren<ISpawnable>();
            if(!spawnable.HasWillBeenWritten())
            {
                spawnable.RegisterDeathEvent(ReportDeath);
            }
            
            EnemyBase enemyBase = newSpawnObj.GetComponent<EnemyBase>();

            if(enemyBase)
            {
                enemyBase.SpawnWithRagdoll(actualSpawnPos);
            }

            EventSystem.Instance.TriggerEvent(Strings.Events.ENEMY_SPAWNED, newSpawnObj);

        }
        else
        {
            //TODO THIS WILL BREAK
            //ERROR PENDING
            OnEnemyDeath();
            Debug.LogFormat("<color=#ffff00>" + "NOT ENOUGH SPAWN-OBJECTS for: " + spawnType + "</color>");
        }
    }

    private void OnEnemyDeath() {
        currentSpawnAmount--;
        if (currentSpawnAmount <= 0) {
            allEnemiesDead = true;
            onAllEnemiesDead();
        }
    }
    private void Reset(params object[] parameters)
    {
        StopAllCoroutines();
        allEnemiesDead = false;
        currentSpawnAmount = totalSpawnAmount;
        rend.enabled = true;
    }


    #endregion



}
