using System.Collections;
using System.Collections.Generic;
using Steamworks;
using UnityEngine;

public class AllTimeLeaderBoard : GenericSteamLeaderBoard {

    #region Private variables
    private static string LEADER_BOARD_NAME = "ALL_TIME_LEADERBOARD";
    #endregion

    #region Unity lifecycle
    // Use this for initialization
    void Start () {
        Initialize(LEADER_BOARD_NAME);
	}
    #endregion

    #region protected methods
    protected override LeaderBoardVars ExtractLeaderBoardVars(LeaderboardEntry_t entry, int[] details)
    {
        LeaderBoardVars leaderBoardVars = new LeaderBoardVars();
        leaderBoardVars.score = entry.m_nScore;
        leaderBoardVars.userID = SteamFriends.GetFriendPersonaName(entry.m_steamIDUser);
        leaderBoardVars.otherInfo = details[0];
        leaderBoardVars.rank = entry.m_nGlobalRank;
        return leaderBoardVars;
    }

    //Nothing to change
    protected override int GetScoreAndDetails(int score, out int[] details)
    {
        details = new int[MAX_DETAILS];
        return score;
    }

    //Nothing to sort
    protected override List<LeaderBoardVars> SortEntries(List<LeaderBoardVars> results)
    {
        return results;
    }
    #endregion
}
