using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PLE;

public class WaveMenu : PLEMenu
{
    #region Public Fields


    #endregion

    #region Serialized Unity Fields
    [SerializeField] private Text currentWaveText;
    //[SerializeField] private InputField waveInputField;
    [SerializeField] private Text totalWaveText;
    [SerializeField] private Button infWaveButton;
    [SerializeField] private Button removeWave, addWave, prevWave, nextWave;
    [SerializeField] private Text checkBox;
    #endregion

    #region Public Interface

    public WaveMenu() : base(Strings.MenuStrings.LevelEditor.WAVE_PLE_MENU) { }

    #endregion

    #region Private Fields
    private bool IsWavesInfinite {
        get { return PLESpawnManager.Instance.InfiniteWavesEnabled; }
        set {
            PLESpawnManager.Instance.InfiniteWavesEnabled=value;
            UpdateInfiniteWaveUI();
        }
    }
    #endregion

    #region Protected Interface
    #endregion

    #region Public Interface
    public override void SetMenuButtonInteractabilityByState() {
        bool atMaxWaveLimit = PLESpawnManager.Instance.AtMaxWaveLimit;
        addWave.interactable = !atMaxWaveLimit;

        bool atMinWaveLimit = PLESpawnManager.Instance.AtMinWaveLimit;
        removeWave.interactable = !atMinWaveLimit;
        prevWave.interactable = !atMinWaveLimit;
        nextWave.interactable = !atMinWaveLimit;


        bool wavesCanWrap = PLESpawnManager.Instance.Wave0SpawnsAllowForWrapping();
        infWaveButton.interactable = wavesCanWrap;
        if (!wavesCanWrap) { //shouldn't happen, but yeah why not?
            IsWavesInfinite = false;
        }
    }

    public void ResetToWave0() {        
        ChangeToWave(0);
    }    

    public void UpdateWaveText() {
        string currentWaveAsText = PLESpawnManager.Instance.CurrentWaveText;
        //waveInputField.text = currentWaveAsText;
        currentWaveText.text = currentWaveAsText;
        totalWaveText.text = PLESpawnManager.Instance.MaxWaveText;
    }

    public void NextWave() {
        SFXManager.Instance.Play(SFXType.UI_Click);
        int currentWaveIndex = PLESpawnManager.Instance.GoToNextWave();
        ChangeToWave(currentWaveIndex);
    }

    public void PrevWave() {
        SFXManager.Instance.Play(SFXType.UI_Click);
        int currentWaveIndex = PLESpawnManager.Instance.GoToPreviousWave();
        ChangeToWave(currentWaveIndex);
    }

    public void AddNewWave() {
        SFXManager.Instance.Play(SFXType.UI_Click);
        PLESpawnManager.Instance.TryAddWave();
        EventSystem.Instance.TriggerEvent(Strings.Events.PLE_ADD_WAVE);
        EventSystem.Instance.TriggerEvent(Strings.Events.PLE_SYNC_LEVEL_UNIT_STATES);
        mainPLEMenu.SetMenuButtonInteractabilityByState();
        UpdateWaveText();
    }

    public void DeleteCurrentWave() {
        SFXManager.Instance.Play(SFXType.UI_Click);
        int currentWaveIndex = PLESpawnManager.Instance.TryDeleteWave(PLESpawnManager.Instance.CurrentWaveIndex);
        EventSystem.Instance.TriggerEvent(Strings.Events.PLE_DELETE_CURRENTWAVE);
        EventSystem.Instance.TriggerEvent(Strings.Events.PLE_SYNC_LEVEL_UNIT_STATES);
        EventSystem.Instance.TriggerEvent(Strings.Events.PLE_CALL_WAVE, currentWaveIndex, false);
        levelEditor.EnableCurrentWaveSpawnParents();
        mainPLEMenu.SetMenuButtonInteractabilityByState();
        UpdateWaveText();
    }

    public void ToggleInfWaveState(bool playSound=true) {
        if (playSound) {
            SFXManager.Instance.Play(SFXType.UI_Click);
        }
        IsWavesInfinite = !IsWavesInfinite;
    }


    //public void OnSelectedWaveTextValidated() {
    //    int newWave = 0;
    //    if (int.TryParse(waveInputField.text, out newWave)) {
    //        ChangeToWave(newWave - 1);
    //    }
    //    else {
    //        UpdateWaveText();
    //    }
    //}
    #endregion

    #region Unity Lifecycle

    #endregion

    #region Protected Interface    
    protected override void ShowStarted() {
        base.ShowStarted();
        UpdateWaveText();
        UpdateInfiniteWaveUI();
    }
    #endregion

    #region Private Interface
    private void ChangeToWave(int newWave) {
        int currentWaveIndex = PLESpawnManager.Instance.SetToWave(newWave);
        UpdateWaveText();
        UpdateLevelUnitState();
        mainPLEMenu.DeselectAllBlocks();
        mainPLEMenu.SetMenuButtonInteractabilityByState();
        EventSystem.Instance.TriggerEvent(Strings.Events.PLE_CALL_WAVE, currentWaveIndex, false);
    }
    private void UpdateInfiniteWaveUI() {
        checkBox.enabled = IsWavesInfinite;
    }

    private void UpdateLevelUnitState() {
        EventSystem.Instance.TriggerEvent(Strings.Events.PLE_SYNC_LEVEL_UNIT_STATES);
    }
    #endregion
}
