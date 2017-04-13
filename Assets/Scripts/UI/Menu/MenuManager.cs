/**
 *  @author Cornelia Schultz
 */

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class MenuManager : Singleton<MenuManager> {

    #region Unity Serialization Fields
    [SerializeField]
    private List<GameObject> menuPrefabs;
    #endregion

    #region Private Fields
    private List<Menu> menus;
    private Queue<TransitionBundle> transitionQueue = new Queue<TransitionBundle>();
    #endregion

    #region Unity Lifecycle Functions
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
        while (transitionQueue.Count > 0)
        {
            TransitionBundle bundle = transitionQueue.Dequeue();
            bundle.menu.DoTransition(bundle.transition, bundle.effects);
        }
    }
    #endregion

    #region Public Interface
    public void DoTransition(string menuName, Menu.Transition transition,
            Menu.Effect[] effects)
    {
        Menu menu = menus.Find((cmp) => { return menuName == cmp.MenuName; });
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
        Menu menu = menus.Find((cmp) => { return menuName == cmp.MenuName; });
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
    #endregion

    #region
    protected MenuManager() { }
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
