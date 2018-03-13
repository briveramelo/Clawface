using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ModMan;

using Turing.VFX;
using System;
using System.Linq;

public class PLESpawn : PLEItem {

    #region Private Fields
    private int currentSpawnAmount;
    private float spawnHeightOffset = 50.0f;
    private List<GameObject> spawnedEnemies = new List<GameObject>();
    
    private Vector3 ActualSpawnPos { get { return transform.position + Vector3.up * spawnHeightOffset; } }
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
    public string DisplayName { get { return spawnType.DisplayName(); } }
    public int MaxPerWave { get { return spawnType.MaxPerWave(); } }
    #endregion
    protected override string ColorTint { get { return "_Color"; } }
    #region Unity Lifecycle    

    protected override void Start() {
        base.Start();

        EventSystem.Instance.RegisterEvent(Strings.Events.PLE_TEST_END, ResetSpawnValues);               
    }

    private void OnDestroy() {
        if (EventSystem.Instance) {
            EventSystem.Instance.UnRegisterEvent(Strings.Events.PLE_TEST_END, ResetSpawnValues);
        }
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
        EnableAllMeshes(false);
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
            newSpawnObj.transform.position = ActualSpawnPos;
            ISpawnable spawnable = newSpawnObj.GetComponentInChildren<ISpawnable>();
            if(!spawnable.HasWillBeenWritten())
            {
                spawnable.RegisterDeathEvent(ReportDeath);
            }
            
            EnemyBase enemyBase = newSpawnObj.GetComponent<EnemyBase>();

            if(enemyBase)
            {
                enemyBase.SpawnWithRagdoll(ActualSpawnPos);
            }

            EventSystem.Instance.TriggerEvent(Strings.Events.ENEMY_SPAWNED, newSpawnObj);

        }
        else
        {
            OnEnemyDeath();
            Debug.LogFormat("<color=#ffff00>" + "NOT ENOUGH SPAWN-OBJECTS for: " + spawnType + "</color>");
        }
    }

    private void OnEnemyDeath() {
        currentSpawnAmount--;
        if (currentSpawnAmount <= 0) {
            allEnemiesDead = true;
            if (onAllEnemiesDead != null)
            {
                onAllEnemiesDead();
            }
        }
    }
    
    private void ResetSpawnValues(params object[] parameters)
    {
        StopAllCoroutines();
        allEnemiesDead = false;
        currentSpawnAmount = totalSpawnAmount;
        EnableAllMeshes(true);
    }

    void EnableAllMeshes(bool isEnabled) {
        Renderers.ForEach(renderer => {
            renderer.enabled = isEnabled;
            SkinnedMeshRenderer meshRenderer = renderer as SkinnedMeshRenderer;
            if (meshRenderer != null) {
                meshRenderer.updateWhenOffscreen = isEnabled;
            }
        });
    }

    #endregion



}
