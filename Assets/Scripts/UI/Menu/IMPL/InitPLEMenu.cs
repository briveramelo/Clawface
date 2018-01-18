using UnityEngine.UI;
using UnityEngine;
using PlayerLevelEditor;

public class InitPLEMenu : Menu {

    #region Public Fields

    public override Button InitialSelection
    {
        get
        {
            return confirmButton;
        }
    }


    #endregion

    #region Serialized Unity Fields

    [SerializeField] private Button confirmButton;
    [SerializeField] private Slider xSlide;
    [SerializeField] private Slider zSlide;
    [SerializeField] private LevelEditor editorInstance;
    #endregion



    #region Private Fields

    int levelX = 0;
    int levelZ = 0;
    
    GameObject levelBlock;

    private Menu mainEditorMenu = null;

    private bool initialized = false;
    
    #endregion
   
    #region Unity Lifecycle
    
    private void Update()
    {
        if(initialized)
        {
            UpdateBlockPreview();
        }
        
    }

    private void OnDisable()
    {
        initialized = false;
    }

    #endregion  

    #region Protected Interface

    protected override void DefaultShow(Transition transition, Effect[] effects)
    {
        Fade(transition, effects);
    }

    protected override void DefaultHide(Transition transition, Effect[] effects)
    {
        Fade(transition, effects);
    }

    #endregion

    #region Public Interface

    public void Init()
    {
        levelBlock = Resources.Load(Strings.Editor.RESOURCE_PATH + Strings.Editor.BASIC_LVL_BLOCK) as GameObject;
        levelBlock.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);

        initialized = true;

    }

    public void SetMainEditorMenu(Menu i_menu)
    {
        mainEditorMenu = i_menu;
    }


    public InitPLEMenu() : base(Strings.MenuStrings.INIT_PLE_MENU)
    { }

    public void ConfirmAction()
    {

        GameObject _platform = new GameObject("LEVEL");
        GameObject _wall = new GameObject("WALL");

        for (int i = -levelX; i <= levelX; i++)
        {
            for (int j = -levelZ; j <= levelZ; j++)
            {
                GameObject _instance = GameObject.Instantiate(levelBlock, new Vector3(i * PlayerLevelEditor.ParameterSystem.unit_size,
                                                                                   0,
                                                                                   j * PlayerLevelEditor.ParameterSystem.unit_size), Quaternion.identity);

                _instance.name = "TestBlock";

                _instance.AddComponent<PlayerLevelEditor.ClickableObject>();
                //                    _instance.AddComponent<Splattable>();

                //Edge + Wall
                if (i == -levelX || i == levelX || j == -levelZ || j == levelZ)
                {
                    AddNavMeshModifier(_instance, PlayerLevelEditor.NavMeshAreas.NotWalkable);

                    GameObject _instance_Wall = GameObject.Instantiate(levelBlock, new Vector3(i * PlayerLevelEditor.ParameterSystem.unit_size,
                                                                                            1 * PlayerLevelEditor.ParameterSystem.unit_size,
                                                                                            j * PlayerLevelEditor.ParameterSystem.unit_size), Quaternion.identity);

                    _instance_Wall.name = "WallBlock";
                    _instance_Wall.tag = "Wall";
                    _instance_Wall.transform.SetParent(_wall.transform);
                    _instance_Wall.AddComponent<PlayerLevelEditor.ClickableObject>();
                    AddNavMeshModifier(_instance_Wall, PlayerLevelEditor.NavMeshAreas.NotWalkable);
                }
                else if (_instance.GetComponent<LevelUnit>() == null)
                {
                    LevelUnit LU = _instance.AddComponent<LevelUnit>() as LevelUnit;
                    LU.defaultState = LevelUnitStates.floor;
                    AddNavMeshModifier(_instance, PlayerLevelEditor.NavMeshAreas.Walkable);

                    if (i == 0 && j == 0)
                    {
                        GameObject.DestroyImmediate(LU);
                    }
                }

                _instance.transform.SetParent(_platform.transform);
            }
        }

        //hide and show the main ple menu
        MenuManager.Instance.DoTransition(editorInstance.GetMenu(PLEMenu.MAIN), Transition.SHOW, new Menu.Effect[] { Menu.Effect.EXCLUSIVE });

    }

    public void UpdateLevelSizeAction()
    {
        levelX = (int)xSlide.value;
        levelZ = (int)zSlide.value;
    }

    #endregion

    #region Private Interface

    private void UpdateBlockPreview()
    {
        for (int i = -levelX; i <= levelX; i++)
        {
            for (int j = -levelZ; j <= levelZ; j++)
            {
                Vector3 new_position = new Vector3(i * PlayerLevelEditor.ParameterSystem.unit_size, 0.0f, j * PlayerLevelEditor.ParameterSystem.unit_size);
                PlayerLevelEditor.ToolLib.draft(levelBlock, new_position, Color.yellow);
            }
        }
    }

    private void AddNavMeshModifier(GameObject i_object, int i_state)
    {
        UnityEngine.AI.NavMeshModifier mod = i_object.AddComponent<UnityEngine.AI.NavMeshModifier>() as UnityEngine.AI.NavMeshModifier;
        mod.overrideArea = true;
        mod.area = i_state;
    }

    #endregion
}
