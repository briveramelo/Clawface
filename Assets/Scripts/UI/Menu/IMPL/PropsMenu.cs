using System.Collections;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using PlayerLevelEditor;
using System.Collections.Generic;

public class PropsMenu : Menu
{

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
    [SerializeField] private GameObject realLevelParent;
    [SerializeField] private MouseHelper mHelper;

    #endregion

    #region Private Fields

    //TODO: Only uses the level block prefab, need to hook in new method
    //to select other items
    private static GameObject levelBlock;
    private GameObject mainLevelObject;
    private float raycastDistance = 1000.0f;
    private Vector3 sceneMousePos;
    private bool initialized = false;
    private Vector3 newItemPos = Vector3.zero;

    private GameObject selectedProp = null;
    private GameObject newWorldProp = null;

    PointerEventData pointerData;

    private bool inputGuard = false;
    #endregion


    #region Unity Lifecycle

    // Update is called once per frame
    private void Update()
    {
        if(inputGuard)
        {
            //we click on a prop?
            if(Input.GetMouseButtonDown(MouseButtons.LEFT) && selectedProp == null)
            {
                selectedProp = RaycastToUI();
                if(selectedProp)
                {
                    newWorldProp = GameObject.Instantiate(selectedProp, realLevelParent.transform, true);
                }
                
            }

            if(newWorldProp != null)
            {
                if(mHelper.currentBlockUnit != null)
                {
                    newWorldProp.transform.position = mHelper.currentBlockUnit.spawnTrans.position;
                }

            }

            if (Input.GetMouseButtonUp(MouseButtons.LEFT) && newWorldProp != null)
            {
                if(mHelper.currentBlockUnit != null)
                {
                    newWorldProp.transform.position = mHelper.currentBlockUnit.spawnTrans.position;
                    mHelper.currentBlockUnit.SetOccupation(true);
                    GameObject.Instantiate(newWorldProp, realLevelParent.transform, true);
                    newWorldProp = null;
                    selectedProp = null;
                }
            }
            
        }
    }

    private void OnMouseDown()
    {
        selectedProp = RaycastToUI();
        newWorldProp = GameObject.Instantiate(selectedProp, realLevelParent.transform, true);
    }

    private void OnMouseUp()
    {
        selectedProp = null;
        newWorldProp = null;
    }


    #endregion

    #region Public Interface

    public PropsMenu() : base(Strings.MenuStrings.ADD_PROPS_PLE)
    { }

    public void Initialize(params object[] par)
    {
        mainLevelObject = EditorToolKit.FindGameObject("LEVEL");
        levelBlock = Resources.Load(Strings.Editor.RESOURCE_PATH + Strings.Editor.BASIC_LVL_BLOCK) as GameObject;

        initialized = true;
    }

    public void AddAction()
    {
        if (mainLevelObject == null)
        {
            Initialize();
        }

        GameObject _instance = GameObject.Instantiate(levelBlock, newItemPos, Quaternion.identity);

        _instance.transform.SetParent(mainLevelObject.transform);

        newItemPos = Vector3.zero;

    }

    #endregion


    #region Protected Interface

    protected override void ShowComplete()
    {
        base.ShowComplete();
        inputGuard = true;

        //draw the grid
        editorInstance.gridController.currentEditorMenu = EditorMenu.PROPS_MENU;

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

    private GameObject RaycastToUI()
    {
        GameObject selectedProp = null;
        UnityEngine.EventSystems.EventSystem mine = UnityEngine.EventSystems.EventSystem.current;

        pointerData = new PointerEventData(UnityEngine.EventSystems.EventSystem.current);

        pointerData.position = Input.mousePosition;

        List<RaycastResult> results = new List<RaycastResult>();

        mine.RaycastAll(pointerData, results);

        if (results.Count > 0)
        {
            foreach (RaycastResult r in results)
            {
                PLEProp currentProp = r.gameObject.GetComponent<PLEProp>();
                if (currentProp)
                {
                    selectedProp = currentProp.registeredProp;
                }
            }
        }



        return selectedProp;
    }

    private void DrawPreviewItemInWorld(Vector3 i_Pos, GameObject i_obj)
    {
        i_obj.transform.position = i_Pos;
        Debug.Log(i_Pos);
    }
    
    private void BackAction()
    {
        MainPLEMenu menu = editorInstance.GetMenu(PLEMenu.MAIN) as MainPLEMenu;

        MenuManager.Instance.DoTransition(menu, Menu.Transition.SHOW, new Menu.Effect[] { Menu.Effect.EXCLUSIVE });
    }

    #endregion


}
