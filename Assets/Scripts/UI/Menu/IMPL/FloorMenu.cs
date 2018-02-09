using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using PlayerLevelEditor;

public class FloorMenu : Menu {

    #region Public Fields

    public override Button InitialSelection
    {
        get
        {
            return initiallySelected;
        }
    }

    #endregion

    #region Private Fields

    private bool inputGuard = false;
    private GameObject previewGridBlock;
    private bool initialized = false;
    private float raycastDistance = 1000.0f;
    private Vector3 sceneMousePos;
    private Vector3 newItemPos = Vector3.zero;
    #endregion

    #region Serialized Unity Fields

    [SerializeField] private Button initiallySelected;
    [SerializeField] private LevelEditor editorInstance;


    #endregion


    #region Unity Lifecycle

    private void Update()
    {
        if(inputGuard)
        {
            if(initialized)
            {
                //UpdateObjectPreview();
            }

            if (InputManager.Instance.QueryAction(Strings.Input.UI.CANCEL, ButtonMode.DOWN))
            {
                BackAction();
            }
        }
    }

    #endregion  

    #region Public Interface

    public FloorMenu() : base(Strings.MenuStrings.SET_DYNLEVEL_PLE)
    { }

    public void Initialize(params object[] par)
    {
        previewGridBlock = Resources.Load(Strings.Editor.RESOURCE_PATH + Strings.Editor.BASIC_LVL_BLOCK) as GameObject;

        initialized = true;
    }

    public void DropFloorAction()
    {
        List<GameObject> selectedObjects = editorInstance.gridController.GetSelectedBlocks();

        foreach(GameObject GO in selectedObjects)
        {
            if(GO != null)
            {
                LevelUnit LU = GO.GetComponent<LevelUnit>();

                if (LU) LU.SetCurrentState(LevelUnitStates.pit);
            }
            else
            {
                selectedObjects.Remove(GO);
            }
        }
    }

    public void FlatFloorAction()
    {
        List<GameObject> selectedObjects = editorInstance.gridController.GetSelectedBlocks();

        foreach (GameObject GO in selectedObjects)
        {
            if (GO != null)
            {
                LevelUnit LU = GO.GetComponent<LevelUnit>();

                if (LU) LU.SetCurrentState(LevelUnitStates.floor);
            }
            else
            {
                selectedObjects.Remove(GO);
            }
        }
    }

    public void RiseFloorAction()
    {
        List<GameObject> selectedObjects = editorInstance.gridController.GetSelectedBlocks();

        foreach (GameObject GO in selectedObjects)
        {
            if (GO != null)
            {
                LevelUnit LU = GO.GetComponent<LevelUnit>();

                if (LU) LU.SetCurrentState(LevelUnitStates.cover);
            }
            else
            {
                selectedObjects.Remove(GO);
            }
        }
    }

    public void BackAction()
    {
        MenuManager.Instance.DoTransition(editorInstance.GetMenu(PLEMenu.MAIN), Transition.SHOW, new Effect[] { Effect.EXCLUSIVE });
    }


    #endregion

    #region Protected Interface

    protected override void ShowComplete()
    {
        base.ShowComplete();
        inputGuard = true;
        editorInstance.gridController.SetGridVisiblity(true);
        editorInstance.gridController.currentEditorMenu = EditorMenu.FLOOR_MENU;
    }

    protected override void HideStarted()
    {
        base.HideStarted();
        inputGuard = false;
        initialized = false;
        editorInstance.gridController.ClearSelectedBlocks();
        editorInstance.gridController.SetGridVisiblity(false);
    }

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

    private void UpdateObjectPreview()
    {
        Ray r = UnityEngine.Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit h;

        if (Physics.Raycast(r, out h, raycastDistance))
        {
            sceneMousePos = h.point;

            if (Input.GetMouseButtonDown(MouseButtons.LEFT))
            {
                Vector3 objectPos = PlayerLevelEditor.ToolLib.ConvertToGrid(sceneMousePos);

                //Consider when placing on top of spawnpoints
                //IsLegalPlacement();

                if (objectPos != null)
                {
                    newItemPos = objectPos;
                }

            }

        }

        //draw preview block at location
        //ToolLib.draft(previewGridBlock, ToolLib.ConvertToGrid(sceneMousePos - previewGridBlock.transform.position), Color.green);

    }

    #endregion
}
