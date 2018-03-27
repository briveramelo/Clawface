using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using ModMan;

namespace PlayerLevelEditor
{
    public class LevelEditor : MonoBehaviour {        
        
        #region Serialized Unity Fields        
        public PlayerLevelEditorGrid gridController;
        public LevelDataManager levelDataManager;
        [SerializeField] private Transform createdSpawnsParent;
        [SerializeField] private GameObject cameraGameObject;        
        [SerializeField] private PLECameraController cameraController;

        [Header("Editor-Scene Only")]
        [SerializeField] private MainPLEMenu mainPLEMenu;
        #endregion

        #region Private Fields
        #endregion

        #region Public Fields

        public bool IsTesting { get; private set; }

        #endregion

        #region Unity Lifecycle
        private void Start() {
            //EventSystem.Instance.RegisterEvent(Strings.Events.PLE_ON_LEVEL_DATA_LOADED, ResetToWave0);
        }

        private void OnDestroy() {
            if (EventSystem.Instance) {
                //EventSystem.Instance.UnRegisterEvent(Strings.Events.PLE_ON_LEVEL_DATA_LOADED, ResetToWave0);
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

        public void TryCreateAllWaveParents() {
            for (int i = 0; i < PLESpawnManager.Instance.MaxWaveIndex; i++) {
                TryCreateWaveParent(i);
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
        
        public void ExitLevel() {
            TryDestroyPlayer();
            ToggleCameraController(true);
            SetIsTesting(false);
            ObjectPool.Instance.ResetPools();
            EventSystem.Instance.TriggerEvent(Strings.Events.PLE_TEST_END);
            ResetToWave0();
        }

        private void ResetToWave0() {
            gridController.ResetTileHeightsAndStates();
            PLESpawnManager.Instance.SetToWave(0);//sets spawn parent enabling in here
            EventSystem.Instance.TriggerEvent(Strings.Events.PLE_CALL_WAVE, 0);
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

        public void SetIsTesting(bool i_status)
        {
            IsTesting = i_status;
            if (IsTesting) {
                gridController.BakeNavMesh();
            }
        }
        #endregion

        #region Private Interface
        private void TryDestroyPlayer() {
            if (PlayerStateManager.keiraRootGameObject) {
                Helpers.DestroyProper(PlayerStateManager.keiraRootGameObject);
            }
            else {
                GameObject player = GameObject.FindGameObjectWithTag(Strings.Tags.PLAYER);

                if (player) {
                    Helpers.DestroyProper(player.transform.root.gameObject);
                }
                else {
                    player = GameObject.Find("Keira_GroupV1.5(Clone)");
                    if (player) {
                        Helpers.DestroyProper(player.transform.root.gameObject);
                    }
                }
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
