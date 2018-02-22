using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;
using System;

public abstract class SteamLeaderBoard : MonoBehaviour {
    
    #region Private Variables
    //We store date as diff variables in the array dd,mm,yyyy and time. Based on the type of leaderboard, we then set the appropriate values as the score
    private const int MAX_DETAILS = 4;    
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

    private void Start()
    {
       
    }
    #endregion

    #region Public Methods
    public bool FetchLeaderBoardData(ResultsCallBack callbackAction)
    {
        bool result = false;
        if (SteamManager.Initialized && IsReady)
        {
            SteamAPICall_t apiCall = SteamUserStats.DownloadLeaderboardEntries(leaderBoard, ELeaderboardDataRequest.k_ELeaderboardDataRequestGlobal, 1, 10);
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

    public bool UpdateLeaderBoards(int score)
    {
        bool result = false;
        if (SteamManager.Initialized && IsReady)
        {
            int[] details = new int[MAX_DETAILS];
            GetScoreAndDetails(out score, out details);
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
                    //leaderBoardVars.rank = entry.m_nGlobalRank;
                    //leaderBoardVars.score = entry.m_nScore;
                    //leaderBoardVars.userID = entry.m_steamIDUser.ToString();
                    //leaderBoardVars.day = details[0];
                    //leaderBoardVars.month = details[1];
                    //leaderBoardVars.year = details[2];
                    //leaderBoardVars.time = details[3];
                    results.Add(leaderBoardVars);
                }
            }
            results = SortEntries(results);            
        }
        callbackAction(results);
    }

    //Custom implementation based on the type of leader board
    protected abstract LeaderBoardVars ExtractLeaderBoardVars(LeaderboardEntry_t entry, int[] details);

    //Custom implementation based on the type of leader board
    protected abstract List<LeaderBoardVars> SortEntries(List<LeaderBoardVars> results);

    //Custom implementation based on the type of leader board
    protected abstract void GetScoreAndDetails(out int score, out int[] details);
    #endregion

    #region Public structs
    public struct LeaderBoardVars
    {
        public string userID;
        public int rank;
        public int score;
        public int day;
        public int month;
        public int year;
        public int time;
    }
    #endregion

}
