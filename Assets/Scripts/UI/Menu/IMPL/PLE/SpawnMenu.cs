//Garin + Brandon
using UnityEngine.EventSystems;
using UnityEngine;
using PlayerLevelEditor;
using UnityEngine.UI;
using System;
using ModMan;
using System.Collections.Generic;

public class SpawnMenu : PlacementMenu {

    private const int minSpawns = 1;
    public SpawnMenu() : base(Strings.MenuStrings.LevelEditor.SPAWNS_PLE_MENU) { }    

    private PLESpawn SelectedSpawn { get { return selectedPLEItem as PLESpawn; } }
    private LevelData WorkingLevelData { get { return DataPersister.ActiveDataSave.workingLevelData; } }
    private int NumberSpawnsInWave { get { return WorkingLevelData.NumSpawns(SelectedSpawn.spawnType, PLESpawnManager.Instance.CurrentWaveIndex); } }
    private List<PLESpawn> CurrentWavePLESpawns { get { return WorkingLevelData.GetPLESpawnsFromWave(PLESpawnManager.Instance.CurrentWaveIndex); } }

    #region Serialized Unity Fields

    [SerializeField] protected Selectable decreaseSpawnCountButton, increaseSpawnCountButton;
    [SerializeField] private InputField amountField;
    [SerializeField] private Text amountAvailable;
    [SerializeField] private Text nameText;
    #endregion

    #region Public Fields

    static public GameObject playerSpawnInstance = null;

    #endregion

    #region Unity Lifecycle
    protected override void Start() {
        base.Start();
        EventSystem.Instance.RegisterEvent(Strings.Events.PLE_TEST_END, TryEnableKeira);
    }

    private void OnDestroy() {
        if (EventSystem.Instance) {
            EventSystem.Instance.UnRegisterEvent(Strings.Events.PLE_TEST_END, TryEnableKeira);
        }
    }
    #endregion
    void TryEnableKeira(params object[] parameters) {
        if (playerSpawnInstance != null) {
            playerSpawnInstance.SetActive(true);
        }
    }

    #region Public Interface    
    public void Increment() {
        int newAmount = Mathf.Clamp(SelectedSpawn.totalSpawnAmount + 1, minSpawns, SelectedSpawn.MaxPerWave);
        ChangeSpawnAmountInternally(newAmount);
    }
    public void Decrement() {
        int newAmount = Mathf.Clamp(SelectedSpawn.totalSpawnAmount - 1, minSpawns, SelectedSpawn.MaxPerWave);
        ChangeSpawnAmountInternally(newAmount);
    }
    public void SetAmountOnSelectedSpawn() {
        int spawnCount = 0;
        if (selectedPLEItem) {
            if (System.Int32.TryParse(amountField.text, out spawnCount)) {
                spawnCount = Mathf.Clamp(spawnCount, minSpawns, SelectedSpawn.MaxPerWave);
                SelectedSpawn.totalSpawnAmount = spawnCount;
                mainPLEMenu.SetMenuButtonInteractabilityByState();
            }
        }
    }

    public override void SetMenuButtonInteractabilityByState() {
        levelEditor.levelDataManager.SyncWorkingSpawnData();
        bool isItemSelectedAndNotKeira = selectedPLEItem != null && SelectedSpawn.spawnType != SpawnType.Keira;
        allSelectables.ForEach(selectable => { selectable.interactable = isItemSelectedAndNotKeira; });

        decreaseSpawnCountButton.interactable = isItemSelectedAndNotKeira && SelectedSpawn.totalSpawnAmount > minSpawns;
        increaseSpawnCountButton.interactable = isItemSelectedAndNotKeira && NumberSpawnsInWave < SelectedSpawn.MaxPerWave;
        (scrollGroup as SpawnScrollGroup).HandleSpawnUIInteractability(PLESpawnManager.Instance.CurrentWaveIndex);
    }
    #endregion

    #region Protected Interface
    protected override bool SelectUI { get { return base.SelectUI && ScrollGroupHelper.currentUIItem != null && ScrollGroupHelper.currentUIItem.isInteractable; } }
    protected override bool SelectItem { get { return base.SelectUI && MouseHelper.currentSpawn != null; } }
    protected override bool CanDeletedHoveredItem { get { return base.CanDeletedHoveredItem && MouseHelper.currentSpawn; } }
    protected override bool Place { get { return base.Place && !MouseHelper.currentBlockUnit.HasActiveSpawn; } }
    protected override bool UpdatePreview { get { return base.UpdatePreview && !MouseHelper.currentBlockUnit.HasActiveSpawn; } }


    protected override void DeleteHoveredItem() {
        base.DeleteHoveredItem();
        mainPLEMenu.SetMenuButtonInteractabilityByState();
    }

    protected override void ShowStarted() {
        base.ShowStarted();
        mainPLEMenu.SetMenuButtonInteractabilityByState();
        TrySelectFirstAvailable();
    }
    public override void TrySelectFirstAvailable() {
        base.TrySelectFirstAvailable();
    }


    protected override void PostPlaceItem(GameObject newItem) {
        int currentWaveIndex = PLESpawnManager.Instance.CurrentWaveIndex;
        int maxWaveIndex = PLESpawnManager.Instance.MaxWaveIndex;
        Transform waveParent = levelEditor.TryCreateWaveParent(currentWaveIndex);
        for (int i = maxWaveIndex; i >= 0; i--) {
            levelEditor.TryCreateWaveParent(i);
        }
        newItem.transform.SetParent(waveParent);        
        MouseHelper.currentBlockUnit.AddSpawn(newItem);

        if(newItem.CompareTag(Strings.Editor.PLAYER_SPAWN_TAG)) {
            if(playerSpawnInstance != null) {
                DestroyImmediate(playerSpawnInstance);
            }
            playerSpawnInstance = newItem;
            playerSpawnInstance.transform.SetParent(levelEditor.TryCreateWaveParent(0).parent);
        }
        PLESpawn spawn = newItem.GetComponent<PLESpawn>();
        if (spawn) {
            int spawnAmount = spawn.totalSpawnAmount;
            ChangeSpawnAmountInternally(spawnAmount);
            UpdateAmountField(spawnAmount);
            int remainingSpawns = spawn.MaxPerWave - NumberSpawnsInCurrentWave(spawn.spawnType);
            UpdateAvailableField(remainingSpawns);
            if (spawn.spawnType != SpawnType.Keira) {
                SelectGameItem(spawn);
            }            


            if (NumberSpawnsInCurrentWave(spawn.spawnType) >= spawn.MaxPerWave ) {
                TryDestroyPreview();
                DeselectItem();
                DeselectUIItem();
            }

            if (spawn.spawnType==SpawnType.Keira) {
                PLEUIItem firstAvailable = scrollGroup.TryGetFirstAvailableUIItem();
                if (firstAvailable) {
                    selectedPreviewImage.sprite = firstAvailable.imagePreview.sprite;
                    scrollGroup.SelectUIItem(firstAvailable.ItemIndex);
                }
            }
        }
        mainPLEMenu.SetMenuButtonInteractabilityByState();
    }

    protected override void SelectGameItem(PLEItem selectedItem, bool isPickingUp=false) {
        base.SelectGameItem(selectedItem, isPickingUp);        
        selectedPLEItem = selectedItem;
        selectedPLEItem.Select(highlightColor);
        int spawnAmount = SelectedSpawn.totalSpawnAmount;
        UpdateFields(spawnAmount, SelectedSpawn.DisplayName.ToUpper(), SelectedSpawn.iconPreview);
        ChangeSpawnAmountInternally(spawnAmount);
    }
    protected override void DeselectItem() {
        base.DeselectItem();
        ForceMenuButtonInteractability(false);
        UpdateFields(0, "-", null, true);        
    }


    

    protected override void PostSelectUIItemMenuSpecific(GameObject newItem) {
        base.PostSelectUIItemMenuSpecific(newItem);
        PLESpawn spawn = newItem.GetComponent<PLESpawn>();
        UpdateFields(spawn.totalSpawnAmount, spawn.DisplayName.ToUpper(), spawn.iconPreview);
        int remainingSpawns = spawn.MaxPerWave - NumberSpawnsInCurrentWave(spawn.spawnType);
        UpdateAvailableField(remainingSpawns);
    }
    #endregion

    private void ChangeSpawnAmountInternally(int newAmount) {
        UpdateAmountField(newAmount);
        SetAmountOnSelectedSpawn();
        levelEditor.levelDataManager.SyncWorkingSpawnData();
        UpdateAvailableField();
        mainPLEMenu.SetMenuButtonInteractabilityByState();
    }


    #region Private Interface
    private void UpdateFields(int spawnAmount, string newName, Sprite iconPreview, bool isAmountEmpty = false) {
        UpdateAmountField(spawnAmount, isAmountEmpty);
        nameText.text = newName;
        selectedPreviewImage.gameObject.SetActive(iconPreview != null);
        selectedPreviewImage.sprite = iconPreview;
    }



    private void UpdateAmountField(int i_amt, bool makeEmpty = false) {
        if (makeEmpty) {
            amountField.text = "";
            amountAvailable.text = "-";
        }
        else {
            string newAmount = Convert.ToString(i_amt);
            amountField.text = newAmount;            
        }
    }

    private void UpdateAvailableField(int available) {
        string remainingAmount = available.ToString();
        amountAvailable.text = remainingAmount;
    }
    private void UpdateAvailableField() {
        if (SelectedSpawn != null) {
            string remainingAmount = Convert.ToString(SelectedSpawn.MaxPerWave - NumberSpawnsInWave);
            amountAvailable.text = remainingAmount;
        }
    }
    private int NumberSpawnsInCurrentWave(SpawnType type) {
        return WorkingLevelData.NumSpawns(type, PLESpawnManager.Instance.CurrentWaveIndex);
    }

    

    #endregion

}

