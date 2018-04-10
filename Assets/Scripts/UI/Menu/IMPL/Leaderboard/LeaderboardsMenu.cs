using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ModMan;
public class LeaderboardsMenu : Menu
{
    #region Serialized fields
    [SerializeField] private LoadingText loadingText;
    [SerializeField] private GameObject leaderBoardEntryPrefab, loadingObject;
    [SerializeField] private Transform entriesHolder;    

    [SerializeField] private Button globalButton, friendButton, aroundUserButton;
    [SerializeField] private Scrollbar verticalScrollbar;

    [SerializeField] private SelectorToggleGroup selectorToggleGroup;

    [SerializeField] private Color playerColor, standardColor;
    [SerializeField] private int maxEntries = 100;
    [SerializeField] private float joystickMaxScrollSpeed, mouseMaxScrollSpeed;
    #endregion

    #region private fields
    private List<LeaderboardEntry> leaderBoardEntries=new List<LeaderboardEntry>();    
    private string currentLevelName;
    private LeaderBoards.SelectionType currentSelectionType;

    private int SelectedFilterToggle {
        get { return Mathf.Clamp((int)currentSelectionType, 0, selectorToggleGroup.SelectorTogglesCount); }
        set { currentSelectionType = (LeaderBoards.SelectionType)Mathf.Repeat(value, selectorToggleGroup.SelectorTogglesCount); }
    }
    private Dictionary<LeaderBoards.SelectionType, Selectable> buttons = new Dictionary<LeaderBoards.SelectionType, Selectable>();
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
    protected override void Awake() {
        base.Awake();
        buttons = new Dictionary<LeaderBoards.SelectionType, Selectable>() {
            {LeaderBoards.SelectionType.GLOBAL, globalButton },
            {LeaderBoards.SelectionType.FRIENDS, friendButton},
            {LeaderBoards.SelectionType.AROUND_USER, aroundUserButton},
        };
        loadingText.SetOnToggleWaitingForConfirmation((value)=> { ToggleFilterSelectability(!value, currentSelectionType); });
    }

    private void Update()
    {
        if (allowInput) {
            if (InputManager.Instance.QueryAction(Strings.Input.UI.CANCEL, ButtonMode.DOWN))
            {
                OnPressBack();
            }

            CheckToMoveFilter();
            CheckToMoveScrollbar();
        }
    }

    void CheckToMoveScrollbar() {
        const float lookThreshold = 0.3f;
        const float scrollThreshold = 0.1f;
        float joystickY = Mathf.Clamp(InputManager.Instance.QueryAxes(Strings.Input.Axes.LOOK).y, -1f, 1f);
        float mouseDeltaY = Mathf.Clamp(Input.mouseScrollDelta.y, -1f, 1f);

        if (Mathf.Abs(joystickY) > lookThreshold) {
            float extraMultiplier = Mathf.Abs(InputManager.Instance.QueryAxes(Strings.Input.UI.NAVIGATION).y) > lookThreshold ? 2f : 1f;
            MoveScrollBar(joystickMaxScrollSpeed * joystickY * extraMultiplier);
        }
        else if (Mathf.Abs(mouseDeltaY) > scrollThreshold) {
            MoveScrollBar(mouseMaxScrollSpeed * mouseDeltaY);
        }
    }
    void MoveScrollBar(float speed) {
        verticalScrollbar.value += speed * Time.deltaTime;
    }
    void ToggleFilterSelectability(bool enabled, LeaderBoards.SelectionType selectedType) {
        buttons.ForEach((type, button) => {
            if ((selectedType != type && !enabled) || enabled) {
                button.interactable = enabled;
            }
        });
    }    

    private void CheckToMoveFilter() {
        //TODO set this up work with Strings.Input.UI
        //Strings.Input.UI.TAB_LEFT
        //Strings.Input.UI.TAB_RIGHT
        bool leftButtonPressed = InputManager.Instance.QueryAction(Strings.Input.Actions.FIRE_LEFT, ButtonMode.DOWN);
        bool rightBumperPressed = InputManager.Instance.QueryAction(Strings.Input.Actions.FIRE_RIGHT, ButtonMode.DOWN);
        bool mouseClicked = Input.GetMouseButtonDown(MouseButtons.LEFT) || Input.GetMouseButtonDown(MouseButtons.RIGHT) || Input.GetMouseButtonDown(MouseButtons.MIDDLE);
        if (!mouseClicked && (leftButtonPressed || rightBumperPressed)) {
            if (leftButtonPressed) {
                SelectedFilterToggle--;
            }
            else {
                SelectedFilterToggle++;
            }
            GetSelectedLeaderboardEntries();
        }
    }

    #endregion

    #region Public methods
    public LeaderboardsMenu() : base(Strings.MenuStrings.LEADER_BOARDS) { }

    public void OnPressBack()
    {
        foreach (LeaderboardEntry entry in leaderBoardEntries)
        {
            Destroy(entry.gameObject);
        }
        leaderBoardEntries.Clear();
        StopAllCoroutines();
        MenuManager.Instance.DoTransition(Strings.MenuStrings.LevelEditor.LEVELSELECT_PLE_MENU, Transition.SHOW, new Effect[] { Effect.EXCLUSIVE });
    }

    private void GetSelectedLeaderboardEntries() {
        SFXManager.Instance.Play(SFXType.UI_Click);
        switch ((LeaderBoards.SelectionType)SelectedFilterToggle) {
            case LeaderBoards.SelectionType.GLOBAL:  GetGlobalLeaderboardEntries(); break;
            case LeaderBoards.SelectionType.FRIENDS: GetFriendsLeaderboardEntries(); break;
            case LeaderBoards.SelectionType.AROUND_USER: GetAroundUserLeaderboardEntries(); break;
        }
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
    public override MenuType ThisMenuType { get { return MenuType.Leaderboards; } }

    protected override void ShowStarted()
    {
        base.ShowStarted();
        selectorToggleGroup.HandleGroupSelection(0);
        if (MenuManager.Instance.MouseMode) {
            GetLeaderboardEntries(LeaderBoards.SelectionType.GLOBAL);
        }
    }

    #endregion

    #region private methods
    private void GetLeaderboardEntries(LeaderBoards.SelectionType selectionType)
    {
        currentSelectionType = selectionType;
        foreach (LeaderboardEntry entry in leaderBoardEntries)
        {
            entry.IsVisible(false);
        }
        if (!loadingText.IsWaitingForConfirmation) {
            loadingText.IsWaitingForConfirmation = true;
            bool result = LeaderBoards.Instance.GetLeaderBoardData(currentLevelName, OnLeaderBoardEntriesReturned, maxEntries, selectionType);
            loadingText.SetSuccess(result);
            loadingObject.SetActive(true);

            verticalScrollbar.value = 1f;
        }
    }

    private void OnLeaderBoardEntriesReturned(List<GenericSteamLeaderBoard.LeaderBoardVars> results, bool retry)
    {
        loadingText.IsWaitingForConfirmation = false;
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
                    newObject.transform.localScale = Vector3.one;
                    leaderBoardEntries.Add(newObject.GetComponent<LeaderboardEntry>());
                }
            }

            numberOfReusableEntries = leaderBoardEntries.Count;
            loadingObject.SetActive(false);
            string playerName = Steamworks.SteamFriends.GetPersonaName();            
            for (int i = 0; i < numberOfResults; i++)
            {
                GenericSteamLeaderBoard.LeaderBoardVars result = results[i];
                LeaderboardEntry entry = leaderBoardEntries[i];
                Color entryColor = result.userID == playerName ? playerColor : standardColor;
                entry.SetTextColor(entryColor);
                entry.SetData(string.Format("{0}{1}",result.rank.ToCommaSeparated(), "."), result.userID, result.score.ToCommaSeparated());
                entry.IsVisible(true);
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
        MEC.Timing.RunCoroutine(DelayPositioningVerticalScrollbar(), CoroutineName);
    }

    IEnumerator<float> DelayPositioningVerticalScrollbar() {
        yield return 0f;
        verticalScrollbar.value = 1f;
    }

    internal void SetLevelName(string levelName)
    {
        currentLevelName = levelName;
    }
    #endregion
}
