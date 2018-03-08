using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardEntry : MonoBehaviour {

    #region serialized fields
    [SerializeField]
    private Text rankText;
    [SerializeField]
    private Text nameText;
    [SerializeField]
    private Text scoreText;
    #endregion

    #region public methods
    public void SetData(string rank, string name, string score)
    {
        rankText.text = rank;
        nameText.text = name;
        scoreText.text = score;
    }

    public void IsVisible(bool visibility)
    {
        gameObject.SetActive(visibility);
    }
    #endregion
}
