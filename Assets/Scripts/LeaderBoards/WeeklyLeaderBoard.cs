using System.Collections;
using System.Collections.Generic;
using Steamworks;
using UnityEngine;

public class WeeklyLeaderBoard : GenericSteamLeaderBoard {

    private static string LEADER_BOARD_NAME = "WEEKLY_LEADERBOARD";

    // Use this for initialization
    void Start()
    {
        Initialize(LEADER_BOARD_NAME);
    }

    protected override LeaderBoardVars ExtractLeaderBoardVars(LeaderboardEntry_t entry, int[] details)
    {
        throw new System.NotImplementedException();
    }

    protected override int GetScoreAndDetails(int score, out int[] details)
    {
        System.DateTime today = System.DateTime.Now;
        details = new int[MAX_DETAILS];
        return score;
    }

    protected override List<LeaderBoardVars> SortEntries(List<LeaderBoardVars> results)
    {
        throw new System.NotImplementedException();
    }
}
