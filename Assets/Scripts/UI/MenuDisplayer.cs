using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class MenuDisplayer : MonoBehaviour {

    [SerializeField, HideInInspector] private Menu.MenuType displayedMenu;
    List<Menu> allMenus = new List<Menu>();
    public void ShowMenuExclusive(Menu.MenuType genericMenu) {
        allMenus = (Resources.FindObjectsOfTypeAll(typeof(Menu)) as Menu[]).ToList();
        Menu menuToShow = allMenus.Find(menu => menu.ThisMenuType == genericMenu);
        allMenus.ForEach(menu => menu.gameObject.SetActive(menu == menuToShow));
    }
}