using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;
using System;

public abstract class GenericSteamLeaderBoard : MonoBehaviour {
    
    #region Private Variables
    //Size of the details array
    protected const int MAX_DETAILS = 1;    
    protected readonly DateTime epochStart = new DateTime(2018, 1, 1, 0, 0, 0, DateTimeKind.Utc);
    public delegate void ResultsCallBack(List<LeaderBoardVars> results, bool retry);
    public delegate void UpdateCallBack(bool retry);
    private CallResult<LeaderboardFindResult_t> leaderBoardFindResult;
    private CallResult<LeaderboardScoresDownloaded_t> leaderBoardScoresDownloaded;
    private ResultsCallBack callbackAction;
    private Dictionary<string, SteamLeaderboard_t> leaderBoards;
    private int currentScore;
    private byte actionType;
    #endregion

    #region Public Fields    
    #endregion

    #region Unity Lifecycle
    private void OnEnable()
    {
        leaderBoardFindResult = CallResult<LeaderboardFindResult_t>.Create(OnLeaderBoardFindResult);
        leaderBoardScoresDownloaded = CallResult<LeaderboardScoresDownloaded_t>.Create(OnLeaderBoardScoresDownloaded);
        leaderBoards = new Dictionary<string, SteamLeaderboard_t>();
    }
    #endregion

    #region Public Methods
    public bool FetchLeaderBoardData(string leaderBoardName, ResultsCallBack callbackAction, int startRange, int endRange, ELeaderboardDataRequest requestType)
    {
        bool result = false;
        this.callbackAction = callbackAction;
        SteamLeaderboard_t leaderBoard;
        if (SteamManager.Initialized)
        {  
            if (leaderBoards.TryGetValue(leaderBoardName, out leaderBoard))
            {
                SteamAPICall_t apiCall = SteamUserStats.DownloadLeaderboardEntries(leaderBoard, requestType, startRange, endRange);
                if (leaderBoardScoresDownloaded.IsActive())
                {
                    leaderBoardScoresDownloaded.Cancel();
                    leaderBoardScoresDownloaded.Dispose();
                }
                leaderBoardScoresDownloaded.Set(apiCall);
                result = true;
            }
            else
            {
                //Get or create the leader board
                actionType = 1;
                result = FindOrCreateLeaderboard(leaderBoardName);
            }
        }
        return result;
    }

    public bool UpdateLeaderBoard(int score, string leaderBoardName)
    {
        bool result = false;
        SteamLeaderboard_t leaderBoard;
        if (SteamManager.Initialized)
        {            
            if (leaderBoards.TryGetValue(leaderBoardName, out leaderBoard))
            {

                int[] details;
                score = GetScoreAndDetails(score, out details);
                SteamUserStats.UploadLeaderboardScore(leaderBoard, ELeaderboardUploadScoreMethod.k_ELeaderboardUploadScoreMethodKeepBest, score, details, MAX_DETAILS);
                result = true;
                //Reset action type
                actionType = 0;
            }
            else
            {
                //Get or create the leader board if not already called (checking action type to avoid infinite loops)
                if (actionType != 2)
                {
                    actionType = 2;
                    currentScore = score;
                    result = FindOrCreateLeaderboard(leaderBoardName);
                }
            }
        }
        return result;
    }   
    #endregion

    #region Private Methods
    protected bool FindOrCreateLeaderboard(string leaderBoardName)
    {
        bool result = false;
        if (SteamManager.Initialized)
        {
            //Get leader board id
            SteamAPICall_t apiCall = SteamUserStats.FindOrCreateLeaderboard(leaderBoardName, ELeaderboardSortMethod.k_ELeaderboardSortMethodDescending, ELeaderboardDisplayType.k_ELeaderboardDisplayTypeNumeric);
            if (leaderBoardFindResult.IsActive())
            {
                leaderBoardFindResult.Cancel();
                leaderBoardFindResult.Dispose();
            }
            leaderBoardFindResult.Set(apiCall);
            result = true;
        }
        return result;
    }

    private void OnLeaderBoardFindResult(LeaderboardFindResult_t param, bool bIOFailure)
    {
        if(param.m_bLeaderboardFound != 1 || bIOFailure)
        {
            Debug.LogError("LeaderBoard not found");
        }
        else
        {
            SteamLeaderboard_t leaderBoard = param.m_hSteamLeaderboard;
            string leaderBoardName = SteamUserStats.GetLeaderboardName(leaderBoard);
            leaderBoards.Add(leaderBoardName, leaderBoard);
            if (actionType == 1)
            {                
                callbackAction(new List<LeaderBoardVars>(), true);
            }
            else if(actionType == 2)
            {
                UpdateLeaderBoard(currentScore, leaderBoardName);
            }
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
        callbackAction(results, false);
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
        public int rank;
        public int otherInfo; //Use for sorting (if required)
    }
    #endregion

}
