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
    public SpawnMenu() : base(Strings.MenuStrings.LevelEditor.ADD_SPAWNS_PLE) { }    

    private PLESpawn SelectedSpawn { get { return selectedPLEItem as PLESpawn; } }
    private LevelData ActiveLevelData { get { return DataPersister.ActiveDataSave.ActiveLevelData; } }
    private int NumberSpawnsInWave { get { return ActiveLevelData.NumSpawns(SelectedSpawn.spawnType, PLESpawnManager.Instance.CurrentWaveIndex); } }
    private List<PLESpawn> CurrentWavePLESpawns { get { return ActiveLevelData.GetPLESpawnsFromWave(PLESpawnManager.Instance.CurrentWaveIndex); } }

    #region Serialized Unity Fields

    [SerializeField] private InputField amountField;
    [SerializeField] private Text amountAvailable;
    [SerializeField] private Text nameText;

    #endregion

    #region Public Fields

    static public GameObject playerSpawnInstance = null;

    #endregion

    

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
                SetInteractabilityByState();
            }
        }
    }
    #endregion

    #region Protected Interface
    protected override bool SelectUI { get { return base.SelectUI && ScrollGroupHelper.currentUIItem != null && ScrollGroupHelper.currentUIItem.isInteractable; } }
    protected override bool SelectItem { get { return base.SelectUI && MouseHelper.currentSpawn != null; } }
    protected override bool CanDeletedHoveredItem { get { return base.CanDeletedHoveredItem && MouseHelper.currentSpawn; } }
    protected override bool Place { get { return base.Place && !MouseHelper.currentBlockUnit.HasActiveSpawn; } }
    protected override bool UpdatePreview { get { return base.UpdatePreview && !MouseHelper.currentBlockUnit.HasActiveSpawn; } }

    protected override void InitializeSelectables() {
        base.InitializeSelectables();
        selectables.Add(amountField);
    }

    protected override void DeleteHoveredItem() {
        base.DeleteHoveredItem();
        SetInteractabilityByState();
        levelEditor.SetMenuButtonInteractability();
    }

    protected override void ShowStarted() {
        base.ShowStarted();
        if (!playerSpawnInstance) {
            (scrollGroup as SpawnScrollGroup).SelectKeira();
        }
        else {
            TrySelectUIItem(scrollGroup.GetLastUIItem());
        }
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
                    TrySelectUIItem(firstAvailable);
                }
            }
        }
        SetInteractabilityByState();
        levelEditor.SetMenuButtonInteractability();        
    }    

    protected override void SelectGameItem(PLEItem selectedItem) {
        base.SelectGameItem(selectedItem);
        selectedPLEItem = selectedItem;
        selectedPLEItem.Select();
        int spawnAmount = SelectedSpawn.totalSpawnAmount;
        UpdateFields(spawnAmount, SelectedSpawn.DisplayName, false);
        ChangeSpawnAmountInternally(spawnAmount);
    }
    protected override void DeselectItem() {
        base.DeselectItem();
        ForceInteractability(false);
        UpdateFields(0, "-", true);        
    }
    
    
    protected override void SetInteractabilityByState() {
        bool isItemSelectedAndNotKeira = selectedPLEItem != null && SelectedSpawn.spawnType != SpawnType.Keira;
        selectables.ForEach(selectable => { selectable.interactable = isItemSelectedAndNotKeira; });

        leftButton.interactable = isItemSelectedAndNotKeira && SelectedSpawn.totalSpawnAmount > minSpawns;
        rightButton.interactable = isItemSelectedAndNotKeira && NumberSpawnsInWave < SelectedSpawn.MaxPerWave;
        (scrollGroup as SpawnScrollGroup).SetSpawnUIInteractability(PLESpawnManager.Instance.CurrentWaveIndex);
    }

    protected override void PostOnSelectUIItem(GameObject newItem) {
        base.PostOnSelectUIItem(newItem);
        PLESpawn spawn = newItem.GetComponent<PLESpawn>();
        UpdateFields(spawn.totalSpawnAmount, spawn.DisplayName);
        int remainingSpawns = spawn.MaxPerWave - NumberSpawnsInCurrentWave(spawn.spawnType);
        UpdateAvailableField(remainingSpawns);
    }
    #endregion

    private void ChangeSpawnAmountInternally(int newAmount) {
        UpdateAmountField(newAmount);
        SetAmountOnSelectedSpawn();
        levelEditor.levelDataManager.SaveSpawns();
        UpdateAvailableField();
        SetInteractabilityByState();
    }


    #region Private Interface
    private void UpdateFields(int spawnAmount, string newName, bool isAmountEmpty = false) {
        UpdateAmountField(spawnAmount, isAmountEmpty);
        nameText.text = newName;
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
        return ActiveLevelData.NumSpawns(type, PLESpawnManager.Instance.CurrentWaveIndex);
    }

    #endregion

}
