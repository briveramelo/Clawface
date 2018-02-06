using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveSystem : MonoBehaviour
{
    public static int maxWave = 3;
    public static int currentWave = 0;

    private void Start()
    {
//        for (int i = 0; i < maxWave; i++)
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Return))
        {
            UpdateLevelUnitState();
        }
    }

    public void DoTheThing()
    {
        Debug.Log(currentWave++);

        if (currentWave >= 3)
            currentWave = 0;
    }

    public void UpdateLevelUnitState()
    {
        if (EventSystem.Instance)
        {
            EventSystem.Instance.TriggerEvent(Strings.Events.PLE_UPDATE_LEVELSTATE);


            string wave = "PLE_TEST_WAVE_" + currentWave.ToString();
            EventSystem.Instance.TriggerEvent(wave);
        }
    }


    public void AddWave()
    {
        if (EventSystem.Instance)
        {
            EventSystem.Instance.TriggerEvent(Strings.Events.PLE_ADD_WAVE);
        }
    }
}
