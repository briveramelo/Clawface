using System.Collections;
using System.Collections.Generic;
using Steamworks;
using UnityEngine;

public class WeeklyLeaderBoard : GenericSteamLeaderBoard {

    #region private variables
    private static string LEADER_BOARD_NAME = "WEEKLY_LEADERBOARD";
    private const int SECONDS_IN_A_WEEK = 60 * 60 * 24 * 7;
    #endregion

    #region unity lifecycle
    // Use this for initialization
    void Start()
    {
        Initialize(LEADER_BOARD_NAME);
    }
    #endregion

    #region protected methods
    //Extract the actual score from the composite score
    protected override LeaderBoardVars ExtractLeaderBoardVars(LeaderboardEntry_t entry, int[] details)
    {
        LeaderBoardVars leaderBoardVars = new LeaderBoardVars();
        int weekNumber = details[0];
        string savedScoreString = entry.m_nScore.ToString();
        savedScoreString = savedScoreString.Substring(weekNumber.ToString().Length);
        int actualScore = int.Parse(savedScoreString);
        leaderBoardVars.score = actualScore;
        leaderBoardVars.userID = SteamFriends.GetFriendPersonaName(entry.m_steamIDUser);
        leaderBoardVars.otherInfo = weekNumber;
        leaderBoardVars.rank = entry.m_nGlobalRank;
        return leaderBoardVars;
    }

    //The score is a composite of the week number and the actual score
    protected override int GetScoreAndDetails(int score, out int[] details)
    {
        int weekNumber = GetCurrentWeekSinceEpoch();
        details = new int[MAX_DETAILS];
        details[0] = weekNumber;
        string scoreString = weekNumber.ToString() + score.ToString();
        int finalScore = int.Parse(scoreString);
        return finalScore;
    }

    //Remove entries that are not in the current week
    protected override List<LeaderBoardVars> SortEntries(List<LeaderBoardVars> results)
    {        
        int currentWeekNumber = GetCurrentWeekSinceEpoch();
        List<LeaderBoardVars> sortedList = new List<LeaderBoardVars>();
        foreach(LeaderBoardVars vars in results)
        {
            if(vars.otherInfo == currentWeekNumber)
            {
                sortedList.Add(vars);
            }
        }
        return sortedList;
    }
    #endregion

    #region private methods
    private int GetCurrentWeekSinceEpoch()
    {
        long currentTimeSeconds = GetSecondsSinceEpoch();
        int currentWeekNumber = (int)(currentTimeSeconds / SECONDS_IN_A_WEEK);
        return currentWeekNumber;
    }
    #endregion
}
