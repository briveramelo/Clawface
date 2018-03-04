using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace PlayerLevelEditor
{
    public class LevelEditor : MonoBehaviour {

        [Header("Persistent Fields")]
        [SerializeField] private GameObject playerSpawnerPrefab;
        
        public PlayerLevelEditorGrid gridController;
        public LevelDataManager levelDataManager;
        [HideInInspector] public bool hasCreatedPlayer;
        [SerializeField] private Transform createdSpawnsParent;
        [Header("Player Level Editor-Scene Specific Fields")]
        public PLEMenu currentDisplayedMenu;
        public WaveSystem waveSystem;

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

        #region Unity Lifecycle

        private void Start() {

            EventSystem.Instance.RegisterEvent(Strings.Events.PLE_CHANGEWAVE, EnableSpawnsOnWaveChange);
            EventSystem.Instance.RegisterEvent(Strings.Events.LEVEL_STARTED, PlayLevel);

            if (SceneTracker.IsCurrentSceneEditor) {
                SetUpMenus();
            }
        }

        private void OnDestroy()
        {
            if(EventSystem.Instance)
            {
                EventSystem.Instance.UnRegisterEvent(Strings.Events.PLE_CHANGEWAVE, EnableSpawnsOnWaveChange);
                EventSystem.Instance.UnRegisterEvent(Strings.Events.LEVEL_STARTED, PlayLevel);
            }
            
        }


        #endregion

        
        #region Public Interface
        public void EnableSpawnsOnWaveChange(params object[] parameters)
        {
            string activeWaveName = "";
            if(parameters.Length > 0)
            {
                int wave = (int)parameters[0];
                activeWaveName = GetWaveName(wave);
            }


            for (int i = 1; i < createdSpawnsParent.childCount; i++)
            {
                //Accounts for not disabling the player spawn object between switching of waves.
                GameObject currentGO = createdSpawnsParent.GetChild(i).gameObject;

                if (!currentGO.CompareTag(Strings.Editor.PLAYER_SPAWN_TAG))
                {
                    currentGO.SetActive(false);
                }
            }
            Transform activeWave = createdSpawnsParent.Find(activeWaveName);
            if (activeWave != null)
            {
                activeWave.gameObject.SetActive(true);
            }
        }


        public string GetWaveName(int i) { return Strings.Editor.Wave + i; }

        public void PlayLevel(params object[] i_params) {
            SpawnMenu.playerSpawnInstance.SetActive(false);

            playerSpawnerInstance = Instantiate(playerSpawnerPrefab);
            playerSpawnerInstance.transform.position = SpawnMenu.playerSpawnInstance.transform.position;

            if (SceneTracker.IsCurrentSceneEditor) {
                waveSystem.ResetToWave0();
            }
            else {
                PLESpawnManager.Instance.CurrentWave = 0;
            }
            hasCreatedPlayer = true;
        }
        public void ExitLevel() {
            GameObject playerInstance = null;
            if (playerSpawnerInstance != null) {
                playerInstance = playerSpawnerInstance.GetComponent<PlayerSpawner>().GetplayerPrefabGO();

                DestroyImmediate(playerSpawnerInstance);
                playerSpawnerInstance = null;
            }

            if (playerInstance != null) {
                DestroyImmediate(playerInstance);
                playerInstance = null;
            }

            if (SpawnMenu.playerSpawnInstance != null) {
                SpawnMenu.playerSpawnInstance.SetActive(true);
            }

            EventSystem.Instance.TriggerEvent(Strings.Events.PLE_TEST_END);
            hasCreatedPlayer = false;
        }

        public void CheckToSetMenuInteractability() {
            bool isInteractable = gridController.AnyTilesEnabled();
            ToggleMenuInteractable(isInteractable, PLEMenu.PROPS, PLEMenu.SPAWN, PLEMenu.WAVE);

            isInteractable = SpawnMenu.playerSpawnInstance != null;
            ToggleMenuInteractable(isInteractable, PLEMenu.SAVE, PLEMenu.TEST);
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
            loadMenu.TargetScenePath = Strings.Scenes.ScenePaths.MainMenu;

            MenuManager.Instance.DoTransition(loadMenu,Menu.Transition.SHOW, new Menu.Effect[] { Menu.Effect.EXCLUSIVE });
        }

        public void ToggleCameraController(bool isEnabled) {
            cameraController.enabled=isEnabled;
            if (isEnabled) {
                ToggleCameraGameObject(true);
            }
        }
        public void ToggleCameraGameObject(bool isEnabled) {
            cameraController.gameObject.SetActive(isEnabled);
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

        #region Private Interface
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
