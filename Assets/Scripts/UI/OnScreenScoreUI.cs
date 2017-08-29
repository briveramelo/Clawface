//Garin
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OnScreenScoreUI : MonoBehaviour {

    #region Public Fields
    #endregion

    #region Serialized Unity Inspector Fields
    #endregion

    #region Private Fields
    private Text onScreenScoreText;
    #endregion

    #region Unity Lifecycle
    void Awake()
    {
        onScreenScoreText = gameObject.GetComponent<Text>();    
    }

    private void LateUpdate()
    {
        UpdateScore();
    }
    #endregion

    #region Public Methods
    #endregion

    #region Private Methods
    void UpdateScore()
    {
        onScreenScoreText.text = "Score:\t" + ScoreManager.Instance.GetScore().ToString() + "\r\n" + "Combo:\t" + ScoreManager.Instance.GetCombo().ToString();
    }
    #endregion

    #region Private Structures
    #endregion
}
