using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalWaveData : IWaveData
{
    class WaveData
    {
        public SpawnType spawnType;
        public int       count;
        public float     cooldown;
        public float     cooldown_Max;
        public bool      done = false;

        public WaveData(SpawnType spawnType, int count, float period = 1.0f)
        {
            this.spawnType = spawnType;
            this.count = count;

            cooldown = period;
            cooldown_Max = period;
        }
    }


    private PoolObjectType GetPoolObject(SpawnType spawnType)
    {
        switch (spawnType)
        {
            case SpawnType.Blaster:
                return PoolObjectType.MallCopBlaster;
            case SpawnType.Zombie:
                return PoolObjectType.Zombie;
            case SpawnType.Bouncer:
                return PoolObjectType.Bouncer;
            case SpawnType.Kamikaze:
                return PoolObjectType.Kamikaze;
        }
        return PoolObjectType.MallCopBlaster;
    }

    private void ReportDeath()
    {

    }



    private List<WaveData> m_WaveData = new List<WaveData>();

    public void AddWaveData(SpawnType spawnType, int count)
    {
        m_WaveData.Add(new WaveData(spawnType, count));
    }

    public override void Update()
    {
        throw new System.NotImplementedException();
        /*
        if (m_WaveData.Count == 0) return;

        for(int i = 0; i < m_WaveData.Count; i++)
        {
            if (m_WaveData[i].count == 0) continue;

            m_WaveData[i].cooldown -= Time.deltaTime;

            if (m_WaveData[i].cooldown > 0) continue;

            m_WaveData[i].cooldown = m_WaveData[i].cooldown_Max;
            m_WaveData[i].count--;


            GameObject spawnedObject = ObjectPool.Instance.GetObject(GetPoolObject(m_WaveData[i].spawnType));

            if (spawnedObject)
            {
                ISpawnable spawnable = spawnedObject.GetComponentInChildren<ISpawnable>();

                if (!spawnable.HasWillBeenWritten())
                {
                    spawnable.RegisterDeathEvent(ReportDeath);
                }

//              Vector3 spawnPosition = spawnPoints.GetRandom().position;
 //               spawnable.WarpToNavMesh(spawnPosition);

            }
            */
    }

    public override bool IsFinished()
    {
        throw new System.NotImplementedException();
    }
}
