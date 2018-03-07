using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PlayerLevelEditor;

public class WaveMenu : PlayerLevelEditorMenu
{
    #region Public Fields


    #endregion

    #region Serialized Unity Fields
    [SerializeField] private Text currentWaveText;
    [SerializeField] private InputField waveInputField;
    [SerializeField] private Text totalWaveText;
    [SerializeField] private Toggle infWaveObjToggle;
    [SerializeField] private Button removeWave, addWave, prevWave, nextWave;
    [SerializeField] private Text checkBox;
    #endregion

    #region Public Interface

    public WaveMenu() : base(Strings.MenuStrings.LevelEditor.WAVE_PLE_MENU) { }

    #endregion

    #region Private Fields

    #endregion

    #region Protected Interface
    

    #endregion

    #region Public Interface
    public void ResetToWave0() {
        ChangeToWave(0);
    }

    public void ChangeToWave(int newWave) {
        int currentWaveIndex = PLESpawnManager.Instance.SetToWave(newWave);
        UpdateWaveText();
        EventSystem.Instance.TriggerEvent(Strings.Events.PLE_CHANGEWAVE, currentWaveIndex);
        EventSystem.Instance.TriggerEvent(Strings.Events.PLE_CALL_WAVE, currentWaveIndex);
    }

    public void UpdateWaveText() {
        string currentWaveAsText = PLESpawnManager.Instance.CurrentWaveText;
        waveInputField.text = currentWaveAsText;
        currentWaveText.text = currentWaveAsText;
        totalWaveText.text = PLESpawnManager.Instance.MaxWaveText;
    }

    public void NextWave() {
        int currentWaveIndex = PLESpawnManager.Instance.GoToNextWave();
        UpdateWaveText();
        UpdateLevelUnitState();
        ChangeToWave(currentWaveIndex);
    }

    public void PrevWave() {
        int currentWaveIndex = PLESpawnManager.Instance.GoToPreviousWave();
        UpdateWaveText();
        UpdateLevelUnitState();
        ChangeToWave(currentWaveIndex);
    }

    public void AddNewWave() {
        PLESpawnManager.Instance.TryAddWave();
        EventSystem.Instance.TriggerEvent(Strings.Events.PLE_ADD_WAVE);
        EventSystem.Instance.TriggerEvent(Strings.Events.PLE_SYNC_LEVEL_UNIT_STATES);
        levelEditor.EnableCurrentWaveSpawnParents();

        SetMenuButtonInteractability();
        UpdateWaveText();
    }

    public void DeleteCurrentWave() {
        int currentWaveIndex = PLESpawnManager.Instance.TryDeleteWave(PLESpawnManager.Instance.CurrentWaveIndex);
        EventSystem.Instance.TriggerEvent(Strings.Events.PLE_DELETE_CURRENTWAVE);
        EventSystem.Instance.TriggerEvent(Strings.Events.PLE_SYNC_LEVEL_UNIT_STATES);
        EventSystem.Instance.TriggerEvent(Strings.Events.PLE_CHANGEWAVE, currentWaveIndex);
        levelEditor.EnableCurrentWaveSpawnParents();

        SetMenuButtonInteractability();
        UpdateWaveText();
    }

    public void UpdateInfWaveState() {
        PLESpawnManager.Instance.InfiniteWavesEnabled = infWaveObjToggle.isOn;
        checkBox.enabled = infWaveObjToggle.isOn;
    }

    public void OnSelectedWaveTextValidated() {
        int newWave = 0;
        if (int.TryParse(waveInputField.text, out newWave)) {
            ChangeToWave(newWave - 1);
        }
        else {
            UpdateWaveText();
        }
    }
    #endregion

    #region Unity Lifecycle

    #endregion

    #region Protected Interface    
    protected override void ShowStarted() {
        base.ShowStarted();
        UpdateWaveText();
        SetMenuButtonInteractability();
        infWaveObjToggle.isOn = PLESpawnManager.Instance.InfiniteWavesEnabled;
        UpdateInfWaveState();
    }
    #endregion

    #region Private Interface
    private void UpdateLevelUnitState() {
        EventSystem.Instance.TriggerEvent(Strings.Events.PLE_SYNC_LEVEL_UNIT_STATES);
    }

    private void SetMenuButtonInteractability() {
        bool atMaxWaveLimit = PLESpawnManager.Instance.AtMaxWaveLimit;
        addWave.interactable = !atMaxWaveLimit;

        bool atMinWaveLimit = PLESpawnManager.Instance.AtMinWaveLimit;
        removeWave.interactable = !atMinWaveLimit;
        prevWave.interactable = !atMinWaveLimit;
        nextWave.interactable = !atMinWaveLimit;
    }


    #endregion
}
