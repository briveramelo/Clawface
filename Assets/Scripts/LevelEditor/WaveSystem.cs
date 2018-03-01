//Lai
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaveSystem : MonoBehaviour
{
    public static bool infWaveSelected = true;
    public static int maxWave = 3;
    public static int currentWave = 0;

    private static int systemMaxWave = 12;

    public GameObject WaveTextObj;
    private Text WaveText;

    public GameObject TotalWaveTextObj;
    private Text TotalWaveText;

    public  GameObject InfWaveObj;
    private Toggle InfWaveObjToggle;

    private void Start()
    {
        WaveText = WaveTextObj.GetComponent<Text>();

        TotalWaveText = TotalWaveTextObj.GetComponent<Text>();

        InfWaveObjToggle = InfWaveObj.GetComponent<Toggle>();
        infWaveSelected = InfWaveObjToggle.isOn;

        UpdateWaveText();
    }

    private void Update()
    {

        //if(Input.GetKeyDown(KeyCode.Return))
        //{
        //    string wave = Strings.Events.PLE_TEST_WAVE_ + currentWave.ToString();

        //    EventSystem.Instance.TriggerEvent(wave);

        //    currentWave++;

        //    if (currentWave >= maxWave)
        //        currentWave = 0;
        //}
    }

    private void UpdateWaveText()
    {
        WaveText.text = (currentWave + 1).ToString();
        TotalWaveText.text = maxWave.ToString();
    }



    public void DoTheThing()
    {

    }

    public void ResetToWave0() {
        currentWave = 1;
        PrevWave();
    }

    public void NextWave()
    {
        currentWave++;

        if (currentWave >= maxWave)
            currentWave = 0;


        UpdateWaveText();

        string wave = Strings.Events.PLE_TEST_WAVE_ + currentWave.ToString();
        EventSystem.Instance.TriggerEvent(wave);
        EventSystem.Instance.TriggerEvent(Strings.Events.PLE_CHANGEWAVE);
    }

    public void PrevWave()
    {
        currentWave--;

        if (currentWave < 0)
            currentWave = maxWave - 1;

        UpdateWaveText();

        string wave = Strings.Events.PLE_TEST_WAVE_ + currentWave.ToString();

        EventSystem.Instance.TriggerEvent(wave);
        EventSystem.Instance.TriggerEvent(Strings.Events.PLE_CHANGEWAVE);
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


    public void AddNewWave()
    {
        if (maxWave == systemMaxWave) return;

        maxWave++;
        UpdateWaveText();

        if (EventSystem.Instance)
        {
            EventSystem.Instance.TriggerEvent(Strings.Events.PLE_ADD_WAVE);
            EventSystem.Instance.TriggerEvent(Strings.Events.PLE_UPDATE_LEVELSTATE);
        }
    }


    public void DeleteCurrentWave()
    {
        if (maxWave == 1) return;


        if (EventSystem.Instance)
        {
            EventSystem.Instance.TriggerEvent(Strings.Events.PLE_DELETE_CURRENTWAVE);
            EventSystem.Instance.TriggerEvent(Strings.Events.PLE_UPDATE_LEVELSTATE);
        }

        maxWave--;

        if (currentWave >= maxWave)
            currentWave = 0;

        UpdateWaveText();


        if (EventSystem.Instance)
        {
            string wave = Strings.Events.PLE_TEST_WAVE_ + currentWave.ToString();
            EventSystem.Instance.TriggerEvent(wave);
        }
    }


    public void UpdateInfWaveState()
    {
        infWaveSelected = InfWaveObjToggle.isOn;
    }

}
