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

    [SerializeField]
    private Scrollbar verticalScrollbar;

    [SerializeField]
    private float scrollSpeed;
    #endregion

    #region private fields
    private List<LeaderboardEntry> leaderBoardEntries;
    private bool allowInput;
    private string currentLevelName;
    private LeaderBoards.SelectionType currentSelectionType;
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

    #region unity lifecycle
    private void Update()
    {
        if (allowInput && InputManager.Instance.QueryAction(Strings.Input.UI.CANCEL, ButtonMode.DOWN))
        {
            OnPressBack();
        }

        if (allowInput && InputManager.Instance.QueryAxes(Strings.Input.Axes.LOOK).y < -0.3f)
        {
            verticalScrollbar.value -= scrollSpeed * Time.deltaTime;
        }
        else if (allowInput && InputManager.Instance.QueryAxes(Strings.Input.Axes.LOOK).y > 0.3f)
        {
            verticalScrollbar.value += scrollSpeed * Time.deltaTime;
        }
    }
    #endregion

    #region Public methods
    public LeaderboardsMenu() : base(Strings.MenuStrings.LEADER_BOARDS) {
        leaderBoardEntries = new List<LeaderboardEntry>();
    }

    public void OnPressBack()
    {
        foreach (LeaderboardEntry entry in leaderBoardEntries)
        {
            Destroy(entry.gameObject);
        }
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

    protected override void ShowStarted()
    {
        base.ShowStarted();
        allowInput = true;
    }

    protected override void HideStarted()
    {
        base.HideStarted();
        allowInput = false;
    }
    #endregion

    #region private methods
    private void GetLeaderboardEntries(LeaderBoards.SelectionType selectionType)
    {
        foreach (LeaderboardEntry entry in leaderBoardEntries)
        {
            entry.IsVisible(false);
        }       

        bool result = LeaderBoards.Instance.GetLeaderBoardData(currentLevelName, OnLeaderBoardEntriesReturned, maxEntries, selectionType);
        
        loadingObject.GetComponent<LoadingText>().SetError(result);
        loadingObject.SetActive(true);

    }

    private void OnLeaderBoardEntriesReturned(List<GenericSteamLeaderBoard.LeaderBoardVars> results, bool retry)
    {
        if (!retry)
        {
            int numberOfReusableEntries = leaderBoardEntries.Count;
            int numberOfResults = results.Count;
            if (numberOfReusableEntries < numberOfResults)
            {
                int numberOfNewObjects = numberOfResults - numberOfReusableEntries;
                for (int i = 0; i < numberOfNewObjects; i++)
                {
                    GameObject newObject = Instantiate(leaderBoardEntryPrefab);
                    newObject.transform.SetParent(entriesHolder);
                    leaderBoardEntries.Add(newObject.GetComponent<LeaderboardEntry>());
                }
            }

            numberOfReusableEntries = leaderBoardEntries.Count;
            loadingObject.SetActive(false);
            for (int i = 0; i < numberOfResults; i++)
            {
                GenericSteamLeaderBoard.LeaderBoardVars result = results[i];
                leaderBoardEntries[i].SetData(result.rank.ToString(), result.userID, result.score.ToString());
                leaderBoardEntries[i].IsVisible(true);
            }

            for (int i = numberOfResults; i < numberOfReusableEntries; i++)
            {
                leaderBoardEntries[i].IsVisible(false);
            }
        }
        else
        {
            GetLeaderboardEntries(currentSelectionType);
        }
    }

    internal void SetLevelName(string levelName)
    {
        currentLevelName = levelName;
    }
    #endregion
}
