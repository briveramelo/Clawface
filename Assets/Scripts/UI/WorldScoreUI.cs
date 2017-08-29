//Garin
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorldScoreUI : MonoBehaviour {


    #region Public Fields
     #endregion

    #region Serialized Unity Inspector Fields
    [SerializeField]
    private Text scoreText;
    
    #endregion

    #region Private Fields
    private int scoreValue;
    private GameObject owner;
    #endregion

    #region Unity Lifecycle

    #endregion

    #region Public Methods
    public void DisplayScoreAndHide(int i_val, int i_delay)
    {
        scoreValue = i_val;
        if (i_val > 0)
            scoreText.text = "+";
        else if (i_val < 0)
            scoreText.text = "-";

        scoreText.text += i_val.ToString();
        HideScoreTextAfterDelay(i_delay);
    }
    #endregion

    #region Private Methods



    private void SetAlphaOfScoreText(float i_val)
    {
        Color c = scoreText.color;
        c.a = i_val;
        scoreText.color = c;
        
    }

    private void HideScoreTextAfterDelay(float i_delay)
    {
        //TODO: Just needs a timer that goes i_delay amount of seconds...
        LeanTween.value(gameObject, 0f, 1f, i_delay).setOnUpdate((float val) =>
           {
           }).setOnComplete(HideAndReset);
    }

    private void HideAndReset()
    {
        SetAlphaOfScoreText(0f);
        scoreText.text = "";
        Suicide();
    }

    private void Suicide()
    {
        gameObject.SetActive(false);
    }


    #endregion

    #region Private Structures
    #endregion
}
