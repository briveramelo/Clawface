/**
 *  @author Cornelia Schultz
 */

using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using Rewired.Integration.UnityUI;


using UnityES = UnityEngine.EventSystems.EventSystem;

public class MenuManager : Singleton<MenuManager> {

    #region Accessors (Internal)

    internal bool MouseMode
    {
        get
        {
            return mouseMode;
        }
        private set
        {
            mouseMode = value;
            Cursor.visible = mouseMode;
            if (mouseMode)
            {
                eventSystem.SetSelectedGameObject(null);
            } else
            {
                StartCoroutine(SelectNextFrame());
            }
        }
    }

    #endregion

    #region Unity Serialization Fields
    [SerializeField]
    private List<GameObject> menuPrefabs;
    [SerializeField]
    private UnityES eventSystem;
    [SerializeField]
    private RewiredStandaloneInputModule input;
    [SerializeField]
    private Button deadNavButton;
    #endregion

    #region Private Fields
    private List<Menu> menus = new List<Menu>();
    private Queue<TransitionBundle> transitionQueue = new Queue<TransitionBundle>();
    private List<Menu> menuStack = new List<Menu>();
    private bool mouseMode = true;
    #endregion

    #region Unity Lifecycle Functions
    protected override void Awake()
    {
        base.Awake();
        foreach (GameObject prefab in menuPrefabs)
        {
            GameObject obj = Instantiate(prefab, gameObject.transform, false);
            Menu menu = obj.GetComponent<Menu>();

            // Make sure incoming menus don't interfere with each other and
            // are hidden 
            menu.Canvas.SetActive(false);
            menu.CanvasGroup.alpha = 0.0F;
            menu.CanvasGroup.blocksRaycasts = false;
            menu.CanvasGroup.interactable = false;

            menus.Add(menu);
        }
    }

    protected override void Start()
    {
        base.Start();
        MouseMode = !InputManager.Instance.HasJoystick();
    }

    private void Update()
    {
        if (transitionQueue.Count > 0)
        {
            EnableEventSystem(false);
            while (transitionQueue.Count > 0)
            {
                TransitionBundle bundle = transitionQueue.Dequeue();
                ProcessBundle(bundle);
                bundle.menu.DoTransition(bundle.transition, bundle.effects);
            }
            EnableEventSystem(true);
        }

        // Check if we should switch between mouse mode or not
        bool mouseInput = InputManager.Instance.Player.controllers.Mouse.GetAnyButton();
        if (!mouseMode && (InputManager.Instance.Player.controllers.Mouse.GetAnyButtonDown()
            || Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0))
        {
            MouseMode = true;
        } else if (!mouseInput && mouseMode && (
            InputManager.Instance.Player.controllers.Joysticks.Any((joystick) => joystick.GetAnyButton()) ||
            InputManager.Instance.Player.controllers.Joysticks.Any((joystick) => joystick.Axes.Any((axis) => axis.value != 0))
            ))
        {
            MouseMode = false;
        }

        // Coerce back to keyboard navigation if necessary
        if ((UnityES.current.currentSelectedGameObject == null || UnityES.current.currentSelectedGameObject == deadNavButton.gameObject) 
            && InputManager.Instance.QueryAxes(Strings.Input.UI.NAVIGATION).sqrMagnitude > 0)
        {
            StartCoroutine(SelectNextFrame());
        }
    }
    #endregion

    #region Public Interface
    public void DoTransition(string menuName, Menu.Transition transition,
            Menu.Effect[] effects)
    {
        Menu menu = GetMenuByName(menuName);
        Assert.IsNotNull(menu);
        transitionQueue.Enqueue(new TransitionBundle(menu, transition, effects));
    }
    public void DoTransition(Menu menu, Menu.Transition transition,
            Menu.Effect[] effects)
    {
        transitionQueue.Enqueue(new TransitionBundle(menu, transition, effects));
    }
    public void DoTransitionOthers(string menuName, Menu.Transition transition,
            Menu.Effect[] effects)
    {
        Menu menu = GetMenuByName(menuName);
        Assert.IsNotNull(menu);
        DoTransitionOthers(menu, transition, effects);
    }
    public void DoTransitionOthers(Menu menu, Menu.Transition transition,
            Menu.Effect[] effects)
    {
        foreach (Menu other in menus)
        {
            if (menu == other) continue;
            transitionQueue.Enqueue(new TransitionBundle(other, transition, effects));
        }
    }

    public void EnableEventSystem(bool enabled)
    {
        if (input)
        { 
            input.enabled = enabled;
        }
    }

    public bool PopMenu(bool removeRoot = false)
    {
        switch (menuStack.Count) {
            case 0:
                return false;
            case 1:
                if (!removeRoot)
                {
                    return false;
                }
                break;
        }
        Menu menu = menuStack[0];
        DoTransition(menu, Menu.Transition.HIDE, new Menu.Effect[] { });
        menuStack.RemoveAt(0);

        // Reassign Selection
        if (menuStack.Count != 0)
        {
            if (menuStack[0].InitialSelection != null)
            {
                menuStack[0].InitialSelection.Select();
            }
        } else
        {
            deadNavButton.gameObject.SetActive(true);
            deadNavButton.Select();
        }

        return true;
    }
    public void ClearMenus()
    {
        while (PopMenu(true)) { } // clear stack
    }

    public Menu GetMenuByName(string menuName)
    {
        return menus.Find((cmp) => { return menuName == cmp.MenuName; });
    }
    #endregion

    #region Private Interface
    private MenuManager() { }

    private void ProcessBundle(TransitionBundle bundle)
    {
        if (bundle.effects.Contains(Menu.Effect.EXCLUSIVE))
        {
            ClearMenus();
        }
        if (bundle.menu == null) return;
        switch (bundle.transition)
        {
            case Menu.Transition.HIDE:
                bundle.menu.CanvasGroup.blocksRaycasts = false;
                bundle.menu.CanvasGroup.interactable = false;
                menuStack.RemoveAll((cmp) => { return cmp.MenuName == bundle.menu.MenuName; });
                break;
            case Menu.Transition.SHOW:
                deadNavButton.gameObject.SetActive(false);
                bundle.menu.CanvasGroup.blocksRaycasts = true;
                bundle.menu.CanvasGroup.interactable = true;
                menuStack.Add(bundle.menu);
                break;
            case Menu.Transition.TOGGLE:
                // Do nothing, a SHOW or HIDE will come through soon.
                break;
            case Menu.Transition.SPECIAL:
                // There's nothing to do.. we don't know if a show is coming or not..
                break;
        }
    }

    private void SelectInitialButtonIfPossible()
    {
        if (menuStack.Count > 0)
        {
            Menu active = menuStack[menuStack.Count - 1];
            active.SelectInitialButton();
        }
    }

    private System.Collections.IEnumerator SelectNextFrame()
    {
        yield return null;
        SelectInitialButtonIfPossible();
    }

    #endregion

    #region Types
    private struct TransitionBundle
    {
        public TransitionBundle(Menu menu, Menu.Transition transition,
                Menu.Effect[] effects)
        {
            this.menu = menu;
            this.transition = transition;
            this.effects = effects;
        }

        public Menu menu;
        public Menu.Transition transition;
        public Menu.Effect[] effects;
    }
    #endregion
}
