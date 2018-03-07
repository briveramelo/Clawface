﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;
using System;

public abstract class GenericSteamLeaderBoard : MonoBehaviour {
    
    #region Private Variables
    //We store the actual score value in the details array
    protected const int MAX_DETAILS = 1;    
    protected readonly DateTime epochStart = new DateTime(2018, 1, 1, 0, 0, 0, DateTimeKind.Utc);
    public delegate void ResultsCallBack(List<LeaderBoardVars> results);
    private CallResult<LeaderboardFindResult_t> leaderBoardFindResult;
    private CallResult<LeaderboardScoresDownloaded_t> leaderBoardScoresDownloaded;
    private ResultsCallBack callbackAction;
    private SteamLeaderboard_t leaderBoard;
    #endregion

    #region Public Fields
    private bool IsReady
    {
        get
        {
            return leaderBoard.m_SteamLeaderboard != 0;
        }
    }
    #endregion

    #region Unity Lifecycle
    private void OnEnable()
    {
        leaderBoardFindResult = CallResult<LeaderboardFindResult_t>.Create(OnLeaderBoardFindResult);
        leaderBoardScoresDownloaded = CallResult<LeaderboardScoresDownloaded_t>.Create(OnLeaderBoardScoresDownloaded);
        leaderBoard.m_SteamLeaderboard = 0;
    }
    #endregion

    #region Public Methods
    public bool FetchLeaderBoardData(ResultsCallBack callbackAction, int numberOfEntries)
    {
        bool result = false;
        if (SteamManager.Initialized && IsReady)
        {
            SteamAPICall_t apiCall = SteamUserStats.DownloadLeaderboardEntries(leaderBoard, ELeaderboardDataRequest.k_ELeaderboardDataRequestGlobal, 1, numberOfEntries);
            if (leaderBoardScoresDownloaded.IsActive())
            {
                leaderBoardScoresDownloaded.Cancel();
                leaderBoardScoresDownloaded.Dispose();
            }
            leaderBoardScoresDownloaded.Set(apiCall);
            this.callbackAction = callbackAction;
            result = true;
        }
        return result;
    }

    public bool UpdateLeaderBoard(int score)
    {
        bool result = false;
        if (SteamManager.Initialized && IsReady)
        {
            int[] details;
            score = GetScoreAndDetails(score, out details);
            SteamUserStats.UploadLeaderboardScore(leaderBoard, ELeaderboardUploadScoreMethod.k_ELeaderboardUploadScoreMethodKeepBest, score, details, MAX_DETAILS);
            result = true;
        }
        return result;
    }   
    #endregion

    #region Private Methods
    protected void Initialize(string leaderBoardName)
    {
        if (SteamManager.Initialized)
        {
            //Get leader board id
            SteamAPICall_t apiCall = SteamUserStats.FindOrCreateLeaderboard(leaderBoardName, ELeaderboardSortMethod.k_ELeaderboardSortMethodDescending, ELeaderboardDisplayType.k_ELeaderboardDisplayTypeNumeric);
            leaderBoardFindResult.Set(apiCall);
        }
    }

    private void OnLeaderBoardFindResult(LeaderboardFindResult_t param, bool bIOFailure)
    {
        if(param.m_bLeaderboardFound != 1 || bIOFailure)
        {
            Debug.LogError("LeaderBoard not found");
        }
        else
        {
            leaderBoard = param.m_hSteamLeaderboard;                        
        }
    }

    private void OnLeaderBoardScoresDownloaded(LeaderboardScoresDownloaded_t param, bool bIOFailure)
    {
        List<LeaderBoardVars> results = new List<LeaderBoardVars>();
        if (bIOFailure)
        {
            Debug.LogError("Error getting leader board entries");
        }
        else
        {
            int count = param.m_cEntryCount;
            SteamLeaderboardEntries_t entries = param.m_hSteamLeaderboardEntries;            
            for (int i = 0; i < count; i++)
            {
                LeaderboardEntry_t entry;
                int[] details = new int[MAX_DETAILS];
                if(SteamUserStats.GetDownloadedLeaderboardEntry(entries, i, out entry, details, MAX_DETAILS))
                {
                    LeaderBoardVars leaderBoardVars = ExtractLeaderBoardVars(entry, details);                    
                    results.Add(leaderBoardVars);
                }
            }
            results = SortEntries(results);            
        }
        callbackAction(results);
    }

    protected long GetSecondsSinceEpoch()
    {
        return (long)(DateTime.UtcNow - epochStart).TotalSeconds;
    }

    //Custom implementation based on the type of leader board
    protected abstract LeaderBoardVars ExtractLeaderBoardVars(LeaderboardEntry_t entry, int[] details);

    //Custom implementation based on the type of leader board
    protected abstract List<LeaderBoardVars> SortEntries(List<LeaderBoardVars> results);

    //Custom implementation based on the type of leader board
    protected abstract int GetScoreAndDetails(int score, out int[] details);
    #endregion

    #region Public structs
    public struct LeaderBoardVars
    {
        public string userID;
        public int score;
        public int otherInfo; //Use for sorting (if required)
    }
    #endregion

}
