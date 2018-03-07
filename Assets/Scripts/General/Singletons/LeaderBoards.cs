using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AllTimeLeaderBoard), typeof(WeeklyLeaderBoard), typeof(DailyLeaderBoard))]
public class LeaderBoards : Singleton<LeaderBoards> {

    public enum LeaderBoardType
    {
        ALL_TIME,
        WEEKLY,
        DAILY
    }

    public enum SelectionType
    {
        GLOBAL,
        FRIENDS,
        AROUND_USER
    }

    #region Private Variables
    private AllTimeLeaderBoard allTimeLeaderBoard;
    private WeeklyLeaderBoard weeklyLeaderBoard;
    private DailyLeaderBoard dailyLeaderBoard;
    #endregion

    #region Unity Lifecycle
    // Use this for initialization
    void Start () {
        allTimeLeaderBoard = GetComponent<AllTimeLeaderBoard>();
        weeklyLeaderBoard = GetComponent<WeeklyLeaderBoard>();
        dailyLeaderBoard = GetComponent<DailyLeaderBoard>();
    }
    #endregion

    #region Public Methods
    public bool GetLeaderBoardData(GenericSteamLeaderBoard.ResultsCallBack callBackFunction, int numberOfEntries, SelectionType type = SelectionType.GLOBAL)
    {
        return GetLeaderBoardData(LeaderBoardType.ALL_TIME, callBackFunction, numberOfEntries, type);
    }

    public bool GetLeaderBoardData(LeaderBoardType leaderBoardType, GenericSteamLeaderBoard.ResultsCallBack callBackFunction, int numberOfEntries, SelectionType selectionType = SelectionType.GLOBAL)
    {
        bool result = false;
        Steamworks.ELeaderboardDataRequest requestType;

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
        
        switch (leaderBoardType)
        {
            case LeaderBoardType.ALL_TIME:
                result = allTimeLeaderBoard.FetchLeaderBoardData(callBackFunction, numberOfEntries, requestType);
                break;
            case LeaderBoardType.WEEKLY:
                result = weeklyLeaderBoard.FetchLeaderBoardData(callBackFunction, numberOfEntries, requestType);
                break;
            case LeaderBoardType.DAILY:
                result = dailyLeaderBoard.FetchLeaderBoardData(callBackFunction, numberOfEntries, requestType);
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
        //result = result && weeklyLeaderBoard.UpdateLeaderBoard(score);
        //result = result && dailyLeaderBoard.UpdateLeaderBoard(score);
        return result;
    }
    #endregion
}
