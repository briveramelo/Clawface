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
        PLESpawnManager.Instance.SetToWave(newWave);
        UpdateWaveText();
        bool shouldChangeColor = true;
        int currentWaveIndex = PLESpawnManager.Instance.CurrentWaveIndex;
        string waveName = Strings.Events.PLE_TEST_WAVE_ + currentWaveIndex.ToString();
        levelEditor.EnableCurrentWaveSpawnParents();
        EventSystem.Instance.TriggerEvent(waveName, shouldChangeColor);
        EventSystem.Instance.TriggerEvent(Strings.Events.PLE_CHANGEWAVE, currentWaveIndex);
    }

    public void UpdateWaveText() {
        string currentWaveAsText = PLESpawnManager.Instance.CurrentWaveText;
        waveInputField.text = currentWaveAsText;
        currentWaveText.text = currentWaveAsText;
        totalWaveText.text = PLESpawnManager.Instance.MaxWaveText;
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
        PLESpawnManager.Instance.TryAddWave();

        SetMenuButtonInteractability();
        UpdateWaveText();
        levelEditor.EnableCurrentWaveSpawnParents();

        EventSystem.Instance.TriggerEvent(Strings.Events.PLE_ADD_WAVE);
        EventSystem.Instance.TriggerEvent(Strings.Events.PLE_UPDATE_LEVELSTATE);
    }

    public void DeleteCurrentWave() {
        PLESpawnManager.Instance.TryDeleteWave(PLESpawnManager.Instance.CurrentWaveIndex);

        EventSystem.Instance.TriggerEvent(Strings.Events.PLE_DELETE_CURRENTWAVE);
        EventSystem.Instance.TriggerEvent(Strings.Events.PLE_UPDATE_LEVELSTATE);

        levelEditor.EnableCurrentWaveSpawnParents();
        SetMenuButtonInteractability();
        UpdateWaveText();


        bool shouldChangeColor = true;
        string wave = Strings.Events.PLE_TEST_WAVE_ + PLESpawnManager.Instance.CurrentWaveIndex.ToString();
        EventSystem.Instance.TriggerEvent(wave, shouldChangeColor);
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
    }
    #endregion

    #region Private Interface
    private void UpdateLevelUnitState() {
        bool shouldChangeColor = true;
        EventSystem.Instance.TriggerEvent(Strings.Events.PLE_UPDATE_LEVELSTATE);
        string wave = Strings.Events.PLE_TEST_WAVE_ + PLESpawnManager.Instance.CurrentWaveIndex.ToString();
        EventSystem.Instance.TriggerEvent(wave, shouldChangeColor);
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
