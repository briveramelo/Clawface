//Garin
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class OnScreenComboTimerUI : MonoBehaviour {

    #region Public Fields
    #endregion

    #region Serialized Unity Inspector Fields
    #endregion

    #region Private Fields
    private Slider comboTimer;
    private ScoreManager sm;
    #endregion

    #region Unity Lifecycle
    private void Start()
    {
        sm = ScoreManager.Instance;
        comboTimer = gameObject.GetComponent<Slider>();
        comboTimer.value = 0f;
    }
    private void Update()
    {
        //if there's an sm, not close to 0, and non-negative
        if (sm && !Mathf.Equals(sm.GetMaxTimeRemaining(),0f) && sm.GetCurrentTimeRemaining() > 0f)
        {
            SetTimerValue();           
        }
        else if(sm.GetCurrentTimeRemaining() < 0f)
        {
            ResetTimerValue();
        }
    }

    #endregion

    #region Public Methods
    #endregion

    #region Private Methods
    private void SetTimerValue()
    {
        comboTimer.value = sm.GetCurrentTimeRemaining() / sm.GetMaxTimeRemaining();
    }

    private void ResetTimerValue()
    {
        comboTimer.value = 0f;
    }
    #endregion

    #region Private Structures
    #endregion


}
