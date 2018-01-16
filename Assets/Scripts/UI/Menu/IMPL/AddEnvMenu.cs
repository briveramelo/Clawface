using UnityEngine;
using UnityEngine.UI;

using PlayerLevelEditor;

public class AddEnvMenu : Menu {

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

        levelObject = UnityTool.FindGameObject("LEVEL");

        //TODO: Get rid of "test"
        levelBlock = Resources.Load(Strings.Editor.RESOURCE_PATH + "Env/test") as GameObject;

    }

    // Update is called once per frame
    private void Update()
    {
        Ray r = UnityEngine.Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit h;

        if(Physics.Raycast(r,out h, raycastDistance))
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

    #endregion

    #region Public Interface

    public AddEnvMenu() : base(Strings.MenuStrings.ADD_ENV_PLE)
    { }

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
