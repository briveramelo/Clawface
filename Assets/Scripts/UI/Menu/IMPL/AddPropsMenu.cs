﻿using UnityEngine;
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
    
    private bool initialized = false;

    private Vector3 newItemPos = Vector3.zero;

    private bool inputGuard = false;
    #endregion


    #region Unity Lifecycle

    // Update is called once per frame
    private void Update()
    {
        if(inputGuard)
        {
            if (initialized)
            {
                UpdateObjectPreview();
            }

            if (!newItemPos.Equals(Vector3.zero))
            {
                DrawSelectedItemPlacement();
            }

            if (InputManager.Instance.QueryAction(Strings.Input.UI.CANCEL, ButtonMode.DOWN))
            {
                BackAction();
            }

        }
    }



    #endregion

    #region Public Interface

    public AddPropsMenu() : base(Strings.MenuStrings.ADD_PROPS_PLE)
    { }

    public void Initialize(params object[] par)
    {
        levelObject = EditorToolKit.FindGameObject("LEVEL");
        levelBlock = Resources.Load(Strings.Editor.RESOURCE_PATH + Strings.Editor.BASIC_LVL_BLOCK) as GameObject;

        initialized = true;
    }

    public void AddAction()
    {
        GameObject _instance = GameObject.Instantiate(levelBlock, newItemPos, Quaternion.identity);

        _instance.transform.SetParent(levelObject.transform);

        newItemPos = Vector3.zero;
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
    }

    protected override void HideStarted()
    {
        base.HideStarted();
        inputGuard = false;
        initialized = false;
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

    private void DrawSelectedItemPlacement()
    {
        ToolLib.draft(levelBlock, newItemPos, Color.blue);
    }

    private void UpdateObjectPreview()
    {

        Ray r = UnityEngine.Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit h;

        if (Physics.Raycast(r, out h, raycastDistance))
        {
            sceneMousePos = h.point;

            if(Input.GetMouseButtonDown(0))
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
        ToolLib.draft(levelBlock, ToolLib.ConvertToGrid(sceneMousePos - levelBlock.transform.position), Color.green);
        
    }



    #endregion


}