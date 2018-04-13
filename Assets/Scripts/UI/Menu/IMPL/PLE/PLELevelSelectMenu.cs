using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ModMan;
using PLE;
using System.Linq;
using System;
using MEC;
public class PLELevelSelectMenu : PLEMenu {

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
    [SerializeField] private Selectable allFilter, downloadedFilter, userFilter, favoriteFilter;
    [SerializeField] private Scrollbar levelScrollBar;
    [SerializeField] private Sprite hathosBigPreview;
    [SerializeField] private RectTransform levelSelectArea;
    [SerializeField] private Vector2 offsetMinEditor, offsetMaxEditor, offsetMinMain, offsetMaxMain;
    [SerializeField] private SelectorToggleGroup filterToggleGroup;
    [SerializeField] private List<MemorableTransform> preMadeLevelUITransforms;
    [SerializeField] private RectOffset gridLayoutConfigWithHathos, gridLayoutConfigWithoutHathos;
    [SerializeField] private DiffAnim scrollSlideAnim;
    [SerializeField] private AbsAnim selectLevelAnim;    
    #endregion

    #region Fields (Private)
    private DataSave ActiveDataSave { get { return DataPersister.ActiveDataSave; } }
    private List<LevelData> Levels { get { return ActiveDataSave.levelDatas; } }
    private List<LevelUI> levelUIs = new List<LevelUI>();
    private List<LevelUI> DisplayedLevelUIs { get { return levelUIs.Where(levelUI => levelUI.gameObject.activeInHierarchy).ToList(); } }
    private int selectedLevelIndex = 0;
    private int SelectedLevelIndex { get { return Mathf.Clamp(selectedLevelIndex, 0, levelUIs.Count - 1); } set { selectedLevelIndex = value; } }

    private bool IsHathosLevelDisplayed { get { return NumHathosLevels > 0 && levelUIs.Count > 0 && levelUIs[0].gameObject.activeInHierarchy; } }
    private bool IsHathosLevelSelected { get { return !SceneTracker.IsCurrentSceneEditor && SelectedLevelIndex == 0; } }
    private int NumHathosLevels { get { return !SceneTracker.IsCurrentSceneEditor ? preMadeLevelUITransforms.Count : 0; } }
    private Predicate<LevelUI> currentSelectedUIIsLevel;
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
    private List<Selectable> filterSelectables = new List<Selectable>();
    private int SelectedFilterToggle {
        get {
            return Mathf.Clamp(selectedFilterIndex, 0, filterSelectables.Count-1);
        }
        set {
            if (value >= filterSelectables.Count) {
                value = 0;
            }
            else if (value < 0) {
                value = filterSelectables.Count;
            }
            selectedFilterIndex = value;
            filterToggleGroup.HandleGroupSelection(SelectedFilterToggle);
        }
    }
    private LevelSelectFilterType ActiveFilter { get { return (LevelSelectFilterType)SelectedFilterToggle; } }
    private Predicate<LevelUI> shouldSearchShow;
    private List<Selectable> bottomRowSelectables = new List<Selectable>();
    private int selectedFilterIndex;
    private bool IsLastLevelShowing { get { return lastSelectedLevel == null ? false : (lastSelectedLevel.gameObject.activeInHierarchy); } }
    private string ScrollCoroutineName { get { return CoroutineName + "Scroll"; } }
    private string PulseCoroutineName { get { return CoroutineName + "Pulse"; } }
    #endregion

    #region Unity Lifecyle

    protected override void KillCoroutines() {
        base.KillCoroutines();
        Timing.KillCoroutines(ScrollCoroutineName);
        Timing.KillCoroutines(PulseCoroutineName);
    }
    protected override void Start() {
        base.Start();

        shouldSearchShow = (levelUI) => {
            string searchTerm = searchField.text.ToLowerInvariant();
            List<string> searchTerms = searchTerm.Split(' ').ToList();
            LevelData levelData = levelUI.levelData;
            string levelDataName = levelData.name.ToLowerInvariant();
            string levelDataDescription = levelData.description.ToLowerInvariant();
            bool shouldShow =
                (string.IsNullOrEmpty(searchTerm) ||
                searchTerm.Length == 0 ||
                searchTerms.All(term => { return levelDataName.Contains(term) || levelDataDescription.Contains(term); }));

            levelUI.gameObject.SetActive(shouldShow);
            if (!shouldShow) {
                CheckToNullLastSelected(levelUI.selectable);
            }
            return shouldShow;
        };


        filterSelectables = new List<Selectable>() {
            allFilter, downloadedFilter, userFilter, favoriteFilter
        };
        bottomRowSelectables = new List<Selectable>() {
            backButton, deleteButton, favoriteButton, leaderboardButton, loadButton
        };
        scrollSlideAnim.OnUpdate = (val) => { levelScrollBar.value = val; };
        currentSelectedUIIsLevel = item => CurrentEventSystemGameObject == item.selectable.gameObject;
    }

    protected override void Update() {
        base.Update();
        if (allowInput && !MenuManager.Instance.MouseMode) {
            CheckToSelectNewLevelFromController();
            CheckToMoveFilter();
        }
    }
    #endregion

    #region Public Interface
    public override MenuType ThisMenuType { get { return MenuType.LevelSelect; } }
    public void ChangeFilters(int newFilterType) {
        SFXManager.Instance.Play(SFXType.UI_Click);
        SelectedFilterToggle = newFilterType;
        FilterLevels(shouldSearchShow);
    }
    

    void FilterLevels(Predicate<LevelUI> shouldSearchShow) {
        for (int i = 0; i < levelUIs.Count; i++) {
            LevelData levelData = levelUIs[i].levelData;
            bool shouldShow = levelData.IsValidLevel(ActiveFilter) && shouldSearchShow(levelUIs[i]);
            levelUIs[i].gameObject.SetActive(shouldShow);
            if (!shouldShow) {
                CheckToNullLastSelected(levelUIs[i].selectable);
            }
        }
        PositionLevelsInSceneContext();
        StartCoroutine(ResortImagePositions());
    }

    public void OnSearchChange() {
        FilterLevels(shouldSearchShow);
    }

    public void LoadLevel() {

        if (SceneTracker.IsCurrentSceneEditor)
        {
            ConfirmMenu confirmMenu = (ConfirmMenu)MenuManager.Instance.GetMenuByName(Strings.MenuStrings.CONFIRM);
            Action onYesAction = () =>
            {
                TryLoadSelectedLevel();
            };

            Action onNoAction = () =>
            {
                MenuManager.Instance.DoTransition(confirmMenu, Transition.HIDE, new Effect[] { Effect.INSTANT });
                SelectInitialButton();
            };            
            confirmMenu.DefineActions("You will lose any unsaved data. Are you sure?", onYesAction, onNoAction);

            MenuManager.Instance.DoTransition(confirmMenu, Transition.SHOW, new Effect[] { Effect.INSTANT });
        }
        else
        {
            TryLoadSelectedLevel();
        }
    }

    public void SelectLevel(LevelUI levelUI, bool selectSelectable = true) {
        SelectLevel(levelUI.LevelIndex);
    }

    public void SelectLevel(int levelIndex, bool selectSelectable=true) {
        SFXManager.Instance.Play(SFXType.UI_Click);
        Timing.KillCoroutines(PulseCoroutineName);
        if (lastSelectedLevel!=null) {
            lastSelectedLevel.ScaleLevelUISize(1f);
        }

        SelectedLevelIndex = levelIndex;

        selectLevelAnim.OnUpdate = SelectedLevelUI.ScaleLevelUISize;
        selectLevelAnim.Animate(PulseCoroutineName);

        if (selectSelectable) {
            SelectedLevelUI.selectable.Select();
            CurrentEventSystem.SetSelectedGameObject(SelectedLevelUI.selectable.gameObject);
        }
        lastSelectedLevel = SelectedLevelUI;

        LevelData selectedLevel = SelectedLevelUI.levelData;
        selectedLevelNameText.text = selectedLevel.name;
        selectedLevelDescriptionText.text = selectedLevel.description;
        Sprite spriteToShow = selectedLevel.isHathosLevel ? hathosBigPreview : selectedLevel.MySprite;
        
        selectedLevelImage.sprite = spriteToShow;

        levelUIs.ForEach(levelUI => { levelUI.OnGroupSelectChanged(levelIndex); });
        selectedLevelFavoriteIcon.enabled = selectedLevel.isFavorite;
        SetButtonsInteractabilityAndNavigation();
        AlignScrollbar();
    }

    public override void BackAction() {
        if (SceneTracker.IsCurrentSceneEditor) {
            Menu confirmMenu = MenuManager.Instance.GetMenuByName(Strings.MenuStrings.CONFIRM);
            if (!confirmMenu.Displayed) {
                base.BackAction();
            }
        }
        else {
            MenuManager.Instance.DoTransition(backMenuTarget, Transition.SHOW, new Effect[] { Effect.EXCLUSIVE });
        }
    }

    public void ClearAndGenerateLevelUI() {
        levelUIPrefab.transform.SetParent(null);
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
            MenuManager.Instance.DoTransition(Strings.MenuStrings.LEADER_BOARDS, Transition.SHOW, new Effect[] { Effect.EXCLUSIVE, Effect.INSTANT });
            ((LeaderboardsMenu)MenuManager.Instance.GetMenuByName(Strings.MenuStrings.LEADER_BOARDS)).SetLevelName(levelName);
        }
    }

    public void ToggleSelectedLevelIsFavorite() {
        LevelUI selectedLevelUI = levelUIs[SelectedLevelIndex];
        selectedLevelUI.ToggleIsFavorite();
        selectedLevelFavoriteIcon.enabled = selectedLevelUI.levelData.isFavorite;
        DataPersister.Instance.TrySaveDataFile();
    }

    public void DeleteSelectedLevel() {

        ConfirmMenu confirmMenu = (ConfirmMenu)MenuManager.Instance.GetMenuByName(Strings.MenuStrings.CONFIRM);

        Action onYesAction = () =>
        {
            levelDataManager.TryDeleteLevel(SelectedLevelUI.levelData.UniqueSteamName);
            SetButtonsInteractabilityAndNavigation();
            ClearAndGenerateLevelUI();
        };

        Action onNoAction = () =>
        {
            MenuManager.Instance.DoTransition(confirmMenu, Transition.HIDE, new Effect[] { Effect.INSTANT });
            SelectInitialButton();
        };
        string confirmationMessage = SelectedLevelUI.levelData.isDownloaded ? "Unsubscribe from this Steam Workshop item to permanently remove. Are you sure?" : "This cannot be undone. Are you sure?";
        confirmMenu.DefineActions(confirmationMessage, onYesAction, onNoAction);

        MenuManager.Instance.DoTransition(confirmMenu, Transition.SHOW, new Effect[] { Effect.INSTANT });

    }
    #endregion

    #region Protected Interface

    private void TryLoadSelectedLevel()
    {
        if (SelectedLevelUI!=null) {
            if (!SelectedLevelUI.levelData.isHathosLevel) {
                DataPersister.ActiveDataSave.FillWorkingLevelDataWithExistingLevelData(SelectedLevelUI.levelData.UniqueSteamName);
            }

            if (SceneTracker.IsCurrentSceneEditor)
            {
                levelDataManager.LoadSelectedLevel();
            }
            else
            {
                string loadMenuName = Strings.MenuStrings.LOAD;
                WeaponSelectMenu weaponSelectMenu = (WeaponSelectMenu)MenuManager.Instance.GetMenuByName(Strings.MenuStrings.WEAPON_SELECT);
                weaponSelectMenu.DefineNavigation(Strings.MenuStrings.LevelEditor.LEVELSELECT_PLE_MENU, loadMenuName);

                LoadMenu loadMenu = (LoadMenu)MenuManager.Instance.GetMenuByName(loadMenuName);
                loadMenu.SetNavigation(IsHathosLevelSelected ? Strings.Scenes.ScenePaths.Arena : Strings.Scenes.ScenePaths.PlayerLevels);


                MenuManager.Instance.DoTransition(weaponSelectMenu, Transition.SHOW, new Effect[] { Effect.EXCLUSIVE });
            }
        }
    }
    protected override void ShowStarted() {
        base.ShowStarted();
        SelectedFilterToggle = (int)LevelSelectFilterType.All;
        searchField.text = "";
        InitializeHathosLevels();
        ClearAndGenerateLevelUI();
        if (SceneTracker.IsCurrentSceneEditor) {
            deleteButton.gameObject.SetActive(true);
            leaderboardButton.gameObject.SetActive(false);
            levelSelectArea.offsetMin = offsetMinEditor;
            levelSelectArea.offsetMax = offsetMaxEditor;
        }
        else {
            deleteButton.gameObject.SetActive(false);
            leaderboardButton.gameObject.SetActive(true);
            levelSelectArea.offsetMin = offsetMinMain;
            levelSelectArea.offsetMax = offsetMaxMain;
        }
    }
    #endregion

    #region Private Interface 
    private void CheckToMoveFilter() {
        bool leftButtonPressed = InputManager.Instance.QueryAction(Strings.Input.UI.TAB_LEFT, ButtonMode.DOWN);
        bool rightBumperPressed = InputManager.Instance.QueryAction(Strings.Input.UI.TAB_RIGHT, ButtonMode.DOWN);
        bool mouseClicked = Input.GetMouseButtonDown(MouseButtons.LEFT) || Input.GetMouseButtonDown(MouseButtons.RIGHT) || Input.GetMouseButtonDown(MouseButtons.MIDDLE);
        if (!mouseClicked && (leftButtonPressed || rightBumperPressed)) {
            if (leftButtonPressed) {
                SelectedFilterToggle--;
            }
            else {
                SelectedFilterToggle++;
            }
            ChangeFilters(SelectedFilterToggle);
            CurrentEventSystem.SetSelectedGameObject(filterSelectables[SelectedFilterToggle].gameObject);
        }
    }


    private void CheckToSelectNewLevelFromController() {
        bool isLevelUISelected = SelectedLevelUI != null && levelUIs.Exists(currentSelectedUIIsLevel);
        if (isLevelUISelected && SelectedLevelUI.selectable.gameObject != lastSelectedGameObject) {
            LevelUI newlySelectedLevelUI = levelUIs.Find(currentSelectedUIIsLevel);
            SelectLevel(newlySelectedLevelUI.LevelIndex);
        }

        if (InputManager.Instance.QueryAction(Strings.Input.UI.SUBMIT, ButtonMode.DOWN)) {
            if (isLevelUISelected) {
                SFXManager.Instance.Play(SFXType.UI_Click);
                CurrentEventSystem.SetSelectedGameObject(loadButton.gameObject);
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
        SetFilterButtonsAndSearchBarNavigation(filterButtonDownSelectable);
        SetButtonsInteractabilityAndNavigation();
    }

    void SetPlayerLevelNavigation(List<LevelUI> displayedLevelUIs) {
        SetMainBodyofLevelUINavigation(displayedLevelUIs);
        SetLastLevelNavigation(displayedLevelUIs);
    }

    void SetHathosLevelNavigation(List<LevelUI> displayedLevelUIs) {
        Selectable hathosDownTarget = displayedLevelUIs.Count > 1 ? displayedLevelUIs[1].selectable : loadButton;
        SetNavigation(levelUIs[0].selectable, hathosDownTarget, SelectableDirection.Down, SelectableDirection.Right);
        SetNavigation(levelUIs[0].selectable, favoriteFilter, SelectableDirection.Up);
    }

    private void SetMainBodyofLevelUINavigation(List<LevelUI> displayedLevelUIs) {
        Selectable topRowUpSelectable = IsHathosLevelDisplayed ? levelUIs[0].selectable : downloadedFilter;
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

    private void SetFilterButtonsAndSearchBarNavigation(Selectable filterButtonDownSelectable) {
        SetNavigation(allFilter, filterButtonDownSelectable, SelectableDirection.Down);
        SetNavigation(favoriteFilter, filterButtonDownSelectable, SelectableDirection.Down);
        SetNavigation(userFilter, filterButtonDownSelectable, SelectableDirection.Down);
        SetNavigation(downloadedFilter, filterButtonDownSelectable, SelectableDirection.Down);
        SetNavigation(searchField, filterButtonDownSelectable, SelectableDirection.Down);
    }

    private void SetLevelItemButtonsNavigation(List<LevelUI> displayedLevelUIs) {

        Selectable upperTarget = displayedLevelUIs.Count > 0 ? (lastSelectedLevel ?? displayedLevelUIs[0]).selectable : allFilter;
        Selectable leftMostOnBottom = backButton;
        Predicate<Selectable> isInteractable = (item) => item.Interactable();

        //back button navigation
        Selectable leftTarget = loadButton;
        Selectable rightTarget = bottomRowSelectables.FindInDirection(isInteractable, 0, true);
        SetNavigation(backButton, leftTarget, SelectableDirection.Left);
        SetNavigation(backButton, upperTarget, SelectableDirection.Up);
        SetNavigation(backButton, rightTarget, SelectableDirection.Right);

        //delete button navigation
        leftTarget = backButton;
        rightTarget = bottomRowSelectables.FindInDirection(isInteractable, 1, true);
        SetNavigation(deleteButton, leftTarget, SelectableDirection.Left);
        SetNavigation(deleteButton, upperTarget, SelectableDirection.Up);
        SetNavigation(deleteButton, rightTarget, SelectableDirection.Right);

        //favorite button navigation
        leftTarget = bottomRowSelectables.FindInDirection(isInteractable, 2, false, false);
        rightTarget = bottomRowSelectables.FindInDirection(isInteractable, 2, true);
        SetNavigation(favoriteButton, leftTarget, SelectableDirection.Left);
        SetNavigation(favoriteButton, upperTarget, SelectableDirection.Up);
        SetNavigation(favoriteButton, rightTarget, SelectableDirection.Right);

        //leaderboard button navigation
        leftTarget = bottomRowSelectables.FindInDirection(isInteractable, 3, false, false);
        rightTarget = bottomRowSelectables.FindInDirection(isInteractable, 3, true);
        SetNavigation(leaderboardButton, leftTarget, SelectableDirection.Left);
        SetNavigation(leaderboardButton, upperTarget, SelectableDirection.Up);
        SetNavigation(leaderboardButton, rightTarget, SelectableDirection.Right);

        //load button navigation
        leftTarget = bottomRowSelectables.FindInDirection(isInteractable, 4, false, false);
        rightTarget = backButton;
        SetNavigation(loadButton, leftTarget, SelectableDirection.Left);
        SetNavigation(loadButton, upperTarget, SelectableDirection.Up);
        SetNavigation(loadButton, rightTarget, SelectableDirection.Right);
    }


    private void SetNavigation(Selectable selectable, Selectable target, params SelectableDirection[] directions) {
        selectable.navigation = selectable.SetSelection(target, directions);
    }


    private void SetButtonsInteractabilityAndNavigation() {
        List<LevelUI> displayedLevelUIs = DisplayedLevelUIs;
        bool anyLevelsDisplayed = displayedLevelUIs.Count > 0;
        deleteButton.interactable = anyLevelsDisplayed;
        favoriteButton.interactable = anyLevelsDisplayed && !IsHathosLevelSelected;
        leaderboardButton.interactable = true;
        loadButton.interactable = anyLevelsDisplayed;

        Selectable selected = lastSelectedLevel == null ? (anyLevelsDisplayed? displayedLevelUIs[0].selectable : null) : lastSelectedLevel.selectable;
        SetFilterButtonsAndSearchBarNavigation(selected);
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

        Timing.KillCoroutines(ScrollCoroutineName);
        scrollSlideAnim.startValue = levelScrollBar.value;
        scrollSlideAnim.diff = targetProgress - scrollSlideAnim.startValue;
        scrollSlideAnim.Animate(ScrollCoroutineName);
    }


    private void PositionLevelsInSceneContext() {
        gridLayoutGroup.padding = IsHathosLevelDisplayed ? gridLayoutConfigWithHathos : gridLayoutConfigWithoutHathos;
    }

    private void CheckToNullLastSelected(Selectable turnedOff) {
        if (lastSelectedLevel!=null && turnedOff == lastSelectedLevel.selectable) {
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
        List<LevelUI> displayedLevels = DisplayedLevelUIs;
        if (displayedLevels!=null && displayedLevels.Count>0) {
            SelectLevel(displayedLevels[0].LevelIndex, false);
        }
        SetButtonNavigation();
        AlignScrollbar();
    }

    private IEnumerator DelayGeneration() {
        yield return new WaitForEndOfFrame();
        GenerateLevelUI();
    }
    private void GenerateLevelUI() {
        ResetLevelUIsForHathosLevels();

        int i = NumHathosLevels;
        levelUIPrefab.SetActive(true);
        Levels.ForEach(level => {
            if (!string.IsNullOrEmpty(level.name)) {
                GameObject newUI = Instantiate(levelUIPrefab, levelContentParent);
                newUI.transform.localScale = Vector3.one;
                newUI.name = string.Format("{0}{1}", newUI.name, i);
                LevelUI levelUI = newUI.GetComponent<LevelUI>();
                levelUI.Initialize(this, level, i);
                levelUIs.Add(levelUI);
            }
            i++;
        });
        levelUIPrefab.SetActive(false);
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

    public override void SetMenuButtonInteractabilityByState() {
        SetButtonsInteractabilityAndNavigation();
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