using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PlayerLevelEditor
{
    public class LevelEditor : MonoBehaviour
    {
        #region Public Fields
        
        public PlayerLevelEditorGrid gridController;

        #endregion
        

        #region Serialized Unity Fields

        [SerializeField] private CanvasGroup editorCG;
        [SerializeField] private MainPLEMenu mainEditorMenu;
        [SerializeField] private PropsMenu propsEditorMenu;
        [SerializeField] private SpawnMenu spawnsEditorMenu;
        [SerializeField] private FloorMenu dynLevelEditorMenu;
        [SerializeField] private SaveMenu saveEditorMenu;
        [SerializeField] private WaveMenu waveEditorMenu;
        [SerializeField] private HelpMenu helpEditorMenu;
       
        #endregion  

        private void Start()
        {
            editorCG.alpha = 0f;
            editorCG.interactable = false;
            
            MenuSetup();
        }

        void Update()
        {
        }

        #region Private Interface


        private void MenuSetup()
        {
            //Hide menus that aren't need to be shown yet
            MenuManager.Instance.DoTransition(propsEditorMenu, Menu.Transition.HIDE, new Menu.Effect[] { });
            MenuManager.Instance.DoTransition(spawnsEditorMenu, Menu.Transition.HIDE, new Menu.Effect[] { });
            MenuManager.Instance.DoTransition(dynLevelEditorMenu, Menu.Transition.HIDE, new Menu.Effect[] { });
            MenuManager.Instance.DoTransition(saveEditorMenu, Menu.Transition.HIDE, new Menu.Effect[] { });
            MenuManager.Instance.DoTransition(helpEditorMenu, Menu.Transition.HIDE, new Menu.Effect[] { });
            MenuManager.Instance.DoTransition(waveEditorMenu, Menu.Transition.HIDE, new Menu.Effect[] { });

            //show the init menu
            MenuManager.Instance.DoTransition(mainEditorMenu, Menu.Transition.SHOW, new Menu.Effect[] { Menu.Effect.EXCLUSIVE });

        }

        #endregion


        #region Public Interface  
        
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
                case PLEMenu.DYNAMIC:
                    return dynLevelEditorMenu;
                case PLEMenu.WAVE:
                    return waveEditorMenu;
                case PLEMenu.HELP:
                    return helpEditorMenu;
                case PLEMenu.SAVE:
                    return saveEditorMenu;
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
        DYNAMIC,
        SPAWN,
        SAVE,
        HELP,
        WAVE
    }
}
