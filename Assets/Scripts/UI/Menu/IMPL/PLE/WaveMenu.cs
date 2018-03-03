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
    [SerializeField] private GameObject realLevelParent;
    [SerializeField] private InputField waveInputField;    

    #endregion

    #region Public Interface

    public WaveMenu() : base(Strings.MenuStrings.LevelEditor.WAVE_PLE_MENU) { }

    #endregion

    #region Private Fields

    #endregion

    #region Protected Interface


    #endregion

    #region Public Interface
    

    public void OnSelectedWaveTextValidated() {
        int newWave = 0;
        if (int.TryParse(waveInputField.text, out newWave)) {
            levelEditor.waveSystem.ChangeToWave(newWave - 1);
        }
        else {
            levelEditor.waveSystem.UpdateWaveText();
        }
    }
    #endregion

    #region Unity Lifecycle

    #endregion

    #region Protected Interface    

    #endregion

    #region Private Interface



    #endregion
}
