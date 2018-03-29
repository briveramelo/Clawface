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
    #endregion

    #region Unity Lifecycle
    // Use this for initialization
    protected override void Start () {
        base.Start();
        allTimeLeaderBoard = GetComponent<AllTimeLeaderBoard>();
    }
    #endregion

    #region Public Methods
    public bool GetLeaderBoardData(string levelName, GenericSteamLeaderBoard.ResultsCallBack callBackFunction, int numberOfEntries, SelectionType selectionType = SelectionType.GLOBAL)
    {
        bool result = false;
        Steamworks.ELeaderboardDataRequest requestType;

        int startRange = 1;
        int endRange = numberOfEntries;

        switch (selectionType)
        {
            case SelectionType.FRIENDS:
                requestType = Steamworks.ELeaderboardDataRequest.k_ELeaderboardDataRequestFriends;
                break;
            case SelectionType.AROUND_USER:
                requestType = Steamworks.ELeaderboardDataRequest.k_ELeaderboardDataRequestGlobalAroundUser;
                startRange = -5;
                endRange = numberOfEntries - 5;
                if (endRange < 0)
                {
                    endRange = 1;
                }
                break;
            case SelectionType.GLOBAL:
            default:
                requestType = Steamworks.ELeaderboardDataRequest.k_ELeaderboardDataRequestGlobal;
                break;
        }

        result = allTimeLeaderBoard.FetchLeaderBoardData(levelName, callBackFunction, startRange, endRange, requestType);
        return result;
    }

    public bool UpdateScore(int score, string levelName)
    {
        bool result = false;
        string betaBranchName;
        if (SteamManager.Initialized && !Steamworks.SteamApps.GetCurrentBetaName(out betaBranchName, 128))
        {
            result = allTimeLeaderBoard.UpdateLeaderBoard(score, levelName);
        }
        return result;
    }
    #endregion
}
