using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ModMan;
using PlayerLevelEditor;
using System.Linq;

public class PLELevelSelectMenu : PlayerLevelEditorMenu {

    public PLELevelSelectMenu() : base (Strings.MenuStrings.LevelEditor.LEVELSELECT_PLE_MENU) { }

    public string backMenuTarget, forwardMenuTarget;


    [SerializeField] private Transform levelContentParent;
    [SerializeField] private GameObject levelUIPrefab;
    [SerializeField] private Text levelNameText, levelDescriptionText;
    [SerializeField] private InputField searchField;
    [SerializeField] private Image levelImage;
    [SerializeField] private GridLayoutGroup gridLayoutGroup;
    [SerializeField] private GameObject deleteButtonObject;
    [SerializeField] private LevelDataManager levelDataManager;
    [SerializeField] private GameObject plePrefab;

    //private const int width = 600;
    //private const int height = 400;
    //private static readonly Vector2 imageDimensions = new Vector2(width, height);    
    private DataSave ActiveDataSave { get { return DataPersister.ActiveDataSave; } }
    private List<LevelData> Levels { get { return ActiveDataSave.levelDatas; } }
    private List<LevelUI> levelUIs= new List<LevelUI>();
    private int selectedLevelIndex = 0;

    #region Unity Lifecyle
    protected override void Start() {
        base.Start();
        if (!SceneTracker.IsCurrentSceneEditor) {
            CreateDataParentTransforms();
        }
    }
    #endregion
    #region Public Interface
    public void OnSearchChange() {
        gridLayoutGroup.enabled = false;
        string searchTerm = searchField.text.ToLowerInvariant();
        List<string> searchTerms = searchTerm.Split(' ').ToList();

        for (int i = 0; i < levelUIs.Count; i++) {
            LevelData levelData = levelUIs[i].levelData;
            string levelDataName = levelData.name.ToLowerInvariant();
            string levelDataDescription = levelData.description.ToLowerInvariant();
            bool shouldShow =
                string.IsNullOrEmpty(searchTerm) ||
                searchTerm.Length == 0 ||
                searchTerms.All(term => { return levelDataName.Contains(term) || levelDataDescription.Contains(term); });                

            levelUIs[i].gameObject.SetActive(shouldShow);
        }
        
        StartCoroutine(ResortImagePositions());
    }

    public void LoadLevel() {
        ActiveDataSave.SelectedIndex = selectedLevelIndex;

        if (SceneTracker.IsCurrentSceneEditor) {
            levelDataManager.LoadSelectedLevel();
            levelEditor.SwitchToMenu(PLEMenu.FLOOR);
        }
        else {
            string loadMenuName = Strings.MenuStrings.LOAD;
            WeaponSelectMenu weaponSelectMenu = (WeaponSelectMenu)MenuManager.Instance.GetMenuByName(Strings.MenuStrings.WEAPON_SELECT);
            weaponSelectMenu.DefineNavigation(Strings.MenuStrings.LevelEditor.LEVELSELECT_PLE_MENU, loadMenuName);

            LoadMenu loadMenu = (LoadMenu)MenuManager.Instance.GetMenuByName(loadMenuName);
            loadMenu.TargetScenePath = Strings.Scenes.ScenePaths.PlayerLevels;


            MenuManager.Instance.DoTransition(weaponSelectMenu, Transition.SHOW, new Effect[] { Effect.EXCLUSIVE });
        }
    }

    public void SelectLevel(int levelIndex) {
        selectedLevelIndex = levelIndex;
        LevelData selectedLevel = ActiveDataSave.levelDatas[levelIndex];
        levelNameText.text = selectedLevel.name;
        levelDescriptionText.text = selectedLevel.description;
        levelImage.sprite = selectedLevel.MySprite;
        levelUIs.ForEach(levelUI => { levelUI.OnGroupSelectChanged(levelIndex); });
    }
    public void ClearAndGenerateLevelUI() {
        levelContentParent.DestroyAllChildren();
        StartCoroutine(DelayGeneration());
    }
    #endregion

    #region Protected Interface
    protected override void ShowComplete() {
        base.ShowComplete();
    }

    protected override void ShowStarted() {
        base.ShowStarted();
        searchField.text = "";
        ClearAndGenerateLevelUI();
        if (SceneTracker.IsCurrentSceneEditor) {
            deleteButtonObject.SetActive(true);
        }
        else {
            deleteButtonObject.SetActive(false);
        }
    }
    protected override void HideComplete() {
        base.HideComplete();
    }

    protected override void HideStarted() {
        base.HideStarted();
        if (SceneTracker.IsCurrentSceneEditor) {

        }
        else {
            
        }
    }
    #endregion


    #region Private Interface

    public override void BackAction() {
        if (SceneTracker.IsCurrentSceneEditor) {
            base.BackAction();
        }
        else {
            MenuManager.Instance.DoTransition(backMenuTarget, Transition.SHOW, new Effect[] { Effect.EXCLUSIVE });
        }
    }

    private void ForwardAction() {

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
        int i = 0;
        levelUIs.Clear();
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
    void CreateDataParentTransforms() {
        //GameObject pleGO = Instantiate(plePrefab) as GameObject;
        //pleGO.AddComponent<SceneTraverser>().Initialize(Strings.Scenes.SceneNames.PlayerLevels);
        //levelDataManager = pleGO.GetComponentInChildren<LevelDataManager>();
        //pleGO.SetActive(false);
    }
    #endregion

}
