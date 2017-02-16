// Adam Kay

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitstopManager : Singleton<HitstopManager> {

    [SerializeField]
    private float remainingHitstop;

    [SerializeField]
    private bool isInHitstop;

    private float hitstopTimer;

	// Use this for initialization
	void Start () {
		
	}
	
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
        isInHitstop = false;
    }
}
