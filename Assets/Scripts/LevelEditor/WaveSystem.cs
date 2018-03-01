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

    private static int systemMaxWave = 20;

    [SerializeField] private InputField waveInputField;
    [SerializeField] private Text totalWaveText;
    [SerializeField] private Toggle infWaveObjToggle;
    [SerializeField] private Button removeWave, addWave;

    #region Unity Lifecycle
    private void Start() {
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
    #endregion

    #region Public interface
    public void UpdateWaveText() {
        waveInputField.text = (currentWave + 1).ToString();
        totalWaveText.text = maxWave.ToString();
    }

    public void ResetToWave0() {
        ChangeToWave(0);
    }

    public void ChangeToWave(int newWave) {
        currentWave = Mathf.Clamp(newWave, 0, maxWave-1);
        UpdateWaveText();
        bool shouldChangeColor = true;
        string wave = Strings.Events.PLE_TEST_WAVE_ + currentWave.ToString();
        EventSystem.Instance.TriggerEvent(wave, shouldChangeColor);
        EventSystem.Instance.TriggerEvent(Strings.Events.PLE_CHANGEWAVE);
    }

    public void NextWave() {
        currentWave++;
        if (currentWave >= maxWave) {
            currentWave = 0;
        }        
        ChangeToWave(currentWave);
    }

    public void PrevWave() {
        currentWave--;
        if (currentWave < 0) {
            currentWave = maxWave - 1;
        }
        ChangeToWave(currentWave);
    }

    public void UpdateLevelUnitState() {
        bool shouldChangeColor = true;
        EventSystem.Instance.TriggerEvent(Strings.Events.PLE_UPDATE_LEVELSTATE);
        string wave = Strings.Events.PLE_TEST_WAVE_ + currentWave.ToString();
        EventSystem.Instance.TriggerEvent(wave, shouldChangeColor);
    }


    public void AddNewWave() {
        if (maxWave == systemMaxWave) return;

        maxWave++;
        if (maxWave==systemMaxWave) {
            addWave.interactable = false;
        }
        removeWave.interactable = true;
        UpdateWaveText();
        
        EventSystem.Instance.TriggerEvent(Strings.Events.PLE_ADD_WAVE);
        EventSystem.Instance.TriggerEvent(Strings.Events.PLE_UPDATE_LEVELSTATE);
    }


    public void DeleteCurrentWave() {
        if (maxWave == 1) return;
        
        EventSystem.Instance.TriggerEvent(Strings.Events.PLE_DELETE_CURRENTWAVE);
        EventSystem.Instance.TriggerEvent(Strings.Events.PLE_UPDATE_LEVELSTATE);        

        maxWave--;
        if (currentWave >= maxWave) {
            currentWave = 0;
        }
        if (maxWave == 1) {
            removeWave.interactable = false;
        }
        addWave.interactable = true;

        UpdateWaveText();

        
        bool shouldChangeColor = true;
        string wave = Strings.Events.PLE_TEST_WAVE_ + currentWave.ToString();
        EventSystem.Instance.TriggerEvent(wave, shouldChangeColor);
    }


    public void UpdateInfWaveState() {
        infWaveSelected = infWaveObjToggle.isOn;
    }
    #endregion
}
