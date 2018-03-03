using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ModMan;
using PlayerLevelEditor;
using System.Linq;

public class PLELevelSelectMenu : PlayerLevelEditorMenu {

    public PLELevelSelectMenu() : base(Strings.MenuStrings.LevelEditor.LEVELSELECT_PLE_MENU) { }

    [HideInInspector] public string backMenuTarget;

    #region Fields (Serialized)
    [SerializeField] private Transform levelContentParent;
    [SerializeField] private GameObject levelUIPrefab;
    [SerializeField] private Text selectedLevelNameText, selectedLevelDescriptionText;
    [SerializeField] private Image selectedLevelImage, selectedLevelFavoriteIcon;
    [SerializeField] private InputField searchField;
    [SerializeField] private GridLayoutGroup gridLayoutGroup;
    [SerializeField] private Button deleteButton, favoriteButton, loadButton;
    [SerializeField] private LevelDataManager levelDataManager;
    [SerializeField] private GameObject plePrefab;
    [SerializeField] private List<MemorableTransform> preMadeLevelUITransforms;
    [SerializeField] private RectOffset gridLayoutConfigWithHathos, gridLayoutConfigWithoutHathos;
    #endregion

    #region Fields (Private)
    private DataSave ActiveDataSave { get { return DataPersister.ActiveDataSave; } }
    private List<LevelData> Levels { get { return ActiveDataSave.levelDatas; } }
    private List<LevelUI> levelUIs = new List<LevelUI>();
    private int selectedLevelIndex = 0;
    private bool IsHathosLevelSelected { get { return !SceneTracker.IsCurrentSceneEditor && selectedLevelIndex == 0; } }
    private int NumHathosLevels { get { return !SceneTracker.IsCurrentSceneEditor ? preMadeLevelUITransforms.Count : 0; } }
    private LevelSelectFilterType activeFilter=LevelSelectFilterType.All;
    #endregion



    #region Unity Lifecyle
    protected override void Start() {
        base.Start();
    }
    #endregion

    #region Public Interface
    public void FilterLevels(int filterType) {
        activeFilter = (LevelSelectFilterType)filterType;
        for (int i = 0; i < levelUIs.Count; i++) {
            LevelData levelData = levelUIs[i].levelData;
            bool shouldShow = levelData.IsValidLevel(activeFilter);
            levelUIs[i].gameObject.SetActive(shouldShow);
        }
    }

    public void OnSearchChange() {
        gridLayoutGroup.enabled = false;
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
        }
        FilterLevels((int)activeFilter);
        StartCoroutine(ResortImagePositions());
    }

    public void LoadLevel() {
        ActiveDataSave.SelectedLevelIndex = selectedLevelIndex - NumHathosLevels;

        if (SceneTracker.IsCurrentSceneEditor) {
            levelDataManager.LoadSelectedLevel();
            levelEditor.SwitchToMenu(PLEMenu.FLOOR);
        }
        else {
            string loadMenuName = Strings.MenuStrings.LOAD;
            WeaponSelectMenu weaponSelectMenu = (WeaponSelectMenu)MenuManager.Instance.GetMenuByName(Strings.MenuStrings.WEAPON_SELECT);
            weaponSelectMenu.DefineNavigation(Strings.MenuStrings.LevelEditor.LEVELSELECT_PLE_MENU, loadMenuName);

            LoadMenu loadMenu = (LoadMenu)MenuManager.Instance.GetMenuByName(loadMenuName);
            loadMenu.TargetScenePath = IsHathosLevelSelected ? Strings.Scenes.ScenePaths.Arena : Strings.Scenes.ScenePaths.PlayerLevels;


            MenuManager.Instance.DoTransition(weaponSelectMenu, Transition.SHOW, new Effect[] { Effect.EXCLUSIVE });
        }
    }

    public void SelectLevel(int levelIndex) {
        selectedLevelIndex = levelIndex;
        LevelData selectedLevel = levelUIs[levelIndex].levelData;
        selectedLevelNameText.text = selectedLevel.name;
        selectedLevelDescriptionText.text = selectedLevel.description;
        selectedLevelImage.sprite = selectedLevel.MySprite;

        levelUIs.ForEach(levelUI => { levelUI.OnGroupSelectChanged(levelIndex); });
        favoriteButton.interactable = !IsHathosLevelSelected;
        selectedLevelFavoriteIcon.enabled = selectedLevel.isFavorite;
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

    public void ToggleSelectedLevelIsFavorite() {
        LevelUI selectedLevelUI = levelUIs[selectedLevelIndex];
        selectedLevelUI.ToggleIsFavorite();
        selectedLevelFavoriteIcon.enabled = selectedLevelUI.levelData.isFavorite;
    }

    public void DeleteSelectedLevel() {
        levelDataManager.DeleteSelectedLevel();
        SetButtonsInteractability();
    }
    #endregion

    #region Protected Interface
    protected override void ShowStarted() {
        base.ShowStarted();
        activeFilter = LevelSelectFilterType.All;
        searchField.text = "";
        InitializeHathosLevels();
        PositionLevelsInSceneContext();
        ClearAndGenerateLevelUI();
        if (SceneTracker.IsCurrentSceneEditor) {
            deleteButton.gameObject.SetActive(true);
        }
        else {
            deleteButton.gameObject.SetActive(false);
        }
        SetButtonsInteractability();
    }
    #endregion

    #region Private Interface    
    private void SetButtonsInteractability() {
        bool buttonsInteractable = levelUIs.Count > 0;
        deleteButton.interactable = buttonsInteractable;
        favoriteButton.interactable = buttonsInteractable;
        loadButton.interactable = buttonsInteractable;
    }


    private void PositionLevelsInSceneContext() {
        gridLayoutGroup.padding = SceneTracker.IsCurrentSceneEditor ? gridLayoutConfigWithoutHathos : gridLayoutConfigWithHathos;
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
        yield return new WaitForEndOfFrame();
        gridLayoutGroup.enabled = true;
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
            SelectLevel(levelUIs.FindIndex(containsLevel));
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

public enum LevelSelectFilterType {
    All=0,
    Downloaded=1,
    User=2,
    Favorite=3,
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