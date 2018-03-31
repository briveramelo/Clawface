//Garin + Brandon
using UnityEngine.EventSystems;
using UnityEngine;
using PlayerLevelEditor;
using UnityEngine.UI;
using System;
using System.Linq;
using ModMan;
using System.Collections.Generic;

public class SpawnMenu : PlacementMenu {

    private const int minSpawns = 1;
    public SpawnMenu() : base(Strings.MenuStrings.LevelEditor.SPAWNS_PLE_MENU) { }

    private PLESpawn SelectedSpawn { get { return selectedPLEItem as PLESpawn; } }
    private static LevelData WorkingLevelData { get { return DataPersister.ActiveDataSave.workingLevelData; } }
    private PLESpawnManager SpawnManager { get { return PLESpawnManager.Instance; } }
    private int NumberSpawnsInWave { get { return WorkingLevelData.NumSpawns(SelectedSpawn.spawnType, PLESpawnManager.Instance.CurrentWaveIndex); } }
    
    
    private int MaxSpawnsAllowedInWave { get { return SpawnManager.GetMaxSpawnsAllowedInWave(SelectedSpawn); } }
    private int MaxMinSpawnsAllowedInWave { get { return Mathf.Min(SelectedSpawn.MaxPerWave - SpawnManager.GetNumberSpawnsInNextWave(SelectedSpawn), NumberSpawnsInWave); } }
    private int GetMaxMinSpawnsAllowedInWave(PLESpawn spawn) {
        return 
            Mathf.Min(spawn.MaxPerWave - SpawnManager.GetNumberSpawnsInNextWave(spawn),
            WorkingLevelData.NumSpawns(spawn.spawnType, PLESpawnManager.Instance.CurrentWaveIndex));
    }
    private int RemainingSpawnsInWave(PLESpawn spawn) { return SpawnManager.GetMaxSpawnsAllowedInWave(spawn) - SpawnManager.NumberSpawnsInCurrentWave(spawn.spawnType); }
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

    #region Event Subscriptions
    protected override LifeCycle SubscriptionLifecycle { get { return LifeCycle.StartDestroy; } }
    protected override Dictionary<string, FunctionPrototype> EventSubscriptions {
        get {
            return new Dictionary<string, FunctionPrototype>() {
                { Strings.Events.PLE_TEST_END, TryEnableKeira },
                { Strings.Events.PLE_CALL_WAVE, DeselectOnWaveChange},
            };
        }
    }
    #endregion

    #region Unity Lifecycle    
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
        int newMinAmount = Mathf.Clamp(SelectedSpawn.MinSpawns + 1, 0, MaxMinSpawnsAllowedInWave);
        ChangeSpawnAmountsInternally(SelectedSpawn.totalSpawnAmount, newMinAmount);
    }
    public void DecrementMinSpawns() {
        int newMinAmount = Mathf.Clamp(SelectedSpawn.MinSpawns - 1, 0, MaxMinSpawnsAllowedInWave);
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
        if (SelectedSpawn.MinSpawns > numSpawnsInWave) {
            ChangeSpawnAmountsInternally(SelectedSpawn.totalSpawnAmount, numSpawnsInWave, false);
        }
    }
    public void SetSpawnAmountsOnSelectedSpawn(bool playSound) {
        int spawnCount = 0;
        int minSpawnCount = 0;
        if (selectedPLEItem) {
            if (playSound) {
                SFXManager.Instance.Play(SFXType.UI_Click);
            }
            if (System.Int32.TryParse(spawnCountAmountField.text, out spawnCount)) {
                spawnCount = Mathf.Clamp(spawnCount, minSpawns, MaxSpawnsAllowedInWave);
                SelectedSpawn.totalSpawnAmount = spawnCount;
            }
            if (System.Int32.TryParse(minSpawnCountAmountField.text, out minSpawnCount)) {
                minSpawnCount = Mathf.Clamp(minSpawnCount, 0, MaxMinSpawnsAllowedInWave);
                SelectedSpawn.MinSpawns = minSpawnCount;
            }
            UpdateAmountFields(SelectedSpawn.totalSpawnAmount, SelectedSpawn.MinSpawns);
            mainPLEMenu.SetMenuButtonInteractabilityByState();
        }
        UpdateAvailableField();
    }

    public override void SetMenuButtonInteractabilityByState() {
        levelEditor.levelDataManager.SyncWorkingSpawnData();
        bool isItemSelectedAndNotKeira = selectedPLEItem != null && SelectedSpawn.spawnType != SpawnType.Keira;
        allSelectables.ForEach(selectable => { selectable.interactable = isItemSelectedAndNotKeira; });

        decreaseSpawnCountButton.interactable = isItemSelectedAndNotKeira && SelectedSpawn.totalSpawnAmount > minSpawns;
        increaseSpawnCountButton.interactable = isItemSelectedAndNotKeira && NumberSpawnsInWave < MaxSpawnsAllowedInWave;

        decreaseMinSpawnCountButton.interactable = isItemSelectedAndNotKeira && SelectedSpawn.MinSpawns > 0;
        increaseMinSpawnCountButton.interactable = isItemSelectedAndNotKeira && SelectedSpawn.MinSpawns < MaxMinSpawnsAllowedInWave;


        (scrollGroup as SpawnScrollGroup).HandleSpawnUIInteractability();
    }
    #endregion

    #region Protected Interface
    protected override bool SelectUI { get { return base.SelectUI && ScrollGroupHelper.currentUIItem != null && ScrollGroupHelper.currentUIItem.isInteractable; } }
    protected override bool SelectItem { get { return base.SelectItem && MouseHelper.currentSpawn != null; } }
    protected override bool CanDeleteHoveredItem { get { return base.CanDeleteHoveredItem && MouseHelper.currentSpawn; } }
    protected override bool Place { get { return base.Place && !MouseHelper.currentBlockUnit.HasActiveSpawn && !(!MouseHelper.currentBlockUnit.IsFlatAtWave(0) && selectedItemPrefab.GetComponent<PLESpawn>().spawnType==SpawnType.Keira); } }
    protected override bool ReplaceGameItem { get { return base.ReplaceGameItem && !(!MouseHelper.currentBlockUnit.IsFlatAtWave(0) && SelectedSpawn.spawnType == SpawnType.Keira); } }
    protected override bool UpdateGameItem { get { return base.UpdateGameItem && !(!MouseHelper.currentBlockUnit.IsFlatAtWave(0) && SelectedSpawn.spawnType == SpawnType.Keira); } }
    protected override bool UpdatePreview { get { return base.UpdatePreview && !MouseHelper.currentBlockUnit.HasActiveSpawn; } }


    protected override void DeleteHoveredItem() {
        PLESpawn currentSpawn = MouseHelper.currentSpawn;
        PLESpawn existingMatchingType = null;
        List<PLESpawn> allVisibleSpawns = FindObjectsOfType<PLESpawn>().ToList();
        if (allVisibleSpawns.Count > 1) {
            existingMatchingType = allVisibleSpawns.Find(spawn => spawn.spawnType == currentSpawn.spawnType && spawn != currentSpawn);
        }
        base.DeleteHoveredItem();
        mainPLEMenu.SetMenuButtonInteractabilityByState();
        if (existingMatchingType!=null) {
            existingMatchingType.MinSpawns = Mathf.Clamp(existingMatchingType.MinSpawns, 0, GetMaxMinSpawnsAllowedInWave(existingMatchingType));
        }
    }

    protected override void ShowStarted() {
        base.ShowStarted();
        mainPLEMenu.SetMenuButtonInteractabilityByState();
        TrySelectFirstAvailable();
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
        PLESpawn newSpawn = newItem.GetComponent<PLESpawn>();
        if (newSpawn) {
            int spawnAmount = newSpawn.totalSpawnAmount;
            int minSpawnAmount = UpdateMinSpawnAmounts(newSpawn);
            ChangeSpawnAmountsInternally(spawnAmount, minSpawnAmount, false);            

            int remainingSpawns = RemainingSpawnsInWave(newSpawn);
            UpdateAvailableField(remainingSpawns);
            SelectGameItem(newSpawn);

            if (!SpawnManager.SpawnsUnderMaximum(newSpawn)) {
                DeselectItem();
                DeselectUIItem();//destroys preview gameobject too
            }

            if (newSpawn.spawnType==SpawnType.Keira) {
                PLEUIItem firstAvailable = scrollGroup.TryGetFirstAvailableUIItem();
                if (firstAvailable) {
                    selectedPreviewImage.sprite = firstAvailable.imagePreview.sprite;
                    scrollGroup.SelectUIItem(firstAvailable.ItemIndex, false);
                }
            }
        }
        mainPLEMenu.SetMenuButtonInteractabilityByState();
    }

    protected override void SelectGameItem(PLEItem selectedItem, bool isPickingUp=false) {
        base.SelectGameItem(selectedItem, isPickingUp);        
        int spawnAmount = SelectedSpawn.totalSpawnAmount;
        int minAmount = SelectedSpawn.MinSpawns;
        UpdateAllFields(spawnAmount, minAmount, SelectedSpawn.DisplayName.ToUpper(), SelectedSpawn.iconPreview);
        ChangeSpawnAmountsInternally(spawnAmount, minAmount, false);
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

    #region Private Interface
    private int UpdateMinSpawnAmounts(PLESpawn newSpawn) {
        List<PLESpawn> allVisibleSpawns = FindObjectsOfType<PLESpawn>().ToList();
        int minSpawnAmount = 0;
        if (allVisibleSpawns.Count > 1) {
            PLESpawn existingMatchingType = allVisibleSpawns.Find(spawn => spawn.spawnType == newSpawn.spawnType && spawn != newSpawn && !spawn.gameObject.name.Contains(Strings.PREVIEW));
            if (existingMatchingType != null) {
                minSpawnAmount = existingMatchingType.MinSpawns;
            }
        }
        newSpawn.MinSpawns = minSpawnAmount;
        return minSpawnAmount;
    }
    private void ChangeSpawnAmountsInternally(int newSpawnAmount, int newMinSpawnAmount, bool playSound = true) {
        UpdateAmountFields(newSpawnAmount, newMinSpawnAmount);
        SetSpawnAmountsOnSelectedSpawn(playSound);
        mainPLEMenu.SetMenuButtonInteractabilityByState();
    }

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

