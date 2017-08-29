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
    private Text scoreText_;
    
    #endregion

    #region Private Fields
    private int scoreValue_;
    private GameObject owner_;
    #endregion

    #region Unity Lifecycle
    void Start()
    {
        //SetAlphaOfScoreText(0f);
    }
    #endregion

    #region Public Methods
    public void DisplayScoreAndHide(int i_val, int i_delay)
    {
        scoreValue_ = i_val;
        if (i_val > 0)
            scoreText_.text = "+";
        else if (i_val < 0)
            scoreText_.text = "-";

        scoreText_.text += i_val.ToString();
        HideScoreTextAfterDelay(i_delay);
    }
    #endregion

    #region Private Methods


    private void SetAlphaOfScoreText(float i_val)
    {
        Color c = scoreText_.color;
        c.a = i_val;
        scoreText_.color = c;
        
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
        scoreText_.text = "";
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
