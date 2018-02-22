using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AllTimeLeaderBoard))]
public class LeaderBoards : Singleton<LeaderBoards> {

    public enum LeaderBoardType
    {
        ALL_TIME,
        WEEKLY,
        DAILY
    }

    #region Private Variables
    private AllTimeLeaderBoard allTimeLeaderBoard;
    #endregion

    #region Unity Lifecycle
    // Use this for initialization
    void Start () {
        allTimeLeaderBoard = GetComponent<AllTimeLeaderBoard>();
    }
	
	// Update is called once per frame
	void Update () {
       
    }
    #endregion

    #region Public Methods
    public bool GetLeaderBoardData(LeaderBoardType type, GenericSteamLeaderBoard.ResultsCallBack callBackFunction, int numberOfEntries)
    {
        bool result = false;
        switch (type)
        {
            case LeaderBoardType.ALL_TIME:
                result = allTimeLeaderBoard.FetchLeaderBoardData(callBackFunction, numberOfEntries);
                break;
            case LeaderBoardType.WEEKLY:
                break;
            case LeaderBoardType.DAILY:
                break;
            default:
                break;
        }
        return result;
    }

    public bool UpdateScore(int score)
    {
        bool result = false;
        result = allTimeLeaderBoard.UpdateLeaderBoard(score);
        return result;
    }
    #endregion
}
