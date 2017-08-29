using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyScoreUI : MonoBehaviour {


    #region Public Fields

    public GameObject owner
    {
        get
        {
            return owner_;
        }
        set
        {
            owner_ = value;
        }
    }
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
        //TODO: get stats from current enemy and get score value
        SetAlphaOfScoreText(0f);
    }
    #endregion

    #region Public Methods
    public void DisplayScoreAndHide(int i_val)
    {
        scoreValue_ = i_val;
        scoreText_.text = "+"+scoreValue_.ToString();
        SetAlphaOfScoreText(1f);
        HideScoreTextAfterDelay(2f);
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
        LeanTween.value(gameObject, 0f, 1f, i_delay).setOnUpdate((float val) =>
           {
               //Debug.Log("sorry");
           }).setOnComplete(HideScore);
    }

    private void HideScore()
    {
        SetAlphaOfScoreText(0f);
        Suicide();
    }

    private void Suicide()
    {
        owner_.SetActive(false);
    }


    #endregion

    #region Private Structures
    #endregion




    //// Update is called once per frame
    //void Update () {

    //}
}
