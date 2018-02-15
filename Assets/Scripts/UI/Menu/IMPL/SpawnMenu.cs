using System.Collections.Generic;

using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine;

using ModMan;
using PlayerLevelEditor;


public class SpawnMenu : Menu {

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
    [SerializeField] private Transform spawnParent;

    #endregion

    #region Private Fields

    private GameObject selectedSpawn = null;
    private GameObject previewSpawn = null;
    private PointerEventData pointerData;
    private bool inputGuard = false;

    #endregion

    #region Boolean Helpers

    bool SelectUI { get { return Input.GetMouseButtonDown(MouseButtons.LEFT) && ScrollGroupHelper.currentSpawn != null; } }
    bool DeselectUI { get { return Input.GetMouseButtonDown(MouseButtons.RIGHT); } }
    bool Place { get { return Input.GetMouseButtonDown(MouseButtons.LEFT) && selectedSpawn != null && MouseHelper.currentBlockUnit != null && !MouseHelper.currentBlockUnit.GetOccupation(); } }
    bool UpdatePreview { get { return previewSpawn != null && MouseHelper.currentBlockUnit != null && !MouseHelper.currentBlockUnit.GetOccupation(); } }

    #endregion

    #region Unity Lifecycle

    private void Update()
    {
        if(inputGuard)
        {
            if (SelectUI)
            {
                SelectUIItem();
            }
            else if (Place)
            {
                PlaceSpawn();
            }
            else if (DeselectUI)
            {
                DeselectUIItem();
            }
            else if (UpdatePreview)
            {
                UpdatePreviewPosition();
            }
            //TODO: Make function for delete selected item

            if (InputManager.Instance.QueryAction(Strings.Input.UI.CANCEL, ButtonMode.UP))
            {
                BackAction();
            }
            
        }
    }

    #endregion  

    #region Public Interface

    public SpawnMenu() : base(Strings.MenuStrings.ADD_SPAWNS_PLE)
    { }

    #endregion
    
    #region Protected Interface

    protected override void ShowComplete()
    {
        base.ShowComplete();
        inputGuard = true;
        editorInstance.gridController.currentEditorMenu = EditorMenu.SPAWN_MENU;
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

    private void SelectUIItem()
    {
        PLESpawn currentSpawn = ScrollGroupHelper.currentSpawn;

        if(currentSpawn)
        {
            selectedSpawn = currentSpawn.registeredSpawner;
            previewSpawn = GameObject.Instantiate(selectedSpawn);
        }
    }

    private void UpdatePreviewPosition()
    {
        previewSpawn.transform.position = MouseHelper.currentBlockUnit.spawnTrans.position;
    }

    private void DeselectUIItem()
    {
        selectedSpawn = null;
        TryDestroyPreview();
    }

    private void TryDestroyPreview()
    {
        if(previewSpawn)
        {
            Helpers.DestroyProper(previewSpawn);
        }
    }

    private void PlaceSpawn()
    {
        GameObject nextWorldSpawn = Instantiate(selectedSpawn, spawnParent);
        nextWorldSpawn.transform.position = MouseHelper.currentBlockUnit.spawnTrans.position;
        nextWorldSpawn.transform.SetParent(MouseHelper.currentBlockUnit.spawnTrans);
        nextWorldSpawn.name = selectedSpawn.name.TryCleanClone();
        MouseHelper.currentBlockUnit.SetOccupation(true);
    }
    
    private void BackAction()
    {
        MainPLEMenu menu = editorInstance.GetMenu(PLEMenu.MAIN) as MainPLEMenu;
        MenuManager.Instance.DoTransition(menu, Menu.Transition.SHOW, new Menu.Effect[] { Menu.Effect.EXCLUSIVE });
    }

    #endregion

}
