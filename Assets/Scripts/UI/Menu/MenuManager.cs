/**
 *  @author Cornelia Schultz
 */

using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;

public class MenuManager : Singleton<MenuManager> {

    #region Unity Serialization Fields
    [SerializeField]
    private List<GameObject> menuPrefabs;
    private StandaloneInputModule input;
    #endregion

    #region Private Fields
    private List<Menu> menus = new List<Menu>();
    private Queue<TransitionBundle> transitionQueue = new Queue<TransitionBundle>();
    private Stack<Menu> menuStack = new Stack<Menu>();
    #endregion

    #region Unity Lifecycle Functions

    private void Awake()
    {
        input = EventSystem.current.GetComponent<StandaloneInputModule>();
    }
    private void Start()
    {
        
        foreach (GameObject prefab in menuPrefabs)
        {
            GameObject obj = Instantiate(prefab);
            obj.transform.SetParent(gameObject.transform, false);
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
        input.enabled = enabled;
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
        return true;
    }
    public void ClearMenus()
    {
        while (menuStack.Count > 0)
        {
            DoTransition(menuStack.Pop(), Menu.Transition.HIDE, new Menu.Effect[] { });
        }
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
            menuStack.Clear();
        }
        switch (bundle.transition)
        {
            case Menu.Transition.NONE:
            case Menu.Transition.HIDE:
                bundle.menu.canvasGroup.blocksRaycasts = false;
                bundle.menu.canvasGroup.interactable = false;
                break;
            case Menu.Transition.SHOW:
                bundle.menu.canvasGroup.blocksRaycasts = true;
                bundle.menu.canvasGroup.interactable = true;
                menuStack.Push(bundle.menu);
                break;
            case Menu.Transition.TOGGLE:
                if (!bundle.menu.Displayed)
                {
                    bundle.menu.canvasGroup.blocksRaycasts = true;
                    bundle.menu.canvasGroup.interactable = true;
                    menuStack.Push(bundle.menu);
                } else
                {
                    bundle.menu.canvasGroup.blocksRaycasts = false;
                    bundle.menu.canvasGroup.interactable = false;
                }
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
