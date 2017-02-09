using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ModMan;

public class MallCopSpawner : MonoBehaviour {

    [SerializeField] GameObject mallCop;
    [SerializeField] List<Transform> spawnPoints;
    [System.Serializable]
    struct Wave {
        public int maxMallCops;
        public int minMallCops;
        public Wave(int maxMallCops, int minMallCops) {
            this.maxMallCops = maxMallCops;
            this.minMallCops = minMallCops;
        }
    }
    [SerializeField] List<Wave> mallCopWaves;
    int currentWave {
        get {
            if (currentWaveNumber >= mallCopWaves.Count) {
                return mallCopWaves.Count-1;
            }
            return currentWaveNumber;
        }
        set { currentWaveNumber = value; }
    }           
    [SerializeField] int currentWaveNumber=0;
    [SerializeField] int numMallCops=0;

    void Awake() {
        SpawnMallCop();
    }

    
    public void ReportDeath() {
        if (Application.isPlaying) {
            numMallCops--;
            if (numMallCops < mallCopWaves[currentWave].minMallCops) {
                SpawnMallCop();
            }
        }
    }

    void SpawnMallCop() {
        numMallCops++;
        //GameObject cop = Instantiate(mallCop, spawnPoints.GetRandom(), false);
        GameObject cop = Instantiate(mallCop);
        cop.transform.position = spawnPoints.GetRandom().position;
        cop.GetComponent<MallCopAI>().RegisterDeathEvent(ReportDeath);
        if (numMallCops < mallCopWaves[currentWave].maxMallCops)
        {
            Invoke("SpawnMallCop", Random.Range(1f,2f));
        }
        else {
            currentWave++;
        }
    }
}
