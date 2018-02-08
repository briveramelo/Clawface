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
            switch (currentWave)
            {
                case 0:
                    EventSystem.Instance.TriggerEvent(Strings.Events.PLE_TEST_WAVE_0);
                    break;
                case 1:
                    EventSystem.Instance.TriggerEvent(Strings.Events.PLE_TEST_WAVE_1);
                    break;
                case 2:
                    EventSystem.Instance.TriggerEvent(Strings.Events.PLE_TEST_WAVE_2);
                    break;
            }


            currentWave++;

            if (currentWave >= 3)
                currentWave = 0;
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
}
