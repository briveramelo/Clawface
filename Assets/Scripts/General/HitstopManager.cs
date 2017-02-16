// Adam Kay

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitstopManager : Singleton<HitstopManager> {

    [SerializeField]
    private float remainingHitstop;

    [SerializeField]
    private bool isInHitstop;

    public event Action OnStopEvent;

    // Update is called once per frame
    void Update () {
		if (remainingHitstop > 0f)
        {
            remainingHitstop -= Time.deltaTime;

            if (remainingHitstop <= 0f)
            {
                StopHitstop();
            }
        }
	}

    public bool IsInHitstop()
    {
        return isInHitstop;
    }

    public void StartHitstop(float time)
    {
        remainingHitstop = time;
        isInHitstop = true;
    }

    private void StopHitstop()
    {
        OnStopEvent();
        isInHitstop = false;
    }
}
