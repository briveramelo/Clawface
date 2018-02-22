using System.Collections;
using System.Collections.Generic;
using Steamworks;
using UnityEngine;

public class AllTimeLeaderBoard : GenericSteamLeaderBoard {

    private static string LEADER_BOARD_NAME = "ALL_TIME_LEADERBOARD";

    // Use this for initialization
    void Start () {
        Initialize(LEADER_BOARD_NAME);
	}

    protected override LeaderBoardVars ExtractLeaderBoardVars(LeaderboardEntry_t entry, int[] details)
    {
        LeaderBoardVars leaderBoardVars;
        leaderBoardVars.rank = entry.m_nGlobalRank;
        leaderBoardVars.score = entry.m_nScore;
        leaderBoardVars.userID = SteamFriends.GetFriendPersonaName(entry.m_steamIDUser);
        leaderBoardVars.day = details[0];
        leaderBoardVars.month = details[1];
        leaderBoardVars.year = details[2];
        leaderBoardVars.time = details[3];
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
}
