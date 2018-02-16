//Garin + Brandon
using UnityEngine.EventSystems;
using UnityEngine;
public class SpawnMenu : PlacementMenu {

    #region Public Interface
    public SpawnMenu() : base(Strings.MenuStrings.ADD_SPAWNS_PLE) { }
    #endregion

    #region Private Fields
    private PointerEventData pointerData;
    #endregion

    private void Awake() {
        EventSystem.Instance.RegisterEvent(Strings.Events.PLE_CHANGEWAVE, OnWaveChange);
    }
    private void OnDestroy() {
        if (EventSystem.Instance) {
            EventSystem.Instance.UnRegisterEvent(Strings.Events.PLE_CHANGEWAVE, OnWaveChange);
        }
    }

    void OnWaveChange(params object[] parameters) {
        string activeWaveName = GetWaveName(WaveSystem.currentWave);
        for (int i = 0; i < createdItemsParent.childCount; i++) {
            createdItemsParent.GetChild(i).gameObject.SetActive(false);
        }
        Transform activeWave = createdItemsParent.Find(activeWaveName);
        if (activeWave!=null) {
            activeWave.gameObject.SetActive(true);
        }
    }

    #region Protected Interface
    protected override bool SelectUI { get { return base.SelectUI && ScrollGroupHelper.currentSpawn != null; } }

    protected override void ShowComplete() {
        base.ShowComplete();
        editorInstance.gridController.currentEditorMenu = EditorMenu.SPAWN_MENU;
    }

    protected override void SelectUIItem() {
        PLESpawn currentSpawn = ScrollGroupHelper.currentSpawn;

        if(currentSpawn) {
            selectedItem = currentSpawn.registeredSpawner;
            TryDestroyPreview();
            previewItem = Instantiate(selectedItem);
        }
    }
    protected override void PostPlaceItem(GameObject newItem) {
        string waveName = GetWaveName(WaveSystem.currentWave);
        Transform waveParent = createdItemsParent.Find(waveName);
        if (waveParent==null) {
            waveParent = new GameObject(waveName).transform;
            waveParent.SetParent(createdItemsParent);
        }
        newItem.transform.SetParent(waveParent);
    }
    #endregion
    private string GetWaveName(int i) { return Strings.Editor.Wave + i; }
}
