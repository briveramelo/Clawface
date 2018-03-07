using System.Collections;
using System.Collections.Generic;
using Steamworks;
using UnityEngine;

public class DailyLeaderBoard : GenericSteamLeaderBoard
{

    #region private variables
    private static string LEADER_BOARD_NAME = "DAILY_LEADERBOARD";
    private const int SECONDS_IN_A_DAY = 60 * 60 * 24;
    #endregion

    #region unity lifecycle
    // Use this for initialization
    void Start()
    {
        Initialize(LEADER_BOARD_NAME);
    }
    #endregion

    #region protected methods
    protected override LeaderBoardVars ExtractLeaderBoardVars(LeaderboardEntry_t entry, int[] details)
    {
        LeaderBoardVars leaderBoardVars = new LeaderBoardVars();
        int dayNumber = details[0];
        string savedScoreString = entry.m_nScore.ToString();
        savedScoreString = savedScoreString.Substring(dayNumber.ToString().Length);
        int actualScore = int.Parse(savedScoreString);
        leaderBoardVars.score = actualScore;
        leaderBoardVars.userID = SteamFriends.GetFriendPersonaName(entry.m_steamIDUser);
        leaderBoardVars.otherInfo = dayNumber;
        leaderBoardVars.rank = entry.m_nGlobalRank;
        return leaderBoardVars;
    }

    protected override int GetScoreAndDetails(int score, out int[] details)
    {
        int dayNumber = GetCurrentDaySinceEpoch();
        details = new int[MAX_DETAILS];
        details[0] = dayNumber;
        string scoreString = dayNumber.ToString() + score.ToString();
        int finalScore = int.Parse(scoreString);
        return finalScore;
    }

    protected override List<LeaderBoardVars> SortEntries(List<LeaderBoardVars> results)
    {
        int dayNumber = GetCurrentDaySinceEpoch();
        List<LeaderBoardVars> sortedList = new List<LeaderBoardVars>();
        foreach (LeaderBoardVars vars in results)
        {
            if (vars.otherInfo == dayNumber)
            {
                sortedList.Add(vars);
            }
        }
        return sortedList;
    }
    #endregion

    #region private method
    private int GetCurrentDaySinceEpoch()
    {
        long currentSeconds = GetSecondsSinceEpoch();
        int currentDay = (int)(currentSeconds / SECONDS_IN_A_DAY);
        return currentDay;
    }
    #endregion
}
