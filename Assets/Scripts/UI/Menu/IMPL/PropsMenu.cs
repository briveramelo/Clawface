//Garin

using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using PlayerLevelEditor;

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
    [SerializeField] private Transform propsParent;
    [SerializeField] private MouseHelper mHelper;

    #endregion

    #region Private Fields
   
    private GameObject selectedProp = null;
    private GameObject newWorldProp = null;
    private PointerEventData pointerData;
    private bool inputGuard = false;
    
    #endregion


    #region Unity Lifecycle

    // Update is called once per frame
    private void Update()
    {
        if(inputGuard)
        {
            if(Input.GetMouseButtonDown(MouseButtons.LEFT) && selectedProp == null)
            {
                selectedProp = RaycastToUI();
                if(selectedProp)
                {
                    newWorldProp = GameObject.Instantiate(selectedProp, propsParent, true);
                }
                
            }

            if(newWorldProp != null)
            {
                if(MouseHelper.currentBlockUnit != null)
                {
                    newWorldProp.transform.position = MouseHelper.currentBlockUnit.spawnTrans.position;
                }

            }

            if (Input.GetMouseButtonUp(MouseButtons.LEFT) && newWorldProp != null)
            {
                if(MouseHelper.currentBlockUnit != null)
                {
                    newWorldProp.transform.position = MouseHelper.currentBlockUnit.spawnTrans.position;
                    MouseHelper.currentBlockUnit.SetOccupation(true);
                    GameObject.Instantiate(newWorldProp, propsParent, true);
                    newWorldProp = null;
                    selectedProp = null;
                }
            }

            if (InputManager.Instance.QueryAction(Strings.Input.UI.CANCEL, ButtonMode.DOWN))
            {
                BackAction();
            }

        }
    }

    private void OnMouseDown()
    {
        selectedProp = RaycastToUI();
        newWorldProp = GameObject.Instantiate(selectedProp, propsParent, true);
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
