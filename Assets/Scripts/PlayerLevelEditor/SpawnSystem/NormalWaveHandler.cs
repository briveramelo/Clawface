using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalWaveHandler : IWaveHandler
{
    public override IWaveHandler CheckNextWave()
    {
        throw new System.NotImplementedException();
    }

    public override void Update()
    {
        waveData.Update();
    }

    public override bool IsFinished()
    {
        return waveData.IsFinished();
    }
}
