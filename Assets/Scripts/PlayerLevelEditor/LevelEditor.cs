using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace PlayerLevelEditor
{
    public class LevelEditor : MonoBehaviour
    {
        #region Public Fields
        
        public PLEMenu currentDisplayedMenu;
        public PlayerLevelEditorGrid gridController;

        #endregion
        

        #region Serialized Unity Fields
        
        [SerializeField] private MainPLEMenu mainEditorMenu;
        [SerializeField] private FloorMenu floorEditorMenu;
        [SerializeField] private PropsMenu propsEditorMenu;
        [SerializeField] private SpawnMenu spawnsEditorMenu;
        [SerializeField] private WaveMenu waveEditorMenu;
        [SerializeField] private TestMenu testEditorMenu;
        [SerializeField] private SaveMenu saveEditorMenu;
        [SerializeField] private PLELevelSelectMenu levelSelectEditorMenu;
        [SerializeField] private HelpMenu helpEditorMenu;
        private List<Menu> pleMenus;
        //[SerializeField] private Button initialMenuButton;

        #endregion

        #region Private Fields

        #endregion

        private void Start() {
            pleMenus = new List<Menu>() {
                mainEditorMenu,
                floorEditorMenu,
                propsEditorMenu,
                spawnsEditorMenu,
                waveEditorMenu,
                testEditorMenu,
                saveEditorMenu,
                levelSelectEditorMenu,
                helpEditorMenu
            };
            MenuSetup();
        }

        void Update()
        {

        }


        #region Private Interface


        private void MenuSetup()
        {
            //Hide menus that aren't need to be shown yet
            pleMenus.ForEach(menu => {
                MenuManager.Instance.DoTransition(menu, Menu.Transition.HIDE, new Menu.Effect[] { Menu.Effect.INSTANT });
                menu.Canvas.SetActive(false);
                menu.CanvasGroup.alpha = 0.0F;
                menu.CanvasGroup.blocksRaycasts = false;
                menu.CanvasGroup.interactable = false;
            });

            //show the main/floor menus
            MenuManager.Instance.DoTransition(mainEditorMenu, Menu.Transition.SHOW, new Menu.Effect[] { Menu.Effect.INSTANT });
            MenuManager.Instance.DoTransition(floorEditorMenu, Menu.Transition.SHOW, new Menu.Effect[] { Menu.Effect.INSTANT });

            currentDisplayedMenu = PLEMenu.FLOOR;
            mainEditorMenu.OpenFloorSystemAction();
            gridController.SetGridVisiblity(true);
        }

        #endregion


        #region Public Interface  


        public void SwitchToMenu(PLEMenu i_newMenu)
        {
            
            if (i_newMenu!=currentDisplayedMenu) {                
                if (currentDisplayedMenu != PLEMenu.NONE) {
                    Menu menuToHide = GetMenu(currentDisplayedMenu);
                    MenuManager.Instance.DoTransition(menuToHide, Menu.Transition.HIDE, new Menu.Effect[] { });
                }

                Menu newMenu = GetMenu(i_newMenu);
                MenuManager.Instance.DoTransition(newMenu, Menu.Transition.SHOW, new Menu.Effect[] { });

                currentDisplayedMenu = i_newMenu;
            }
        }
        
        public void UsingQuitFunction(Button thisBtn)
        {
            Menu menu = MenuManager.Instance.GetMenuByName(Strings.MenuStrings.LOAD);
            LoadMenu loadMenu = menu as LoadMenu;
            loadMenu.TargetScene = Strings.Scenes.MainMenu;

            MenuManager.Instance.DoTransition(loadMenu,Menu.Transition.SHOW, new Menu.Effect[] { Menu.Effect.EXCLUSIVE });
        }

        public Menu GetMenu(PLEMenu i_menu)
        {
            switch (i_menu)
            {
                case PLEMenu.MAIN:
                    return mainEditorMenu;
                case PLEMenu.PROPS:
                    return propsEditorMenu;
                case PLEMenu.SPAWN:
                    return spawnsEditorMenu;
                case PLEMenu.FLOOR:
                    return floorEditorMenu;
                case PLEMenu.WAVE:
                    return waveEditorMenu;
                case PLEMenu.HELP:
                    return helpEditorMenu;
                case PLEMenu.SAVE:
                    return saveEditorMenu;
                case PLEMenu.TEST:
                    return testEditorMenu;
                case PLEMenu.LEVELSELECT:
                    return levelSelectEditorMenu;
                default:
                    return null;
            }
        }

        
        

        #endregion  
    }

    class NavMeshAreas
    {
        public const int Walkable = 0;
        public const int NotWalkable = 1;
        public const int Jump = 2;
    }

    public enum PLEMenu
    {
        INIT,
        MAIN,
        PROPS,
        FLOOR,
        SPAWN,
        SAVE,
        HELP,
        WAVE,
        TEST,
        LEVELSELECT,
        NONE
    }
}
