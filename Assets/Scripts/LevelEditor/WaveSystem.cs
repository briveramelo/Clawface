using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaveSystem : MonoBehaviour
{
    public static int maxWave = 3;
    public static int currentWave = 0;

    public GameObject WaveTextObj;
    private Text WaveText;

    public GameObject TotalWaveTextObj;
    private Text TotalWaveText;

    private void Start()
    {
        WaveText = WaveTextObj.GetComponent<Text>();
        TotalWaveText = TotalWaveTextObj.GetComponent<Text>();

        UpdateWaveText();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Return))
        {
            string wave = Strings.Events.PLE_TEST_WAVE_ + currentWave.ToString();

            EventSystem.Instance.TriggerEvent(wave);

            currentWave++;

            if (currentWave >= maxWave)
                currentWave = 0;
        }
    }

    private void UpdateWaveText()
    {
        WaveText.text = (currentWave + 1).ToString();
        TotalWaveText.text = maxWave.ToString();
    }



    public void DoTheThing()
    {

    }

    public void NextWave()
    {
        currentWave++;

        if (currentWave >= maxWave)
            currentWave = 0;


        UpdateWaveText();

        string wave = Strings.Events.PLE_TEST_WAVE_ + currentWave.ToString();
        EventSystem.Instance.TriggerEvent(wave);
    }

    public void PrevWave()
    {
        currentWave--;

        if (currentWave < 0)
            currentWave = maxWave - 1;

        UpdateWaveText();

        string wave = Strings.Events.PLE_TEST_WAVE_ + currentWave.ToString();

        EventSystem.Instance.TriggerEvent(wave);
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
