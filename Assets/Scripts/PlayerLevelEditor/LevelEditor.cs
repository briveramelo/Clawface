using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PlayerLevelEditor
{
    public class LevelEditor : MonoBehaviour
    {
        #region Public Fields

        static public DynamicLevelSystem m_DynamicLevelSystem;
        public PlayerLevelEditorGrid gridController;

        #endregion

        Database ObjectDB;
        FunctionController controller = new FunctionController();

        Button Btn_Init;
        Button Btn_Add;
        Button Btn_Duplicate;
        Button Btn_Dynamic;
        Button Btn_Test;
        Button Btn_EndTest;
        Button Btn_Quit;

        #region Serialized Unity Fields

        [SerializeField] private CanvasGroup editorCG;
        [SerializeField] private MainPLEMenu mainEditorMenu;
        [SerializeField] private PropsMenu propsEditorMenu;
        [SerializeField] private SpawnMenu spawnsEditorMenu;
        [SerializeField] private FloorMenu dynLevelEditorMenu;
        
       
        #endregion  

        private void Start()
        {
            editorCG.alpha = 0f;
            editorCG.interactable = false;
            
            if(EventSystem.Instance)
            {
                EventSystem.Instance.RegisterEvent(Strings.Events.INIT_EDITOR, EditorInitialize);
            }
        }

        void Update()
        {
            if (controller != null)
            {
                controller.Update();
            }
            
        }


        private void OnDestroy()
        {
            if (EventSystem.Instance)
            {
                EventSystem.Instance.UnRegisterEvent(Strings.Events.INIT_EDITOR, EditorInitialize);
            }
        }

        #region Private Interface


        private void MenuSetup()
        {
            //Hide menus that aren't need to be shown yet
            MenuManager.Instance.DoTransition(propsEditorMenu, Menu.Transition.HIDE, new Menu.Effect[] { });
            MenuManager.Instance.DoTransition(spawnsEditorMenu, Menu.Transition.HIDE, new Menu.Effect[] { });
            MenuManager.Instance.DoTransition(dynLevelEditorMenu, Menu.Transition.HIDE, new Menu.Effect[] { });

            //show the init menu
            MenuManager.Instance.DoTransition(mainEditorMenu, Menu.Transition.SHOW, new Menu.Effect[] { Menu.Effect.EXCLUSIVE });

        }

        #endregion


        #region Public Interface  
        // Use this for initialization
        public void EditorInitialize(params object[] par)
        {
            m_DynamicLevelSystem = new DynamicLevelSystem();
            dynLevelEditorMenu.Initialize();
            //ObjectDB = new Database();

            //controller.SetFunction(new Initialize(controller));

            MenuSetup();

        }


        public void UseInitFunc(Button thisBtn)
        {
            controller.SetFunction(new Initialize(controller));
        }

        public void UsingAddFunc(Button thisBtn)
        {
            controller.SetFunction(new Add(controller));
        }

        public void UsingDuplicateFunc(Button thisBtn)
        {
            controller.SetFunction(new Duplicate(controller));
        }

        public void UsingDynamicFunc(Button thisBtn)
        {
            controller.SetFunction(new DynamicLevel(controller));
        }

        public void UsingTestFunc(Button thisBtn)
        {
            if (Initialize.IsDone() == false)
                return;

            controller.SetFunction(new Test(controller));
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
                //case PLEMenu.INIT:
                //    return initEditorMenu;
                case PLEMenu.MAIN:
                    return mainEditorMenu;
                case PLEMenu.PROPS:
                    return propsEditorMenu;
                case PLEMenu.SPAWN:
                    return spawnsEditorMenu;
                case PLEMenu.DYNAMIC:
                    return dynLevelEditorMenu;
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
        SPAWN
    }
}
