using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveSystem : MonoBehaviour
{
    public static int maxWave = 3;
    public static int currentWave = 0;

    private void Start()
    {

    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Return))
        {
            string wave = Strings.Events.PLE_TEST_WAVE_ + currentWave.ToString();

            EventSystem.Instance.TriggerEvent(wave);

            currentWave++;

            if (currentWave >= 3)
                currentWave = 0;
        }
    }

    public void DoTheThing()
    {
        /*
        Debug.Log(currentWave++);

        if (currentWave >= 3)
            currentWave = 0;
            */
    }

    public void UpdateLevelUnitState()
    {
        if (EventSystem.Instance)
        {
            EventSystem.Instance.TriggerEvent(Strings.Events.PLE_UPDATE_LEVELSTATE);

            string wave = Strings.Events.PLE_TEST_WAVE_ + currentWave.ToString();

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


    public void DeleteCurrentWave()
    {
        if (EventSystem.Instance)
        {
            EventSystem.Instance.TriggerEvent(Strings.Events.PLE_DELETE_CURRENTWAVE);
        }
    }
}
