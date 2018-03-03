//Garin + Brandon
using UnityEngine.EventSystems;
using UnityEngine;
using PlayerLevelEditor;
using UnityEngine.UI;
using System;

public class SpawnMenu : PlacementMenu {

    private PLESpawn selectedSpawn;

    #region Public Interface
    public SpawnMenu() : base(Strings.MenuStrings.LevelEditor.ADD_SPAWNS_PLE) { }
    public void SetAmtOnSelectedSpawn()
    {
        if (selectedSpawn)
        {
            int finalAmt = 0;

            if (System.Int32.TryParse(amountField.text, out finalAmt))
            {
                selectedSpawn.totalSpawnAmount = finalAmt;
            }

        }
    }

    #endregion

    #region Serialized Unity Fields

    [SerializeField] private InputField amountField;

    #endregion

    #region Public Fields

    static public GameObject playerSpawnInstance = null;

    #endregion

    #region Unity Lifecycle

    private void Awake() {
        if(EventSystem.Instance){
            EventSystem.Instance.RegisterEvent(Strings.Events.PLE_CHANGEWAVE, OnWaveChange);
        }
        
    }
    private void OnDestroy() {
        if (EventSystem.Instance) {
            EventSystem.Instance.UnRegisterEvent(Strings.Events.PLE_CHANGEWAVE, OnWaveChange);
        }
    }

    #endregion

    #region Private Interface

    private void OnWaveChange(params object[] parameters) {

        string activeWaveName = GetWaveName(WaveSystem.currentWave);

        for (int i = 0; i < createdItemsParent.childCount; i++)
        {
            //Accounts for not disabling the player spawn object between switching of waves.
            GameObject currentGO = createdItemsParent.GetChild(i).gameObject;

            if (!currentGO.CompareTag(Strings.Editor.PLAYER_SPAWN_TAG))
            {
                currentGO.SetActive(false);
            }
        }
        Transform activeWave = createdItemsParent.Find(activeWaveName);
        if (activeWave!=null) {
            activeWave.gameObject.SetActive(true);
        }
    }
    private void UpdateAmtField(int i_amt)
    {
        string toSet = Convert.ToString(i_amt);
        amountField.text = toSet;
    }

    private string GetWaveName(int i) { return Strings.Editor.Wave + i; }
    private Transform TryCreateWaveParent(int i)
    {
        string waveName = GetWaveName(i);
        Transform waveParent = createdItemsParent.Find(waveName);
        if (waveParent == null)
        {
            waveParent = new GameObject(waveName).transform;
            waveParent.SetParent(createdItemsParent);
        }
        return waveParent;
    }

    #endregion

    #region Protected Interface
    protected override bool SelectUI { get { return base.SelectUI && ScrollGroupHelper.currentUIItem != null; } }
    protected override bool SelectItem { get { return base.SelectUI && MouseHelper.currentSpawn != null; } }
    protected override void DeleteHoveredItem() {
        base.DeleteHoveredItem();
        levelEditor.CheckToSetMenuInteractability();
    }
    protected override void ShowComplete() {
        base.ShowComplete();
    }

    protected override void DeselectAll() {
        base.DeselectAll();
        selectedSpawn = null;
    }
    protected override void SelectUIItem() {
        base.SelectUIItem();
    }
    protected override void PostPlaceItem(GameObject newItem) {
        int currentWave = WaveSystem.currentWave;
        Transform waveParent = TryCreateWaveParent(currentWave);
        for (int i = currentWave; i >= 0; i--) {
            TryCreateWaveParent(i);
        }
        newItem.transform.SetParent(waveParent);
        
        MouseHelper.currentBlockUnit.AddSpawn(newItem);

        PLESpawn spawn = newItem.GetComponent<PLESpawn>();
        if(spawn)
        {
            //TODO: What happens if the registered wave is 'deleted'
            spawn.registeredWave = currentWave;
        }

        if(newItem.CompareTag(Strings.Editor.PLAYER_SPAWN_TAG))
        {
            if(playerSpawnInstance != null)
            {
                DestroyImmediate(playerSpawnInstance);
            }

            playerSpawnInstance = newItem;
            playerSpawnInstance.transform.SetParent(TryCreateWaveParent(0).parent);
        }
        levelEditor.CheckToSetMenuInteractability();
        UpdateAmtField(spawn.totalSpawnAmount);
    }

    protected override void SelectGameItem() {
        base.SelectGameItem();
        MouseHelper.currentSpawn.Select();
        selectedSpawn = MouseHelper.currentSpawn;
        UpdateAmtField(selectedSpawn.totalSpawnAmount);
    }
    protected override void DeselectItem() {
        if (selectedSpawn!=null) {
            selectedSpawn.Deselect();
            selectedSpawn = null;
        }
    }


    #endregion
}
