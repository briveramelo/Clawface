//Lai
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaveSystem : MonoBehaviour
{
    public static bool infWaveSelected = true;
    public static int maxWave = 3;
    public static int currentWorkingWave = 0;

    private static int systemMaxWave = 20;

    [SerializeField] private InputField waveInputField;
    [SerializeField] private Text totalWaveText, currentWaveText;
    [SerializeField] private Toggle infWaveObjToggle;
    [SerializeField] private Button removeWave, addWave, prevWave, nextWave;

    #region Unity Lifecycle
    private void Start() {
        UpdateWaveText();
    }
    
    #endregion

    #region Public interface
    public void UpdateWaveText() {
        waveInputField.text = (currentWorkingWave + 1).ToString();
        currentWaveText.text = (currentWorkingWave + 1).ToString();
        totalWaveText.text = maxWave.ToString();
    }

    public void ResetToWave0() {
        ChangeToWave(0);
    }

    public void ChangeToWave(int newWave) {
        currentWorkingWave = Mathf.Clamp(newWave, 0, maxWave-1);
        UpdateWaveText();
        bool shouldChangeColor = true;
        string wave = Strings.Events.PLE_TEST_WAVE_ + currentWorkingWave.ToString();
        EventSystem.Instance.TriggerEvent(wave, shouldChangeColor);
        EventSystem.Instance.TriggerEvent(Strings.Events.PLE_CHANGEWAVE);
    }

    public void NextWave() {
        currentWorkingWave++;
        if (currentWorkingWave >= maxWave) {
            currentWorkingWave = 0;
        }        
        ChangeToWave(currentWorkingWave);
    }

    public void PrevWave() {
        currentWorkingWave--;
        if (currentWorkingWave < 0) {
            currentWorkingWave = maxWave - 1;
        }
        ChangeToWave(currentWorkingWave);
    }

    public void UpdateLevelUnitState() {
        bool shouldChangeColor = true;
        EventSystem.Instance.TriggerEvent(Strings.Events.PLE_UPDATE_LEVELSTATE);
        string wave = Strings.Events.PLE_TEST_WAVE_ + currentWorkingWave.ToString();
        EventSystem.Instance.TriggerEvent(wave, shouldChangeColor);
    }


    public void AddNewWave() {
        if (maxWave == systemMaxWave) return;

        maxWave++;
        if (maxWave==systemMaxWave) {
            addWave.interactable = false;
        }
        removeWave.interactable = true;
        prevWave.interactable = true;
        nextWave.interactable = true;
        UpdateWaveText();
        
        EventSystem.Instance.TriggerEvent(Strings.Events.PLE_ADD_WAVE);
        EventSystem.Instance.TriggerEvent(Strings.Events.PLE_UPDATE_LEVELSTATE);
    }


    public void DeleteCurrentWave() {
        if (maxWave == 1) return;
        
        EventSystem.Instance.TriggerEvent(Strings.Events.PLE_DELETE_CURRENTWAVE);
        EventSystem.Instance.TriggerEvent(Strings.Events.PLE_UPDATE_LEVELSTATE);        

        maxWave--;
        if (currentWorkingWave >= maxWave) {
            currentWorkingWave = maxWave-1;
        }
        if (maxWave == 1) {
            removeWave.interactable = false;
            prevWave.interactable = false;
            nextWave.interactable = false;
        }
        addWave.interactable = true;

        UpdateWaveText();

        
        bool shouldChangeColor = true;
        string wave = Strings.Events.PLE_TEST_WAVE_ + currentWorkingWave.ToString();
        EventSystem.Instance.TriggerEvent(wave, shouldChangeColor);
    }


    public void UpdateInfWaveState() {
        infWaveSelected = infWaveObjToggle.isOn;
    }
    #endregion
}
