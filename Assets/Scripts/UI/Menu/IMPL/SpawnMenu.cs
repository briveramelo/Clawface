//Garin + Brandon
using UnityEngine.EventSystems;
using UnityEngine;
public class SpawnMenu : PlacementMenu {

    private PLESpawn selectedSpawn;

    #region Public Interface
    public SpawnMenu() : base(Strings.MenuStrings.ADD_SPAWNS_PLE) { }
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
    protected override bool SelectUI { get { return base.SelectUI && ScrollGroupHelper.currentUIItem != null; } }
    protected override bool SelectItem { get { return base.SelectUI && MouseHelper.currentSpawn != null; } }

    protected override void ShowComplete() {
        base.ShowComplete();
        editorInstance.gridController.currentEditorMenu = EditorMenu.SPAWN_MENU;
    }


    protected override void DeselectAll() {
        base.DeselectAll();
        selectedSpawn = null;
    }
    protected override void SelectUIItem() {
        PLEUIItem currentUIItem = ScrollGroupHelper.currentUIItem;

        if(currentUIItem) {
            selectedItem = currentUIItem.registeredItem;
            TryDestroyPreview();
            previewItem = Instantiate(selectedItem);
        }
    }
    protected override void PostPlaceItem(GameObject newItem) {
        int currentWave = WaveSystem.currentWave;
        Transform waveParent = TryCreateWaveParent(currentWave);
        for (int i = currentWave; i >= 0; i--) {
            TryCreateWaveParent(i);
        }
        newItem.transform.SetParent(waveParent);
    }
    Transform TryCreateWaveParent(int i) {
        string waveName = GetWaveName(i);
        Transform waveParent = createdItemsParent.Find(waveName);
        if (waveParent == null) {
            waveParent = new GameObject(waveName).transform;
            waveParent.SetParent(createdItemsParent);
        }
        return waveParent;
    }

    #endregion
    private string GetWaveName(int i) { return Strings.Editor.Wave + i; }

    protected override void SelectGameItem() {
        base.SelectGameItem();
        MouseHelper.currentSpawn.Select();
        selectedSpawn = MouseHelper.currentSpawn;
    }
    protected override void DeselectItem() {
        if (selectedSpawn!=null) {
            selectedSpawn.Deselect();
            selectedSpawn = null;
        }
    }
}
