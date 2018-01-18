using UnityEngine;
using UnityEngine.UI;

using PlayerLevelEditor;

public class AddPropsMenu : Menu {

    #region Public Fields

    public override Button InitialSelection
    {
        get
        {
            return initiallySelected;
        }
    }

    #endregion

    #region Serialized Unity Fields

    [SerializeField] private Button initiallySelected;
    [SerializeField] private LevelEditor editorInstance;


    #endregion

    #region Private Fields

    //TODO: Only uses the level block prefab, need to hook in new method
    //to select other items
    private static GameObject levelBlock;

    private GameObject levelObject;

    private float raycastDistance = 1000.0f;

    private Vector3 sceneMousePos;

    private bool addingEnabled;
    #endregion  


    #region Unity Lifecycle

    private void Start()
    {
        if (EventSystem.Instance)
        {
            EventSystem.Instance.RegisterEvent(Strings.Events.INIT_EDITOR, Initialize);
        }

        //TODO: Get rid of "test"
        levelBlock = Resources.Load(Strings.Editor.RESOURCE_PATH + "Env/test") as GameObject;

    }

    private void OnDestroy()
    {
        if (EventSystem.Instance)
        {
            EventSystem.Instance.UnRegisterEvent(Strings.Events.INIT_EDITOR, Initialize);
        }
    }

    // Update is called once per frame
    private void Update()
    {
        //TODO: This needs to go in a better place
        if (levelObject)
        {

            Ray r = UnityEngine.Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit h;

            if (Physics.Raycast(r, out h, raycastDistance))
            {
                sceneMousePos = h.point;

                if (Input.GetMouseButtonDown(0) && addingEnabled)
                {
                    LevelUnit LU = h.transform.gameObject.GetComponent<LevelUnit>();

                    if (LU != null)
                    {
                        GameObject.DestroyImmediate(LU);
                    }

                    Vector3 _pos = PlayerLevelEditor.ToolLib.ConvertToGrid(sceneMousePos);

                    if (_pos.x == 0.0f && _pos.z == 0.0f)
                    {
                        return;
                    }

                    GameObject _instance = GameObject.Instantiate(levelBlock, _pos, Quaternion.identity);

                    _instance.transform.SetParent(levelObject.transform);
                }
            }

            ToolLib.draft(levelBlock, ToolLib.ConvertToGrid(sceneMousePos - levelBlock.transform.position), Color.green);
        }
    }

    #endregion

    #region Public Interface

    public AddPropsMenu() : base(Strings.MenuStrings.ADD_PROPS_PLE)
    { }

    public void Initialize(params object[] par)
    {
        levelObject = UnityTool.FindGameObject("LEVEL");
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

    #region Private Interface

    public void AddAction()
    {
        //TODO: Set Button to activated state via Sprite change
#if UNITY_EDITOR
        Debug.Log("Adding shit ooooh");
#endif

        addingEnabled = !addingEnabled;

    }

    #endregion


}
