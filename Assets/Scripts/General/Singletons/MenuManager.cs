/**
 *  @author Cornelia Schultz
 */

using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using Rewired.Integration.UnityUI;

public class MenuManager : Singleton<MenuManager> {

    #region Unity Serialization Fields
    [SerializeField]
    private List<GameObject> menuPrefabs;
    [SerializeField]
    private RewiredStandaloneInputModule input;
    [SerializeField]
    private Button deadNavButton;
    #endregion

    #region Private Fields
    private List<Menu> menus = new List<Menu>();
    private Queue<TransitionBundle> transitionQueue = new Queue<TransitionBundle>();
    private Stack<Menu> menuStack = new Stack<Menu>();
    #endregion

    #region Unity Lifecycle Functions
    protected override void Awake()
    {
        base.Awake();
        foreach (GameObject prefab in menuPrefabs)
        {
            GameObject obj = Instantiate(prefab, gameObject.transform, false);
            Menu menu = obj.GetComponent<Menu>();
            menus.Add(menu);
        }
    }
    void Update()
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
        DoTransition(menuStack.Pop(), Menu.Transition.HIDE, new Menu.Effect[] { });

        // Reassign Selection
        if (menuStack.Count != 0)
        {
            Menu menu = menuStack.Peek();
            if (menu.InitialSelection != null)
            {
                menu.InitialSelection.Select();
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
        switch (bundle.transition)
        {
            case Menu.Transition.HIDE:
                bundle.menu.CanvasGroup.blocksRaycasts = false;
                bundle.menu.CanvasGroup.interactable = false;
                break;
            case Menu.Transition.SHOW:
                deadNavButton.gameObject.SetActive(false);
                bundle.menu.CanvasGroup.blocksRaycasts = true;
                bundle.menu.CanvasGroup.interactable = true;
                menuStack.Push(bundle.menu);
                break;
            case Menu.Transition.TOGGLE:
                // Do nothing, a SHOW or HIDE will come through soon.
                break;
            case Menu.Transition.SPECIAL:
                // There's nothing to do.. we don't know if a show is coming or not..
                break;
        }
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
