using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ModMan;

public class MallCopSpawner : MonoBehaviour {

    #region Serialized Unity Fields
    [SerializeField] List<Transform> spawnPoints;
    [SerializeField] List<Wave> mallCopWaves;
    [SerializeField] int currentWaveNumber=0;
    [SerializeField] int numMallCops=0;
    #endregion

    #region private variables
    int currentWave {
        get {
            if (currentWaveNumber >= mallCopWaves.Count) {
                return mallCopWaves.Count-1;
            }
            return currentWaveNumber;
        }
        set { currentWaveNumber = value; }
    }
    #endregion

    #region Unity LifeCycle
    void Start() {
        SpawnMallCop();
    }
    #endregion

    #region Private Methods
    private void ReportDeath() {
        numMallCops--;
        if (numMallCops < mallCopWaves[currentWave].minMallCops) {
            SpawnMallCop();
        }
    }

    private void SpawnMallCop() {
        if (Application.isPlaying)
        {
            numMallCops++;
            GameObject cop = ObjectPool.Instance.GetObject(PoolObjectType.MallCop);
            MallCop mallCop = cop.GetComponent<MallCop>();
            if (!mallCop.HasWillBeenWritten()) {
                mallCop.RegisterDeathEvent(ReportDeath);
            }
            cop.transform.position = spawnPoints.GetRandom().position;
            
            if (numMallCops < mallCopWaves[currentWave].maxMallCops)
            {
                Invoke("SpawnMallCop", Random.Range(1f, 2f));
            }
            else
            {
                currentWave++;
            }
        }
    }
    #endregion

    #region Internal Structures
    [System.Serializable]
    struct Wave
    {
        public int maxMallCops;
        public int minMallCops;
        public Wave(int maxMallCops, int minMallCops)
        {
            this.maxMallCops = maxMallCops;
            this.minMallCops = minMallCops;
        }
    }
    #endregion
}
