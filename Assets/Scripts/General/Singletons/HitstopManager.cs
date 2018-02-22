// Adam Kay

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;

public class HitstopManager : Singleton<HitstopManager> {

    protected HitstopManager() { }

    [SerializeField]
    private float remainingHitstop;

    [SerializeField]
    private bool isInHitstop;

    public event Action OnStopEvent;
    private const string coroutineString = "TimePolice";

    // Update is called once per frame
    void Update () {
		if (remainingHitstop > 0f)
        {
            remainingHitstop -= Time.unscaledDeltaTime;

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
        isInHitstop = true;
        //TODO recommendation: Lerp to the target timescale for a smoother transition in and out

        remainingHitstop = time;
        Time.timeScale = 0.2f;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;
    }

    public void LerpToTimeScale(float newTimeScale, float lerpFactor=0.05f) {
        newTimeScale = Mathf.Clamp01(newTimeScale);
        lerpFactor = Mathf.Clamp01(lerpFactor);
        Timing.KillCoroutines(coroutineString);
        Timing.RunCoroutine(LerpToTime(newTimeScale, lerpFactor), coroutineString);        
    }

    private IEnumerator<float> LerpToTime(float newTimeScale, float lerpFactor) {
        while (Mathf.Abs(Time.timeScale-newTimeScale) > 0.02f) {
            Time.timeScale = Mathf.Lerp(Time.timeScale, newTimeScale, lerpFactor);
            Time.fixedDeltaTime = 0.02f * Time.timeScale;
            yield return 0f;
        }
        Time.timeScale = newTimeScale;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;
    }

    private void StopHitstop()
    {
        // OnStopEvent();
        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;
        isInHitstop = false;
    }
}
