using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IWaveHandler
{
    protected IWaveData waveData = null;
    protected IWaveHandler nextHandler = null;

    public IWaveHandler SetNextHandler(IWaveHandler i_nextHandler)
    {
        nextHandler = i_nextHandler;
        return nextHandler;
    }

    public abstract IWaveHandler CheckNextWave();
    public abstract void Update();
    public abstract bool IsFinished();
}
