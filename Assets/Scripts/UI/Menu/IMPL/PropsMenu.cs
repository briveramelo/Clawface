//Garin

using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using PlayerLevelEditor;
using ModMan;

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

    #endregion

    #region Private Fields
   
    private GameObject selectedProp = null;
    private GameObject previewProp = null;
    private PointerEventData pointerData;
    private bool inputGuard = false;
    
    #endregion

    bool SelectUI { get { return Input.GetMouseButtonDown(MouseButtons.LEFT) && RaycastToUI() !=null; } }
    bool DeselectUI { get { return Input.GetMouseButtonDown(MouseButtons.RIGHT); } }
    bool Place { get { return Input.GetMouseButtonDown(MouseButtons.LEFT) && selectedProp != null && MouseHelper.currentBlockUnit != null && !MouseHelper.currentBlockUnit.GetOccupation(); } }
    bool UpdatePreview { get { return previewProp != null && MouseHelper.currentBlockUnit!=null && !MouseHelper.currentBlockUnit.GetOccupation(); } }
   
    #region Unity Lifecycle

    // Update is called once per frame
    private void Update()
    {
        if(inputGuard)
        {
            if (SelectUI) {
                SelectUIItem();
            }
            else if (Place) {
                PlaceProp();
            }
            else if (DeselectUI) {
                DeselectUIItem();
            }
            else if (UpdatePreview) {
                UpdatePreviewPosition();
            }
            //TODO: Make function for delete selected item

            if (InputManager.Instance.QueryAction(Strings.Input.UI.CANCEL, ButtonMode.UP))
            {
                BackAction();
            }

        }
    }

    void SelectUIItem() {
        selectedProp = ScrollGroupHelper.RaycastToScrollGroup();
        if (selectedProp)
        {
            TryDestroyPreview();
            previewProp = GameObject.Instantiate(selectedProp);
        }
    }

    void UpdatePreviewPosition() {
        previewProp.transform.position = MouseHelper.currentBlockUnit.spawnTrans.position;
    }

    void DeselectUIItem() {
        selectedProp = null;
        TryDestroyPreview();
    }

    void PlaceProp() {
        GameObject nextWorldProp = Instantiate(selectedProp, propsParent);
        nextWorldProp.transform.position = MouseHelper.currentBlockUnit.spawnTrans.position;
        nextWorldProp.transform.SetParent(MouseHelper.currentBlockUnit.spawnTrans);
        nextWorldProp.name = selectedProp.name.TryCleanClone();
        MouseHelper.currentBlockUnit.SetOccupation(true);
    }

    void TryDestroyPreview() {
        if (previewProp) {
            Helpers.DestroyProper(previewProp);
        }
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
        TryDestroyPreview();
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


    private void BackAction()
    {
        MainPLEMenu menu = editorInstance.GetMenu(PLEMenu.MAIN) as MainPLEMenu;

        MenuManager.Instance.DoTransition(menu, Menu.Transition.SHOW, new Menu.Effect[] { Menu.Effect.EXCLUSIVE });
    }

    #endregion


}
