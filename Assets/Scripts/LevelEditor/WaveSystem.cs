//Lai
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaveSystem : MonoBehaviour
{
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
        int max = PLESpawnManager.Instance.MaxWave;
        waveInputField.text = (PLESpawnManager.Instance.CurrentWave + 1).ToString();
        currentWaveText.text = (PLESpawnManager.Instance.CurrentWave + 1).ToString();
        totalWaveText.text = PLESpawnManager.Instance.MaxWave.ToString();
    }

    public void ResetToWave0() {
        PLESpawnManager.Instance.SetToWave(0);
        ChangeToWave(0);
    }

    public void ChangeToWave(int newWave) {
        PLESpawnManager.Instance.SetToWave(newWave);
        UpdateWaveText();
        bool shouldChangeColor = true;
        string wave = Strings.Events.PLE_TEST_WAVE_ + PLESpawnManager.Instance.CurrentWave.ToString();
        EventSystem.Instance.TriggerEvent(wave, shouldChangeColor);
        EventSystem.Instance.TriggerEvent(Strings.Events.PLE_CHANGEWAVE);
    }

    public void NextWave() {
        PLESpawnManager.Instance.GoToNextWave();
        UpdateWaveText();
        UpdateLevelUnitState();
    }

    public void PrevWave() {
        PLESpawnManager.Instance.GoToPreviousWave();
        UpdateWaveText();
        UpdateLevelUnitState();

    }

    public void AddNewWave() {

        PLESpawnManager.Instance.AddWave();

        if(PLESpawnManager.Instance.AtMaxWaveLimit)
        {
            addWave.interactable = false;
        }
        else
        {
            removeWave.interactable = true;
            prevWave.interactable = true;
            nextWave.interactable = true;
        }

        UpdateWaveText();
        
        EventSystem.Instance.TriggerEvent(Strings.Events.PLE_ADD_WAVE);
        EventSystem.Instance.TriggerEvent(Strings.Events.PLE_UPDATE_LEVELSTATE);
    }

    public void DeleteCurrentWave() {
        PLESpawnManager.Instance.DeleteWave(PLESpawnManager.Instance.CurrentWave);
        
        EventSystem.Instance.TriggerEvent(Strings.Events.PLE_DELETE_CURRENTWAVE);
        EventSystem.Instance.TriggerEvent(Strings.Events.PLE_UPDATE_LEVELSTATE);        
        
        if(PLESpawnManager.Instance.OneWaveRemaining)
        {
            removeWave.interactable = false;
            prevWave.interactable = false;
            nextWave.interactable = false;
        }
        addWave.interactable = true;

        UpdateWaveText();

        
        bool shouldChangeColor = true;
        string wave = Strings.Events.PLE_TEST_WAVE_ + PLESpawnManager.Instance.CurrentWave.ToString();
        EventSystem.Instance.TriggerEvent(wave, shouldChangeColor);
    }

    public void UpdateInfWaveState() {
        PLESpawnManager.Instance.InfiniteWavesEnabled = infWaveObjToggle.isOn;
        
    }
    #endregion

    #region Private Interface
    private void UpdateLevelUnitState()
    {
        bool shouldChangeColor = true;
        EventSystem.Instance.TriggerEvent(Strings.Events.PLE_UPDATE_LEVELSTATE);
        string wave = Strings.Events.PLE_TEST_WAVE_ + PLESpawnManager.Instance.CurrentWave.ToString();
        EventSystem.Instance.TriggerEvent(wave, shouldChangeColor);
    }

    #endregion
}
