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
    private int NumberSpawnsInCurrentWave(SpawnType type) {
        return WorkingLevelData.NumSpawns(type, PLESpawnManager.Instance.CurrentWaveIndex);
    }
    private int GetMaxSpawnsAllowedInWave(PLESpawn spawn) {
        int currentWaveIndex = PLESpawnManager.Instance.CurrentWaveIndex;
        int previousWaveMinSpawnCount = 0;
        if (currentWaveIndex - 1 >= 0) {
            previousWaveMinSpawnCount = WorkingLevelData.MinNumSpawns(spawn.spawnType, currentWaveIndex - 1);
        }
        return spawn.MaxPerWave - previousWaveMinSpawnCount;        
    }
    private int MaxSpawnsAllowedInWave { get { return GetMaxSpawnsAllowedInWave(SelectedSpawn); } }
    private int RemainingSpawnsInWave(PLESpawn spawn) { return GetMaxSpawnsAllowedInWave(spawn) - NumberSpawnsInCurrentWave(spawn.spawnType); }
    private List<PLESpawn> CurrentWavePLESpawns { get { return WorkingLevelData.GetPLESpawnsFromWave(PLESpawnManager.Instance.CurrentWaveIndex); } }
    #region Serialized Unity Fields

    [SerializeField] protected Selectable decreaseSpawnCountButton, increaseSpawnCountButton, decreaseMinSpawnCountButton, increaseMinSpawnCountButton;
    [SerializeField] private InputField spawnCountAmountField, minSpawnCountAmountField;
    [SerializeField] private Text amountAvailable;
    [SerializeField] private Text nameText;
    [SerializeField] private GameObject previewImageGameObject;
    #endregion

    #region Public Fields

    static public GameObject playerSpawnInstance = null;

    #endregion

    #region Unity Lifecycle
    protected override void Start() {
        base.Start();
        EventSystem.Instance.RegisterEvent(Strings.Events.PLE_TEST_END, TryEnableKeira);
        EventSystem.Instance.RegisterEvent(Strings.Events.PLE_CALL_WAVE, DeselectOnWaveChange);
    }

    private void OnDestroy() {
        if (EventSystem.Instance) {
            EventSystem.Instance.UnRegisterEvent(Strings.Events.PLE_TEST_END, TryEnableKeira);
            EventSystem.Instance.UnRegisterEvent(Strings.Events.PLE_CALL_WAVE, DeselectOnWaveChange);
        }
    }
    #endregion

    void DeselectOnWaveChange(params object[] parameters) {
        if (!levelEditor.IsTesting) {
            DeselectAllGameItems();
            DeselectItem();
            DeselectUIItem();
        }
    }

    void TryEnableKeira(params object[] parameters) {
        if (playerSpawnInstance != null) {
            playerSpawnInstance.SetActive(true);
        }
    }

    #region Public Interface    
    public void IncrementMinSpawns() {
        int newMinAmount = Mathf.Clamp(SelectedSpawn.MinSpawns + 1, 0, NumberSpawnsInWave-1);
        ChangeSpawnAmountsInternally(SelectedSpawn.totalSpawnAmount, newMinAmount);
    }
    public void DecrementMinSpawns() {
        int newMinAmount = Mathf.Clamp(SelectedSpawn.MinSpawns - 1, 0, NumberSpawnsInWave - 1);
        ChangeSpawnAmountsInternally(SelectedSpawn.totalSpawnAmount, newMinAmount);
    }


    public void Increment() {
        int newSpawnAmount = Mathf.Clamp(SelectedSpawn.totalSpawnAmount + 1, minSpawns, MaxSpawnsAllowedInWave);
        ChangeSpawnAmountsInternally(newSpawnAmount, SelectedSpawn.MinSpawns);
    }
    public void Decrement() {
        int newSpawnAmount = Mathf.Clamp(SelectedSpawn.totalSpawnAmount - 1, minSpawns, MaxSpawnsAllowedInWave);
        ChangeSpawnAmountsInternally(newSpawnAmount, SelectedSpawn.MinSpawns);

        int numSpawnsInWave = NumberSpawnsInWave;
        if (SelectedSpawn.MinSpawns >= numSpawnsInWave - 1) {
            ChangeSpawnAmountsInternally(SelectedSpawn.totalSpawnAmount, numSpawnsInWave - 1);
        }
    }
    public void SetSpawnAmountsOnSelectedSpawn() {
        int spawnCount = 0;
        int minSpawnCount = 0;
        if (selectedPLEItem) {
            if (System.Int32.TryParse(spawnCountAmountField.text, out spawnCount)) {
                spawnCount = Mathf.Clamp(spawnCount, minSpawns, MaxSpawnsAllowedInWave);
                SelectedSpawn.totalSpawnAmount = spawnCount;
            }
            if (System.Int32.TryParse(minSpawnCountAmountField.text, out minSpawnCount)) {
                minSpawnCount = Mathf.Clamp(minSpawnCount, 0, NumberSpawnsInWave - 1);
                SelectedSpawn.MinSpawns = minSpawnCount;
            }
            mainPLEMenu.SetMenuButtonInteractabilityByState();
        }
    }

    public override void SetMenuButtonInteractabilityByState() {
        levelEditor.levelDataManager.SyncWorkingSpawnData();
        bool isItemSelectedAndNotKeira = selectedPLEItem != null && SelectedSpawn.spawnType != SpawnType.Keira;
        allSelectables.ForEach(selectable => { selectable.interactable = isItemSelectedAndNotKeira; });

        decreaseSpawnCountButton.interactable = isItemSelectedAndNotKeira && SelectedSpawn.totalSpawnAmount > minSpawns;
        increaseSpawnCountButton.interactable = isItemSelectedAndNotKeira && NumberSpawnsInWave < MaxSpawnsAllowedInWave;

        decreaseMinSpawnCountButton.interactable = isItemSelectedAndNotKeira && SelectedSpawn.MinSpawns > 0;
        increaseMinSpawnCountButton.interactable = isItemSelectedAndNotKeira && SelectedSpawn.MinSpawns < NumberSpawnsInWave -1;


        (scrollGroup as SpawnScrollGroup).HandleSpawnUIInteractability(PLESpawnManager.Instance.CurrentWaveIndex);
    }
    #endregion

    #region Protected Interface
    protected override bool SelectUI { get { return base.SelectUI && ScrollGroupHelper.currentUIItem != null && ScrollGroupHelper.currentUIItem.isInteractable; } }
    protected override bool SelectItem { get { return base.SelectUI && MouseHelper.currentSpawn != null; } }
    protected override bool CanDeletedHoveredItem { get { return base.CanDeletedHoveredItem && MouseHelper.currentSpawn; } }
    protected override bool Place { get { return base.Place && !MouseHelper.currentBlockUnit.HasActiveSpawn && !(!MouseHelper.currentBlockUnit.IsFlatAtWave(0) && selectedItemPrefab.GetComponent<PLESpawn>().spawnType==SpawnType.Keira); } }
    protected override bool ReplaceGameItem { get { return base.ReplaceGameItem && !(!MouseHelper.currentBlockUnit.IsFlatAtWave(0) && SelectedSpawn.spawnType == SpawnType.Keira); } }
    protected override bool UpdateGameItem { get { return base.UpdateGameItem && !(!MouseHelper.currentBlockUnit.IsFlatAtWave(0) && SelectedSpawn.spawnType == SpawnType.Keira); } }
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
            int minSpawnAmount = spawn.MinSpawns;
            ChangeSpawnAmountsInternally(spawnAmount, minSpawnAmount);            

            int remainingSpawns = RemainingSpawnsInWave(spawn);
            UpdateAvailableField(remainingSpawns);
            if (spawn.spawnType != SpawnType.Keira) {
                SelectGameItem(spawn);
            }


            if (NumberSpawnsInCurrentWave(spawn.spawnType) >= GetMaxSpawnsAllowedInWave(spawn)) {
                DeselectItem();
                DeselectUIItem();//destroys preview gameobject too
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
        int minAmount = SelectedSpawn.MinSpawns;
        UpdateAllFields(spawnAmount, minAmount, SelectedSpawn.DisplayName.ToUpper(), SelectedSpawn.iconPreview);
        ChangeSpawnAmountsInternally(spawnAmount, minAmount);
    }

    protected override void DeselectItem() {
        base.DeselectItem();
        ForceMenuButtonInteractability(false);
        UpdateAllFields(0, 0, "-", null, true);        
    }


    

    protected override void PostSelectUIItemMenuSpecific(GameObject newItem) {
        base.PostSelectUIItemMenuSpecific(newItem);
        PLESpawn spawn = newItem.GetComponent<PLESpawn>();
        UpdateAllFields(spawn.totalSpawnAmount, spawn.MinSpawns, spawn.DisplayName.ToUpper(), spawn.iconPreview);
        int remainingSpawns = RemainingSpawnsInWave(spawn);
        UpdateAvailableField(remainingSpawns);
    }
    #endregion

    private void ChangeSpawnAmountsInternally(int newSpawnAmount, int newMinSpawnAmount) {
        UpdateAmountFields(newSpawnAmount, newMinSpawnAmount);
        SetSpawnAmountsOnSelectedSpawn();
        mainPLEMenu.SetMenuButtonInteractabilityByState();
        UpdateAvailableField();
    }



    #region Private Interface
    private void UpdateAllFields(int spawnAmount, int minAmount, string newName, Sprite iconPreview, bool isAmountEmpty = false) {
        UpdateAmountFields(spawnAmount, minAmount, isAmountEmpty);
        nameText.text = newName;

        previewImageGameObject.SetActive(iconPreview != null);
        selectedPreviewImage.sprite = iconPreview;
    }

    private void UpdateAmountFields(int spawnAmount, int minSpawnAmount, bool makeEmpty = false) {
        if (makeEmpty) {
            spawnCountAmountField.text = "";
            minSpawnCountAmountField.text = "";
            amountAvailable.text = "-";
        }
        else {
            string newAmount = Convert.ToString(spawnAmount);
            string newMinAmount = Convert.ToString(minSpawnAmount);
            spawnCountAmountField.text = newAmount;
            minSpawnCountAmountField.text = newMinAmount;
        }
    }
    private void UpdateAvailableField(int available) {
        string availableAmount = available.ToString();
        amountAvailable.text = availableAmount;
    }
    private void UpdateAvailableField() {
        if (SelectedSpawn != null) {
            string remainingAmount = Convert.ToString(RemainingSpawnsInWave(SelectedSpawn));
            amountAvailable.text = remainingAmount;
        }
    }

    #endregion

}

