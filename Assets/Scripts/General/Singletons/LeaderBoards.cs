using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AllTimeLeaderBoard))]
public class LeaderBoards : Singleton<LeaderBoards> {

    public enum SelectionType
    {
        GLOBAL,
        FRIENDS,
        AROUND_USER
    }

    #region Private Variables
    private AllTimeLeaderBoard allTimeLeaderBoard;
    private string leaderboardNamePostString = "_ALL_TIME";
    #endregion

    #region Unity Lifecycle
    // Use this for initialization
    void Start () {
        allTimeLeaderBoard = GetComponent<AllTimeLeaderBoard>();
    }
    #endregion

    #region Public Methods
    public bool GetLeaderBoardData(string levelName, GenericSteamLeaderBoard.ResultsCallBack callBackFunction, int numberOfEntries, SelectionType selectionType = SelectionType.GLOBAL)
    {
        bool result = false;
        Steamworks.ELeaderboardDataRequest requestType;

        levelName += leaderboardNamePostString;

        switch (selectionType)
        {
            case SelectionType.FRIENDS:
                requestType = Steamworks.ELeaderboardDataRequest.k_ELeaderboardDataRequestFriends;
                break;
            case SelectionType.AROUND_USER:
                requestType = Steamworks.ELeaderboardDataRequest.k_ELeaderboardDataRequestGlobalAroundUser;
                break;
            case SelectionType.GLOBAL:
            default:
                requestType = Steamworks.ELeaderboardDataRequest.k_ELeaderboardDataRequestGlobal;
                break;
        }

        result = allTimeLeaderBoard.FetchLeaderBoardData(levelName, callBackFunction, numberOfEntries, requestType);
        return result;
    }

    public bool UpdateScore(int score, string levelName)
    {
        bool result = false;
        levelName += leaderboardNamePostString;
        result = allTimeLeaderBoard.UpdateLeaderBoard(score, levelName);
        return result;
    }
    #endregion
}
