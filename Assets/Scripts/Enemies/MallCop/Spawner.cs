using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ModMan;

public class Spawner : MonoBehaviour {

    #region Serialized Unity Fields
    [SerializeField] SpawnType spawnType;
    [SerializeField] List<Transform> spawnPoints;
    [SerializeField] List<Wave> waves;
    [SerializeField] int currentWaveNumber=0;
    [SerializeField] int numEnemies=0;
    #endregion

    #region private variables
    private PoolObjectType objectToSpawn {
        get {
            switch (spawnType) {
                case SpawnType.Swinger:
                    return PoolObjectType.MallCopSwinger;
                case SpawnType.Blaster:
                    return PoolObjectType.MallCopBlaster;
                case SpawnType.Grappler:
                    return PoolObjectType.GrapplingBot;
            }
            return PoolObjectType.MallCopSwinger;
        }
    }

    private int currentWave {
        get {
            if (currentWaveNumber >= waves.Count) {
                return waves.Count-1;
            }
            return currentWaveNumber;
        }
        set { currentWaveNumber = value; }
    }
    #endregion

    #region Unity LifeCycle
    void Start() {
        SpawnEnemy();
    }
    #endregion

    #region Private Methods
    private void ReportDeath() {
        numEnemies--;
        Invoke("SpawnEnemy", Random.Range(1f,2f));        
    }

    private void SpawnEnemy() {        
        if (Application.isPlaying){
            if (numEnemies < waves[currentWave].max){
                GameObject spawnedObject = ObjectPool.Instance.GetObject(objectToSpawn);
                if (spawnedObject){
                    numEnemies++;
                    ISpawnable spawnable = spawnedObject.GetComponentInChildren<ISpawnable>();
                    if (!spawnable.HasWillBeenWritten()){
                        spawnable.RegisterDeathEvent(ReportDeath);
                    }
                    spawnedObject.transform.position = spawnPoints.GetRandom().position;

                    if (numEnemies < waves[currentWave].max){
                        Invoke("SpawnEnemy", Random.Range(1f, 2f));
                    }
                    else{
                        currentWave++;
                    }
                }
            }
        }
    }
    #endregion

    #region Internal Structures
    [System.Serializable]
    struct Wave
    {
        public int max;
        public int min;
        public Wave(int max, int min)
        {
            this.max = max;
            this.min = min;
        }
    }
    #endregion
}
