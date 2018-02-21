using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;
using System;

public class SteamLeaderBoard : MonoBehaviour {

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

    enum ActionEnum
    {
        Update, Get
    }

    #region Private Variables
    //We store date as diff variables in the array dd,mm,yyyy,time and score. Based on the type of leaderboard, we then set the appropriate values as the score
    static private int MAX_DETAILS = 4;
    static private string LEADERBOARD_ALL_TIME = "LEADERBOARD_ALL_TIME";
    public delegate void ResultsCallBack(List<LeaderBoardVars> results);
    private CallResult<LeaderboardFindResult_t> leaderBoardFindResult;
    private CallResult<LeaderboardScoresDownloaded_t> leaderBoardScoresDownloaded;
    private ResultsCallBack callbackAction;
    private bool isWorking;
    private ActionEnum currentAction;
    #endregion

    #region Public Fields
    public bool IsWorking{
        get
        {
            return isWorking;
        }
    }
    #endregion

    #region Unity Lifecycle
    private void OnEnable()
    {
        leaderBoardFindResult = CallResult<LeaderboardFindResult_t>.Create(OnLeaderBoardFindResult);
        leaderBoardScoresDownloaded = CallResult<LeaderboardScoresDownloaded_t>.Create(OnLeaderBoardScoresDownloaded);
    }

    private void Start()
    {
        if (SteamManager.Initialized)
        {
            
        }
    }
    #endregion

    #region Public Methods
    public bool FetchAllTimeLeaderBoard(ResultsCallBack callbackAction)
    {
        bool result = false;
        if (SteamManager.Initialized && !isWorking)
        {
            SteamAPICall_t apiCall = SteamUserStats.FindOrCreateLeaderboard(LEADERBOARD_ALL_TIME, ELeaderboardSortMethod.k_ELeaderboardSortMethodDescending, ELeaderboardDisplayType.k_ELeaderboardDisplayTypeNumeric);
            if (leaderBoardFindResult.IsActive())
            {
                leaderBoardFindResult.Cancel();
                leaderBoardFindResult.Dispose();
            }
            leaderBoardFindResult.Set(apiCall);
            this.callbackAction = callbackAction;
            result = true;
            isWorking = true;
            currentAction = ActionEnum.Get;
        }
        return result;
    }

    public bool UpdateLeaderBoards(int score)
    {
        bool result = false;
        if (SteamManager.Initialized)
        {
            SteamAPICall_t apiCall = SteamUserStats.FindOrCreateLeaderboard(LEADERBOARD_ALL_TIME, ELeaderboardSortMethod.k_ELeaderboardSortMethodDescending, ELeaderboardDisplayType.k_ELeaderboardDisplayTypeNumeric);
            result = true;
            isWorking = true;
            currentAction = ActionEnum.Update;
        }
        return result;
    }
    #endregion

    #region Private Methods
    private void OnLeaderBoardFindResult(LeaderboardFindResult_t param, bool bIOFailure)
    {
        if(param.m_bLeaderboardFound != 1 || bIOFailure)
        {
            Debug.LogError("LeaderBoard not found");
            isWorking = false;
        }
        else
        {
            SteamLeaderboard_t leaderBoard = param.m_hSteamLeaderboard;
            if (currentAction == ActionEnum.Get)
            {
                SteamAPICall_t apiCall = SteamUserStats.DownloadLeaderboardEntries(leaderBoard, ELeaderboardDataRequest.k_ELeaderboardDataRequestGlobal, 1, 10);
                if (leaderBoardScoresDownloaded.IsActive())
                {
                    leaderBoardScoresDownloaded.Cancel();
                    leaderBoardScoresDownloaded.Dispose();
                }
                leaderBoardScoresDownloaded.Set(apiCall);
            }
            else
            {

            }
        }
    }

    private void OnLeaderBoardScoresDownloaded(LeaderboardScoresDownloaded_t param, bool bIOFailure)
    {
        if (bIOFailure)
        {
            Debug.LogError("Error getting leader board entries");
        }
        else
        {
            int count = param.m_cEntryCount;
            SteamLeaderboardEntries_t entries = param.m_hSteamLeaderboardEntries;
            List<LeaderBoardVars> results = new List<LeaderBoardVars>();
            for (int i = 0; i < count; i++)
            {
                LeaderboardEntry_t entry;
                int[] details = new int[MAX_DETAILS];
                if(SteamUserStats.GetDownloadedLeaderboardEntry(entries, i, out entry, details, MAX_DETAILS))
                {
                    LeaderBoardVars leaderBoardVars;
                    leaderBoardVars.rank = entry.m_nGlobalRank;
                    leaderBoardVars.score = entry.m_nScore;
                    leaderBoardVars.userID = entry.m_steamIDUser.ToString();
                    leaderBoardVars.day = details[0];
                    leaderBoardVars.month = details[1];
                    leaderBoardVars.year = details[2];
                    leaderBoardVars.time = details[3];
                    results.Add(leaderBoardVars);
                }
            }
            callbackAction(results);
            isWorking = false;
        }
    }
    #endregion

}
