using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ModMan;
using PlayerLevelEditor;
using System.Linq;
using System;
 
public class PLELevelSelectMenu : PlayerLevelEditorMenu {

    public PLELevelSelectMenu() : base(Strings.MenuStrings.LevelEditor.LEVELSELECT_PLE_MENU) { }

    [HideInInspector] public string backMenuTarget;

    #region Fields (Serialized)
    [SerializeField] private LevelDataManager levelDataManager;
    [SerializeField] private Transform levelContentParent;
    [SerializeField] private GameObject levelUIPrefab;
    [SerializeField] private Text selectedLevelNameText, selectedLevelDescriptionText;
    [SerializeField] private Image selectedLevelImage, selectedLevelFavoriteIcon;
    [SerializeField] private InputField searchField;
    [SerializeField] private GridLayoutGroup gridLayoutGroup;
    [SerializeField] private Button backButton, deleteButton, favoriteButton, leaderboardButton, loadButton;
    [SerializeField] private Toggle allToggle, downloadedToggle, userToggle, favoriteToggle;
    [SerializeField] private Scrollbar levelScrollBar;
    [SerializeField] private GameObject plePrefab;
    [SerializeField] private Sprite hathosBigPreview;
    [SerializeField] private List<MemorableTransform> preMadeLevelUITransforms;
    [SerializeField] private RectOffset gridLayoutConfigWithHathos, gridLayoutConfigWithoutHathos;
    [SerializeField] private DiffAnim scrollSlideAnim;
    [SerializeField] private AbsAnim selectLevelAnim;
    #endregion

    #region Fields (Private)
    private DataSave ActiveDataSave { get { return DataPersister.ActiveDataSave; } }
    private List<LevelData> Levels { get { return ActiveDataSave.levelDatas; } }
    private List<LevelUI> levelUIs = new List<LevelUI>();
    private List<LevelUI> DisplayedLevelUIs { get { return levelUIs.Where(levelUI=>levelUI.gameObject.activeInHierarchy).ToList(); } }
    private int selectedLevelIndex = 0;
    private int SelectedLevelIndex { get { return Mathf.Clamp(selectedLevelIndex, 0, levelUIs.Count-1); } set { selectedLevelIndex = value; } }

    private bool IsHathosLevelDisplayed { get { return NumHathosLevels > 0 && levelUIs.Count>0 && levelUIs[0].gameObject.activeInHierarchy; } }
    private bool IsHathosLevelSelected { get { return !SceneTracker.IsCurrentSceneEditor && SelectedLevelIndex == 0; } }
    private int NumHathosLevels { get { return !SceneTracker.IsCurrentSceneEditor ? preMadeLevelUITransforms.Count : 0; } }
    private LevelSelectFilterType activeFilter;
    private Predicate<LevelUI> levelUISelectionMatch;
    public LevelUI SelectedLevelUI {
        get {
            if (levelUIs.Count > 0) {
                return levelUIs[SelectedLevelIndex];
            }
            return null;
        }
    }
    private const int levelsPerRow = 4;
    private LevelUI lastSelectedLevel;
    private List<Toggle> filterToggles = new List<Toggle>();
    private int SelectedFilterToggle {
        get {
            return Mathf.Clamp(selectedFilterIndex, 0, filterToggles.Count);
        }
        set {
            if (value >= filterToggles.Count) {
                value = 0;
            }
            else if (value<0) {
                value = filterToggles.Count;
            }
            selectedFilterIndex = value;
            activeFilter = (LevelSelectFilterType)selectedFilterIndex;
        }
    }
    private List<Selectable> levelItemSelectables = new List<Selectable>();
    private int selectedFilterIndex;
    private bool IsLastLevelShowing { get { return lastSelectedLevel == null ? false : (lastSelectedLevel.gameObject.activeInHierarchy); } }
    private string ScrollCoroutineName { get { return coroutineName + "Scroll"; } }
    private string PulseCoroutineName { get { return coroutineName + "Pulse"; } }
    #endregion



    #region Unity Lifecyle
    protected override void Start() {
        base.Start();
        filterToggles = new List<Toggle>() {
            allToggle, downloadedToggle, userToggle, favoriteToggle
        };
        levelItemSelectables = new List<Selectable>() {
            backButton, deleteButton, favoriteButton, leaderboardButton, loadButton
        };
        scrollSlideAnim.OnUpdate = (val) => { levelScrollBar.value = val; };
        levelUISelectionMatch = item => CurrentEventSystemGameObject == item.selectable.gameObject;
    }

    protected override void Update() {
        base.Update();
        if (allowInput) {
            CheckToSelectNewLevelFromController();
            CheckToMoveFilter();
        }
    }    
    #endregion

    #region Public Interface
    public void FilterLevels(int filterType) {
        SelectedFilterToggle = filterType;
        for (int i = 0; i < levelUIs.Count; i++) {
            LevelData levelData = levelUIs[i].levelData;
            bool shouldShow = levelData.IsValidLevel(activeFilter);
            levelUIs[i].gameObject.SetActive(shouldShow);
            if (!shouldShow) {
                CheckToNullLastSelected(levelUIs[i].selectable);
            }
        }
        SetButtonNavigation();
        PositionLevelsInSceneContext();
        StartCoroutine(ResortImagePositions());
    }

    public void OnSearchChange() {
        string searchTerm = searchField.text.ToLowerInvariant();
        List<string> searchTerms = searchTerm.Split(' ').ToList();

        for (int i = 0; i < levelUIs.Count; i++) {
            LevelData levelData = levelUIs[i].levelData;
            string levelDataName = levelData.name.ToLowerInvariant();
            string levelDataDescription = levelData.description.ToLowerInvariant();
            bool shouldShow =
                (string.IsNullOrEmpty(searchTerm) ||
                searchTerm.Length == 0 ||
                searchTerms.All(term => { return levelDataName.Contains(term) || levelDataDescription.Contains(term); }));

            levelUIs[i].gameObject.SetActive(shouldShow);
            if (!shouldShow) {
                CheckToNullLastSelected(levelUIs[i].selectable);
            }
        }
        FilterLevels((int)activeFilter);
    }

    public void LoadLevel() {
        ActiveDataSave.SelectedLevelIndex = SelectedLevelIndex - NumHathosLevels;

        if (SceneTracker.IsCurrentSceneEditor) {
            levelDataManager.LoadSelectedLevel();
            levelEditor.SwitchToMenu(PLEMenu.FLOOR);
        }
        else {
            string loadMenuName = Strings.MenuStrings.LOAD;
            WeaponSelectMenu weaponSelectMenu = (WeaponSelectMenu)MenuManager.Instance.GetMenuByName(Strings.MenuStrings.WEAPON_SELECT);
            weaponSelectMenu.DefineNavigation(Strings.MenuStrings.LevelEditor.LEVELSELECT_PLE_MENU, loadMenuName);

            LoadMenu loadMenu = (LoadMenu)MenuManager.Instance.GetMenuByName(loadMenuName);
            loadMenu.SetNavigation(IsHathosLevelSelected ? Strings.Scenes.ScenePaths.Arena : Strings.Scenes.ScenePaths.PlayerLevels);


            MenuManager.Instance.DoTransition(weaponSelectMenu, Transition.SHOW, new Effect[] { Effect.EXCLUSIVE });
        }
    }

    public void SelectLevel(LevelUI levelUI) {
        SelectLevel(levelUI.LevelIndex);
    }

    public void SelectLevel(int levelIndex) {
        MEC.Timing.KillCoroutines(PulseCoroutineName);
        if (lastSelectedLevel!=null) {
            lastSelectedLevel.ScaleLevelUISize(1f);
        }

        SelectedLevelIndex = levelIndex;
        selectLevelAnim.OnUpdate = SelectedLevelUI.ScaleLevelUISize;
        selectLevelAnim.Animate(PulseCoroutineName);

        SelectedLevelUI.selectable.Select();
        lastSelectedLevel = SelectedLevelUI;

        LevelData selectedLevel = SelectedLevelUI.levelData;
        selectedLevelNameText.text = selectedLevel.name;
        selectedLevelDescriptionText.text = selectedLevel.description;
        Sprite spriteToShow = selectedLevel.isHathosLevel ? hathosBigPreview : selectedLevel.MySprite;
        selectedLevelImage.sprite = spriteToShow;

        levelUIs.ForEach(levelUI => { levelUI.OnGroupSelectChanged(levelIndex); });
        selectedLevelFavoriteIcon.enabled = selectedLevel.isFavorite;
        SetButtonsInteractabilityAndNavigation(DisplayedLevelUIs);

        AlignScrollbar();
    }

    public override void BackAction() {
        if (SceneTracker.IsCurrentSceneEditor) {
            base.BackAction();
        }
        else {
            MenuManager.Instance.DoTransition(backMenuTarget, Transition.SHOW, new Effect[] { Effect.EXCLUSIVE });
        }
    }

    public void ClearAndGenerateLevelUI() {
        preMadeLevelUITransforms.ForEach(memorableTransform => { memorableTransform.transform.SetParent(null); });
        levelContentParent.DestroyAllChildren();
        preMadeLevelUITransforms.ForEach(memorableTransform => {
            memorableTransform.transform.SetParent(levelContentParent);
            memorableTransform.ResetTransform();
        });
        preMadeLevelUITransforms.ForEach(memorableTransform => { memorableTransform.transform.gameObject.SetActive(!SceneTracker.IsCurrentSceneEditor); });
        StartCoroutine(DelayGeneration());
    }

    public void OnShowLeaderboard() {
        if (SelectedLevelUI!=null) {
            string levelName = SelectedLevelUI.levelData.UniqueSteamName;
            MenuManager.Instance.DoTransition(Strings.MenuStrings.LEADER_BOARDS, Transition.SHOW, new Effect[] { Effect.EXCLUSIVE });
            ((LeaderboardsMenu)MenuManager.Instance.GetMenuByName(Strings.MenuStrings.LEADER_BOARDS)).SetLevelName(levelName);
        }
    }

    public void ToggleSelectedLevelIsFavorite() {
        LevelUI selectedLevelUI = levelUIs[SelectedLevelIndex];
        selectedLevelUI.ToggleIsFavorite();
        selectedLevelFavoriteIcon.enabled = selectedLevelUI.levelData.isFavorite;
    }

    public void DeleteSelectedLevel() {
        levelDataManager.DeleteSelectedLevel();
        SetButtonsInteractabilityAndNavigation(DisplayedLevelUIs);
    }
    #endregion

    #region Protected Interface
    protected override void ShowStarted() {
        base.ShowStarted();
        SelectedFilterToggle = (int)LevelSelectFilterType.All;
        searchField.text = "";
        InitializeHathosLevels();
        ClearAndGenerateLevelUI();
        if (SceneTracker.IsCurrentSceneEditor) {
            deleteButton.gameObject.SetActive(true);
        }
        else {
            deleteButton.gameObject.SetActive(false);
        }
    }
    #endregion

    #region Private Interface 
    private void CheckToMoveFilter() {
        bool leftButtonPressed = InputManager.Instance.QueryAction(Strings.Input.Actions.FIRE_LEFT, ButtonMode.DOWN);
        bool rightBumperPressed = InputManager.Instance.QueryAction(Strings.Input.Actions.FIRE_RIGHT, ButtonMode.DOWN);
        if (leftButtonPressed || rightBumperPressed) {
            if (leftButtonPressed) {
                SelectedFilterToggle--;
            }
            else {
                SelectedFilterToggle++;
            }
            FilterLevels(SelectedFilterToggle);
            CurrentEventSystem.SetSelectedGameObject(filterToggles[SelectedFilterToggle].gameObject);
        }
    }


    private void CheckToSelectNewLevelFromController() {
        bool isLevelUISelected = SelectedLevelUI != null && levelUIs.Exists(levelUISelectionMatch);
        if (isLevelUISelected && SelectedLevelUI.selectable.gameObject != CurrentEventSystemGameObject && CurrentEventSystemGameObject.transform.parent.GetComponent<LevelUI>()) {
            LevelUI newlySelectedLevelUI = levelUIs.Find(levelUISelectionMatch);
            SelectLevel(newlySelectedLevelUI.LevelIndex);
        }

        if (InputManager.Instance.QueryAction(Strings.Input.UI.SUBMIT, ButtonMode.DOWN)) {
            if (isLevelUISelected) {
                CurrentEventSystem.SetSelectedGameObject(loadButton.gameObject);
            }
        }

        TryDeselectSearchField();
    }

    private void TryDeselectSearchField() {
        if (CurrentEventSystemGameObject == searchField.gameObject) {
            float yInput = InputManager.Instance.QueryAxes(Strings.Input.UI.NAVIGATION).y;
            if (Mathf.Abs(yInput) > .5f) {
                bool goUp = yInput > 0f;
                List<LevelUI> displayedLevelUIs = DisplayedLevelUIs;
                GameObject downSelection = displayedLevelUIs.Count > 0 ? displayedLevelUIs[0].selectable.gameObject : loadButton.gameObject;
                GameObject selectedObject = goUp ? allToggle.gameObject : downSelection;
                CurrentEventSystem.SetSelectedGameObject(selectedObject);
            }
        }
    }

    #region Button Navigation
    private void SetButtonNavigation() {
        Selectable filterButtonDownSelectable = null;
        List<LevelUI> displayedLevelUIs = DisplayedLevelUIs;
        if (IsHathosLevelDisplayed) {
            SetHathosLevelNavigation(displayedLevelUIs);
            filterButtonDownSelectable = (lastSelectedLevel ?? displayedLevelUIs[0]).selectable;
        }
        else {
            filterButtonDownSelectable = displayedLevelUIs.Count > 0 ? (lastSelectedLevel ?? displayedLevelUIs[0]).selectable : loadButton;
        }

        SetPlayerLevelNavigation(displayedLevelUIs);
        SetFilterButtonsNavigation(filterButtonDownSelectable);
        SetButtonsInteractabilityAndNavigation(displayedLevelUIs);
    }

    void SetPlayerLevelNavigation(List<LevelUI> displayedLevelUIs) {
        SetMainBodyofLevelUINavigation(displayedLevelUIs);
        SetLastLevelNavigation(displayedLevelUIs);
    }

    void SetHathosLevelNavigation(List<LevelUI> displayedLevelUIs) {
        LevelUI nextDisplayedLevelUI = null;
        if (displayedLevelUIs.Count > 1) {
            nextDisplayedLevelUI = displayedLevelUIs[1];
        }


        Selectable hathosDownTarget = levelUIs.Count > 1 ? nextDisplayedLevelUI.selectable : loadButton;
        SetNavigation(levelUIs[0].selectable, hathosDownTarget, SelectableDirection.Down, SelectableDirection.Right);
    }

    private void SetMainBodyofLevelUINavigation(List<LevelUI> displayedLevelUIs) {
        Selectable topRowUpSelectable = IsHathosLevelDisplayed ? levelUIs[0].selectable : downloadedToggle;
        int startIndex = IsHathosLevelDisplayed ? 1 : 0;
        int lastIndex = displayedLevelUIs.Count - 1;
        for (int i = startIndex; i < displayedLevelUIs.Count; i++) {
            if (!displayedLevelUIs.TrySetNavigation(i, i - 4, SelectableDirection.Up)) {
                SetNavigation(displayedLevelUIs[i].selectable, topRowUpSelectable, SelectableDirection.Up);
            }

            displayedLevelUIs.TrySetNavigation(i, i - 1, SelectableDirection.Left);
            displayedLevelUIs.TrySetNavigation(i, i + 1, SelectableDirection.Right);
            
            if (!displayedLevelUIs.TrySetNavigation(i, i + 4, SelectableDirection.Down)) {
                displayedLevelUIs.TrySetNavigation(i, lastIndex, SelectableDirection.Down);
            }
        }
    }

    private void SetLastLevelNavigation(List<LevelUI> displayedLevelUIs) {
        if (displayedLevelUIs.Count>0) {
            SetNavigation(displayedLevelUIs[displayedLevelUIs.Count - 1].selectable, loadButton, SelectableDirection.Down, SelectableDirection.Right);
        }
    }

    private void SetFilterButtonsNavigation(Selectable filterButtonDownSelectable) {
        SetNavigation(allToggle, filterButtonDownSelectable, SelectableDirection.Down);
        SetNavigation(favoriteToggle, filterButtonDownSelectable, SelectableDirection.Down);
        SetNavigation(userToggle, filterButtonDownSelectable, SelectableDirection.Down);
        SetNavigation(downloadedToggle, filterButtonDownSelectable, SelectableDirection.Down);
    }

    private void SetLevelItemButtonsNavigation(List<LevelUI> displayedLevelUIs) {

        Selectable upperTarget = displayedLevelUIs.Count > 0 ? (lastSelectedLevel ?? displayedLevelUIs[0]).selectable : allToggle;
        Selectable leftMostOnBottom = backButton;
        Selectable rightOfBack = levelItemSelectables.Find(item => { return item!=backButton && item.IsActive() && item.interactable; });
        //back button navigation
        SetNavigation(backButton, loadButton, SelectableDirection.Left);
        SetNavigation(backButton, upperTarget, SelectableDirection.Up);
        SetNavigation(backButton, rightOfBack, SelectableDirection.Right);

        //delete button navigation
        SetNavigation(deleteButton, leftMostOnBottom, SelectableDirection.Left, SelectableDirection.Up);

        //favorite button navigation
        bool deleteButtonAvailable = deleteButton.interactable && deleteButton.IsActive();
        Selectable leftTarget = deleteButtonAvailable ? deleteButton : leftMostOnBottom;
        SetNavigation(favoriteButton, leftTarget, SelectableDirection.Left);
        SetNavigation(favoriteButton, upperTarget, SelectableDirection.Up);

        //leaderboard button navigation
        leftTarget = favoriteButton.interactable ? favoriteButton : leftMostOnBottom;
        SetNavigation(leaderboardButton, leftTarget, SelectableDirection.Left);
        SetNavigation(leaderboardButton, upperTarget, SelectableDirection.Up);

        //load button navigation
        leftTarget = leaderboardButton.interactable ? leaderboardButton : leftMostOnBottom;
        SetNavigation(loadButton, leftTarget, SelectableDirection.Left);
        SetNavigation(loadButton, upperTarget, SelectableDirection.Up);
    }


    private void SetNavigation(Selectable selectable, Selectable target, params SelectableDirection[] directions) {
        selectable.navigation = selectable.SetSelection(target, directions);
    }


    private void SetButtonsInteractabilityAndNavigation(List<LevelUI> displayedLevelUIs) {
        bool anyLevelsDisplayed = displayedLevelUIs.Count > 0;
        deleteButton.interactable = anyLevelsDisplayed;
        favoriteButton.interactable = anyLevelsDisplayed && !IsHathosLevelSelected;
        leaderboardButton.interactable = true;
        loadButton.interactable = anyLevelsDisplayed;

        Selectable selected = lastSelectedLevel == null ? (anyLevelsDisplayed? displayedLevelUIs[0].selectable : null) : lastSelectedLevel.selectable;
        SetFilterButtonsNavigation(selected);
        SetLevelItemButtonsNavigation(displayedLevelUIs);
    }
    #endregion

    private void AlignScrollbar() {
        List<LevelUI> displayedLevelUIs = DisplayedLevelUIs;
        int displayedIndex = displayedLevelUIs.FindIndex(levelUI => levelUI == SelectedLevelUI);
        if (IsHathosLevelDisplayed && SelectedLevelIndex != 0) {
            displayedIndex += levelsPerRow-1;
        }        
        int levelRow = Mathf.FloorToInt(1f * displayedIndex / levelsPerRow);
        int totalRows = Mathf.CeilToInt(1f * (displayedLevelUIs.Count / levelsPerRow));
        float invertedPercentProgress = 1f * levelRow / totalRows;
        float targetProgress = 1f - invertedPercentProgress;

        MEC.Timing.KillCoroutines(ScrollCoroutineName);
        scrollSlideAnim.startValue = levelScrollBar.value;
        scrollSlideAnim.diff = targetProgress - scrollSlideAnim.startValue;
        scrollSlideAnim.Animate(ScrollCoroutineName);
    }


    private void PositionLevelsInSceneContext() {
        gridLayoutGroup.padding = IsHathosLevelDisplayed ? gridLayoutConfigWithHathos : gridLayoutConfigWithoutHathos;
    }

    private void CheckToNullLastSelected(Selectable turnedOff) {
        if (turnedOff == lastSelectedLevel) {
            lastSelectedLevel = null;
        }
    }

    private void InitializeHathosLevels() {
        levelUIs.Clear();
        int i = 0;
        preMadeLevelUITransforms.ForEach(memorableTransform => {
            memorableTransform.Initialize();
            Transform hathosLevelTransform = levelContentParent.GetChild(i);
            if (!SceneTracker.IsCurrentSceneEditor) {
                LevelUI hathosLevelUI = hathosLevelTransform.GetComponent<LevelUI>();
                hathosLevelUI.levelData.MySprite = hathosLevelUI.image.sprite;
                hathosLevelUI.Initialize(this, null, i, true);
                levelUIs.Add(hathosLevelUI);
            }
            else {
                hathosLevelTransform.gameObject.SetActive(false);
            }
            i++;
        });
    }    

    private IEnumerator ResortImagePositions() {
        gridLayoutGroup.enabled = false;
        yield return new WaitForEndOfFrame();
        gridLayoutGroup.enabled = true;
        AlignScrollbar();
    }

    private IEnumerator DelayGeneration() {
        yield return new WaitForEndOfFrame();
        GenerateLevelUI();
    }
    private void GenerateLevelUI() {
        ResetLevelUIsForHathosLevels();

        int i = NumHathosLevels;
        Levels.ForEach(level => {
            if (!string.IsNullOrEmpty(level.name)) {
                GameObject newUI = Instantiate(levelUIPrefab, levelContentParent);
                LevelUI levelUI = newUI.GetComponent<LevelUI>();
                levelUI.Initialize(this, level, i);
                levelUIs.Add(levelUI);
            }
            i++;
        });
        System.Predicate<LevelUI> containsLevel = level => !string.IsNullOrEmpty(level.levelData.name);
        if (levelUIs.Count > 0 && levelUIs.Exists(containsLevel)) {
            int firstIndex = levelUIs.FindIndex(containsLevel);
            SelectLevel(firstIndex);
        }
        OnSearchChange();
    }

    private void ResetLevelUIsForHathosLevels() {
        List<LevelUI> hathosUIs = new List<LevelUI>();
        for (int q = 0; q < NumHathosLevels; q++) {
            LevelUI hathosUI = levelUIs[q];
            hathosUIs.Add(hathosUI);
        }
        levelUIs.Clear();
        for (int q = 0; q < NumHathosLevels; q++) {
            LevelUI hathosUI = hathosUIs[q];
            levelUIs.Add(hathosUI);
        }
    }
    #endregion

}

#region Support Classes and Types
[System.Serializable]
public class MemorableTransform {
    public Transform transform;
    public TransformMemento savedConfiguration = new TransformMemento();

    public void Initialize() {
        savedConfiguration.Initialize(transform);
    }

    public void ResetTransform() {
        savedConfiguration.Reset(transform);
    }
}

public enum SelectableDirection {
    Down,
    Left,
    Right,
    Up,
}

public enum LevelSelectFilterType {
    All=0,
    Downloaded=1,
    User=2,
    Favorite=3,
}

public static class NavigationSetter {
    public static bool TrySetNavigation(this List<LevelUI> levelUIs, int currentIndex, int indexInQuestion, SelectableDirection direction) {
        if (levelUIs.IsValidLevelUIIndex(currentIndex) && levelUIs.IsValidLevelUIIndex(indexInQuestion)) {
            _SetSelection(levelUIs[currentIndex].selectable, levelUIs[indexInQuestion].selectable, direction);
            return true;
        }
        return false;
    }

    public static bool IsValidLevelUIIndex(this List<LevelUI> levelUIs, int i) {
        int clampedIndex = Mathf.Clamp(i, 0, levelUIs.Count - 1);
        return i == clampedIndex && levelUIs.Count>0;
    }
    public static Navigation SetSelection(this Selectable selectable, Selectable target, params SelectableDirection[] directions) {
        Navigation nav = selectable.navigation;
        foreach (SelectableDirection direction in directions) {
            switch (direction) {
                case SelectableDirection.Down: nav.selectOnDown = target; break;
                case SelectableDirection.Left: nav.selectOnLeft = target; break;
                case SelectableDirection.Right: nav.selectOnRight = target; break;
                case SelectableDirection.Up: nav.selectOnUp = target; break;
            }
        }
        return nav;
    }

    private static void _SetSelection(Selectable selectable, Selectable target, SelectableDirection direction) {
        selectable.navigation = selectable.SetSelection(target, direction);
    }
}

public static class LevelSelectFliter {
    public static bool IsValidLevel(this LevelData levelData, LevelSelectFilterType filter) {
        switch (filter) {
            case LevelSelectFilterType.All: return true;
            case LevelSelectFilterType.Downloaded: return levelData.isDownloaded;
            case LevelSelectFilterType.User: return levelData.isMadeByThisUser;
            case LevelSelectFilterType.Favorite: return levelData.isFavorite;
        }
        return false;
    }
}
#endregion