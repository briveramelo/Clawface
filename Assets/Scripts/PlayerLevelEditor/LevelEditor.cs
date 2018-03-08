using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using ModMan;

namespace PlayerLevelEditor
{
    public class LevelEditor : MonoBehaviour {

        [Header("Persistent Fields")]
        [SerializeField] private GameObject playerSpawnerPrefab;
        
        public PlayerLevelEditorGrid gridController;
        public LevelDataManager levelDataManager;
        [SerializeField] private Transform createdSpawnsParent;

        [Header("Required")]
        [SerializeField] private GameObject cameraGameObject;
        [Header("Player Level Editor-Scene Specific Fields")]
        public PLEMenu currentDisplayedMenu;

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
        [SerializeField] private PLECameraController cameraController;

        
        #endregion

        #region Private Fields
        private List<Menu> pleMenus = new List<Menu>();
        private GameObject playerSpawnerInstance = null;
        #endregion

        #region Public Fields

        public bool IsTesting { get; private set; }

        #endregion

        #region Unity Lifecycle

        private void Start() {            
            EventSystem.Instance.RegisterEvent(Strings.Events.LEVEL_STARTED, PlayLevel);

            if (SceneTracker.IsCurrentSceneEditor) {
                SetUpMenus();
            }
        }

        private void OnDestroy()
        {
            if(EventSystem.Instance)
            {
                EventSystem.Instance.UnRegisterEvent(Strings.Events.LEVEL_STARTED, PlayLevel);
            }
            
        }


        #endregion

        
        #region Public Interface
        public string GetWaveName(int i) { return Strings.Editor.Wave + i; }
        public void EnableCurrentWaveSpawnParents(params object[] parameters)
        {
            createdSpawnsParent.SortChildrenByName();
            int currentWaveIndex = 0;
            if (parameters.Length > 0) {
                currentWaveIndex = (int)parameters[0];
            }
            else {
                currentWaveIndex = PLESpawnManager.Instance.CurrentWaveIndex;
            }

            int startIndex = SpawnMenu.playerSpawnInstance == null ? 0 : 1;
            for (int i = startIndex; i < createdSpawnsParent.childCount; i++)
            {
                GameObject waveParent = createdSpawnsParent.GetChild(i).gameObject;
                if (!waveParent.CompareTag(Strings.Editor.PLAYER_SPAWN_TAG)) {
                    waveParent.SetActive(false);
                }
            }

            string activeWaveName = GetWaveName(currentWaveIndex);
            Transform currentWaveParent = createdSpawnsParent.Find(activeWaveName);
            if (currentWaveParent != null)
            {
                currentWaveParent.gameObject.SetActive(true);
            }
        }

        public Transform TryCreateWaveParent(int i) {
            string waveName = GetWaveName(i);
            Transform waveParent = createdSpawnsParent.Find(waveName);
            if (waveParent == null) {
                waveParent = new GameObject(waveName).transform;
                waveParent.SetParent(createdSpawnsParent);
            }
            return waveParent;
        }

        public void ResetToWave0() {
            waveEditorMenu.ResetToWave0();
        }

        public void PlayLevel(params object[] i_params) {
            if (SceneTracker.IsCurrentSceneEditor) {
                ResetToWave0();
            }
        }
        public void ExitLevel() {
            ToggleCameraController(true);
            if (SpawnMenu.playerSpawnInstance != null) {
                SpawnMenu.playerSpawnInstance.SetActive(true);
            }
            SetIsTesting(false);
            EventSystem.Instance.TriggerEvent(Strings.Events.PLE_TEST_END);
        }

        public void SetMenuButtonInteractability() {
            bool anyTilesOn = gridController.AnyTilesEnabled();
            ToggleMenuInteractable(anyTilesOn, PLEMenu.PROPS, PLEMenu.SPAWN, PLEMenu.WAVE);

            bool anyTilesOnAndPlayerOn = anyTilesOn && SpawnMenu.playerSpawnInstance != null;
            ToggleMenuInteractable(anyTilesOnAndPlayerOn, PLEMenu.SAVE, PLEMenu.TEST);
        }
        

        public void SwitchToMenu(PLEMenu i_newMenu) {
            
            if (i_newMenu!=currentDisplayedMenu) {                
                if (currentDisplayedMenu != PLEMenu.NONE) {
                    Menu menuToHide = GetMenu(currentDisplayedMenu);
                    MenuManager.Instance.DoTransition(menuToHide, Menu.Transition.HIDE, new Menu.Effect[] { });
                }

                Menu newMenu = GetMenu(i_newMenu);
                MenuManager.Instance.DoTransition(newMenu, Menu.Transition.SHOW, new Menu.Effect[] { });

                currentDisplayedMenu = i_newMenu;

                if (currentDisplayedMenu == PLEMenu.SAVE || currentDisplayedMenu == PLEMenu.LEVELSELECT) {
                    ToggleCameraController(false);
                }
                else {
                    ToggleCameraController(true);
                }
                mainEditorMenu.SetMenuToggleOn(currentDisplayedMenu);
            }
        }
        
        public void UsingQuitFunction(Button thisBtn)
        {
            Menu menu = MenuManager.Instance.GetMenuByName(Strings.MenuStrings.LOAD);
            LoadMenu loadMenu = menu as LoadMenu;
            loadMenu.SetNavigation(Strings.Scenes.ScenePaths.MainMenu);

            MenuManager.Instance.DoTransition(loadMenu,Menu.Transition.SHOW, new Menu.Effect[] { Menu.Effect.EXCLUSIVE });
        }

        public void ToggleCameraController(bool isEnabled) {
            cameraController.enabled=isEnabled;
            if (isEnabled) {
                ToggleCameraGameObject(true);
            }
        }
        public void ToggleCameraGameObject(bool isEnabled) {
            cameraGameObject.SetActive(isEnabled);
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

        public void SetUpMenus() {
            pleMenus.Clear();
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
            ToggleCameraController(true);
        }

        public bool GetIsTesting()
        {
            return IsTesting;
        }

        public void SetIsTesting(bool i_status)
        {
            IsTesting = i_status;
        }
        #endregion

        #region Private Interface


        void ToggleMenuInteractable(bool isInteractable, params PLEMenu[] menus) {
            foreach (PLEMenu menu in menus) {
                mainEditorMenu.ToggleMenuInteractable(menu, isInteractable);
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
        EXIT,
        NONE
    }
}
