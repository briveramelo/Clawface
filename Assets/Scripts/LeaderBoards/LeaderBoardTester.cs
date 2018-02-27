using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LeaderBoards))]
public class LeaderBoardTester : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.U))
        {
            int randomScore = UnityEngine.Random.Range(1, 1000);
            print("Updating score " + randomScore);
            bool result = GetComponent<LeaderBoards>().UpdateScore(randomScore);
            print("Update score result " + result);
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            bool result = GetComponent<LeaderBoards>().GetLeaderBoardData(LeaderBoards.LeaderBoardType.ALL_TIME, AllTimeCallBack, 100);
            print("Get all time score result " + result);
            result = GetComponent<LeaderBoards>().GetLeaderBoardData(LeaderBoards.LeaderBoardType.WEEKLY, WeeklyCallBack, 100);
            print("Get weekly score result " + result);
            result = GetComponent<LeaderBoards>().GetLeaderBoardData(LeaderBoards.LeaderBoardType.DAILY, DailyCallBack, 100);
            print("Get daily score result " + result);
        }
	}

    private void DailyCallBack(List<GenericSteamLeaderBoard.LeaderBoardVars> results)
    {
        print("Daily callback received");
        foreach (GenericSteamLeaderBoard.LeaderBoardVars var in results)
        {
            print("Username: " + var.userID + " Score: " + var.score);
        }
    }

    private void WeeklyCallBack(List<GenericSteamLeaderBoard.LeaderBoardVars> results)
    {
        print("Weekly callback received");
        foreach(GenericSteamLeaderBoard.LeaderBoardVars var in results)
        {
            print("Username: " + var.userID + " Score: " + var.score);
        }
    }

    private void AllTimeCallBack(List<GenericSteamLeaderBoard.LeaderBoardVars> results)
    {
        print("All time callback received");
        foreach (GenericSteamLeaderBoard.LeaderBoardVars var in results)
        {
            print("Username: " + var.userID + " Score: " + var.score);
        }
    }
}
