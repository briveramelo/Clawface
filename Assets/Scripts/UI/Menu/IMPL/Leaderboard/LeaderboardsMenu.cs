using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardsMenu : Menu
{
    #region Serialized fields
    [SerializeField]
    private GameObject leaderBoardEntryPrefab;

    [SerializeField]
    private Transform entriesHolder;

    [SerializeField]
    private GameObject loadingObject;

    [SerializeField]
    private int maxEntries = 100;

    [SerializeField]
    private Button globalButton;

    [SerializeField]
    private Button friendButton;

    [SerializeField]
    private Button aroundUserButton;
    #endregion

    #region private fields
    private List<LeaderboardEntry> leaderBoardEntries;
    #endregion

    #region public fields
    public override Selectable InitialSelection
    {
        get
        {
            return globalButton;
        }
    }
    #endregion

    #region Public methods
    public LeaderboardsMenu() : base(Strings.MenuStrings.LEADER_BOARDS) {
        leaderBoardEntries = new List<LeaderboardEntry>();
    }

    public void OnPressBack()
    {
        leaderBoardEntries.Clear();
        MenuManager.Instance.DoTransition(Strings.MenuStrings.LevelEditor.LEVELSELECT_PLE_MENU, Transition.SHOW, new Effect[] { Effect.EXCLUSIVE });
    }

    public void GetGlobalLeaderboardEntries()
    {
        globalButton.Select();
        GetLeaderboardEntries(LeaderBoards.SelectionType.GLOBAL);
    }

    public void GetFriendsLeaderboardEntries()
    {
        friendButton.Select();
        GetLeaderboardEntries(LeaderBoards.SelectionType.FRIENDS);
    }

    public void GetAroundUserLeaderboardEntries()
    {
        aroundUserButton.Select();
        GetLeaderboardEntries(LeaderBoards.SelectionType.AROUND_USER);
    }
    #endregion

    #region protected methods
    protected override void DefaultHide(Transition transition, Effect[] effects)
    {
        Fade(transition, effects);
    }

    protected override void DefaultShow(Transition transition, Effect[] effects)
    {
        Fade(transition, effects);
        GetLeaderboardEntries(LeaderBoards.SelectionType.GLOBAL);
    }
    #endregion

    #region private methods
    private void GetLeaderboardEntries(LeaderBoards.SelectionType selectionType)
    {
        foreach (LeaderboardEntry entry in leaderBoardEntries)
        {
            entry.IsVisible(false);
        }
        loadingObject.SetActive(true);

        LeaderBoards.Instance.GetLeaderBoardData(OnLeaderBoardEntriesReturned, maxEntries, selectionType);
    }

    private void OnLeaderBoardEntriesReturned(List<GenericSteamLeaderBoard.LeaderBoardVars> results)
    {
        int numberOfReusableEntries = leaderBoardEntries.Count;
        int numberOfResults = results.Count;
        if (numberOfReusableEntries < numberOfResults)
        {
            int numberOfNewObjects = numberOfResults - numberOfReusableEntries;
            for(int i=0;i< numberOfNewObjects; i++)
            {
                GameObject newObject = Instantiate(leaderBoardEntryPrefab);
                newObject.transform.SetParent(entriesHolder);
                leaderBoardEntries.Add(newObject.GetComponent<LeaderboardEntry>());
            }
        }

        numberOfReusableEntries = leaderBoardEntries.Count;
        loadingObject.SetActive(false);
        for(int i=0;i< numberOfResults; i++)
        {
            GenericSteamLeaderBoard.LeaderBoardVars result = results[i];
            leaderBoardEntries[i].SetData(result.rank.ToString(), result.userID, result.score.ToString());
            leaderBoardEntries[i].IsVisible(true);
        }
                
        for(int i = numberOfResults; i < numberOfReusableEntries; i++)
        {
            leaderBoardEntries[i].IsVisible(false);
        }
    }
    #endregion
}
