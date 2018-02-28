using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ModMan;
using PlayerLevelEditor;
using System.Linq;

public class PLELevelSelectMenu : Menu {

    public PLELevelSelectMenu() : base (Strings.MenuStrings.LEVELSELECT_PLE_MENU) { }

    public override Button InitialSelection { get { return null; } }

    [SerializeField] private Transform levelContentParent;
    [SerializeField] private GameObject levelUIPrefab;
    [SerializeField] private Text levelNameText, levelDescriptionText;
    [SerializeField] private InputField searchField;
    [SerializeField] private Image levelImage;
    [SerializeField] private LevelEditor levelEditor;
    [SerializeField] private LevelDataManager levelDataManager;
    [SerializeField] private GridLayoutGroup gridLayoutGroup;

    //private const int width = 600;
    //private const int height = 400;
    //private static readonly Vector2 imageDimensions = new Vector2(width, height);
    private DataSave ActiveDataSave { get { return DataPersister.ActiveDataSave; } }
    private List<LevelData> Levels { get { return ActiveDataSave.levelDatas; } }
    private List<LevelUI> levelUIs= new List<LevelUI>();
    private int selectedLevelIndex = 0;

    #region Unity Lifecyle    


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
        
        StartCoroutine(WaitToActivate());
    }

    public void LoadLevel() {
        ActiveDataSave.SelectedIndex = selectedLevelIndex;
        levelEditor.SwitchToMenu(PLEMenu.FLOOR);
        levelDataManager.LoadSelectedLevel();
    }

    public void SelectLevel(int levelIndex) {
        selectedLevelIndex = levelIndex;
        LevelData selectedLevel = ActiveDataSave.levelDatas[levelIndex];
        levelNameText.text = selectedLevel.name;
        levelDescriptionText.text = selectedLevel.description;
        levelImage.sprite = selectedLevel.MySprite;
        levelUIs.ForEach(levelUI => {
            levelUI.OnGroupSelectChanged(levelIndex);
        });
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
        levelEditor.ToggleCameraController(false);
        searchField.text = "";
        ClearAndGenerateLevelUI();
    }
    protected override void HideComplete() {
        base.HideComplete();
    }

    protected override void HideStarted() {
        base.HideStarted();
        levelEditor.ToggleCameraController(true);
    }    

    protected override void DefaultHide(Transition transition, Effect[] effects) {
        Fade(transition, effects);
    }

    protected override void DefaultShow(Transition transition, Effect[] effects) {
        Fade(transition, effects);
    }
    #endregion


    #region Private Interface

    IEnumerator WaitToActivate() {
        yield return new WaitForEndOfFrame();
        gridLayoutGroup.enabled = true;
    }

    IEnumerator DelayGeneration() {
        yield return new WaitForEndOfFrame();
        GenerateLevelUI();
    }
    void GenerateLevelUI() {
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
    #endregion

}
