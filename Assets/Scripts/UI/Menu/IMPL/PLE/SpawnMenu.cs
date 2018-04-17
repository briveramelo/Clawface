//Garin + Brandon
using UnityEngine.EventSystems;
using UnityEngine;
using PLE;
using UnityEngine.UI;
using System;
using System.Linq;
using ModMan;
using System.Collections.Generic;
using UnityEngine.Events;

public class SpawnMenu : PlacementMenu {

    private const int minSpawns = 1;
    public SpawnMenu() : base(Strings.MenuStrings.LevelEditor.SPAWNS_PLE_MENU) { }

    private PLESpawn SelectedSpawn { get { return selectedPLEItem as PLESpawn; } }
    private static LevelData WorkingLevelData { get { return DataPersister.ActiveDataSave.workingLevelData; } }
    private PLESpawnManager SpawnManager { get { return PLESpawnManager.Instance; } }
    private int NumberSpawnsInCurrentWave { get { return WorkingLevelData.NumSpawns(SelectedSpawn.spawnType, SpawnManager.CurrentWaveIndex); } }
    
    
    private int MaxSpawnsAllowedInWave { get { return SpawnManager.GetMaxSpawnsAllowedInCurrentWave(SelectedSpawn); } }
    private int MaxMinSpawnsAllowedInCurrentWave { get { return SpawnManager.GetMaxMinSpawnsAllowedCurrentInWave(SelectedSpawn); } }
    private int GetMaxMinSpawnsAllowedInCurrentWave(PLESpawn spawn) { return SpawnManager.GetMaxMinSpawnsAllowedCurrentInWave(spawn); }
    private int GetRequiredKillCountInCurrentWave(PLESpawn spawn) { return SpawnManager.GetRequiredKillCountInCurrentWave(spawn); }
    private int RemainingSpawnsInCurrentWave(PLESpawn spawn) { return SpawnManager.RemainingSpawnsAllowedInCurrentWave(spawn); }

    private List<PLESpawn> CurrentWavePLESpawns { get { return WorkingLevelData.GetPLESpawnsFromWave(PLESpawnManager.Instance.CurrentWaveIndex); } }
    private List<PLESpawn> GetWavePLESpawns(int waveIndex) { return WorkingLevelData.GetPLESpawnsFromWave(waveIndex); }

    #region Serialized Unity Fields

    [SerializeField] protected Selectable decreaseSpawnCountButton, increaseSpawnCountButton, decreaseRequiredKillCountButton, increaseRequiredKillCountButton;
    [SerializeField] private InputField spawnCountAmountField, requiredKillCountAmountField;
    [SerializeField] private Text amountAvailable;
    [SerializeField] private Text nameText;
    [SerializeField] private GameObject previewImageGameObject;
    #endregion

    #region Public Fields

    static public GameObject playerSpawnInstance = null;

    #endregion

    #region Event Subscriptions
    protected override LifeCycle SubscriptionLifecycle { get { return LifeCycle.StartDestroy; } }
    protected override Dictionary<string, UnityAction<object[]>> EventSubscriptions {
        get {
            return new Dictionary<string, UnityAction<object[]>>() {
                { Strings.Events.PLE_TEST_END, TryEnableKeira },
                { Strings.Events.PLE_CALL_WAVE, OnWaveChange},
                { Strings.Events.PLE_ADD_WAVE, CheckToSetRequiredKillsTo0},
                { Strings.Events.PLE_DELETE_CURRENTWAVE, CheckToSetRequiredKillsTo0},
            };
        }
    }
    #endregion

    #region Unity Lifecycle    
    #endregion

    private void OnWaveChange(params object[] parameters) {
        DeselectOnWaveChange();
        CheckToSetRequiredKillsTo0();
    }

    private void CheckToSetRequiredKillsTo0(params object[] parameters) {
        int waveIndex = PLESpawnManager.Instance.MaxWaveIndex;
        if (!PLESpawnManager.Instance.InfiniteWavesEnabled) {
            GetWavePLESpawns(waveIndex).ForEach(spawn => {
                spawn.MinSpawns = 0;
            });
        }
    }

    private void DeselectOnWaveChange() {
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
    public void IncrementRequiredKillCount() {
        int newRequiredKillCount = Mathf.Clamp(GetRequiredKillCountInCurrentWave(SelectedSpawn) + 1, 0, MaxMinSpawnsAllowedInCurrentWave);
        ChangeSpawnAmountsInternally(SelectedSpawn.totalSpawnAmount, newRequiredKillCount);
    }
    public void DecrementRequiredKillCount() {
        int newRequiredKillCount = Mathf.Clamp(GetRequiredKillCountInCurrentWave(SelectedSpawn) - 1, 0, MaxMinSpawnsAllowedInCurrentWave);
        ChangeSpawnAmountsInternally(SelectedSpawn.totalSpawnAmount, newRequiredKillCount);
    }

    public void IncrementSpawns() {
        int newSpawnAmount = Mathf.Clamp(SelectedSpawn.totalSpawnAmount + 1, minSpawns, MaxSpawnsAllowedInWave);
        ChangeSpawnAmountsInternally(newSpawnAmount, GetRequiredKillCountInCurrentWave(SelectedSpawn));
    }
    public void DecrementSpawns() {
        int newSpawnAmount = Mathf.Clamp(SelectedSpawn.totalSpawnAmount - 1, minSpawns, MaxSpawnsAllowedInWave);
        ChangeSpawnAmountsInternally(newSpawnAmount, GetRequiredKillCountInCurrentWave(SelectedSpawn));

        int numSpawnsInWave = NumberSpawnsInCurrentWave;
        if (SelectedSpawn.MinSpawns > numSpawnsInWave) {
            ChangeSpawnAmountsInternally(SelectedSpawn.totalSpawnAmount, GetRequiredKillCountInCurrentWave(SelectedSpawn), false);
        }
    }
    public void SetSpawnAmountsOnSelectedSpawn(bool playSound) {
        int spawnCount = 0;
        int requiredKillCount = 0;
        if (selectedPLEItem) {
            if (playSound) {
                SFXManager.Instance.Play(SFXType.UI_Click);
            }
            if (System.Int32.TryParse(spawnCountAmountField.text, out spawnCount)) {
                int maxAllowed = MaxSpawnsAllowedInWave;
                int numberOfOtherSpawnsOfThisType = NumberSpawnsInCurrentWave - SelectedSpawn.totalSpawnAmount;
                int maxAllowedForThisSpawn = maxAllowed - numberOfOtherSpawnsOfThisType;
                spawnCount = Mathf.Clamp(spawnCount, minSpawns, maxAllowedForThisSpawn);
                SelectedSpawn.totalSpawnAmount = spawnCount;
            }
            if (System.Int32.TryParse(requiredKillCountAmountField.text, out requiredKillCount)) {
                requiredKillCount = Mathf.Clamp(requiredKillCount, 0, MaxMinSpawnsAllowedInCurrentWave);
                SelectedSpawn.MinSpawns = MaxMinSpawnsAllowedInCurrentWave - requiredKillCount;
            }
            UpdateAmountFields(SelectedSpawn.totalSpawnAmount, GetRequiredKillCountInCurrentWave(SelectedSpawn));
            mainPLEMenu.SetMenuButtonInteractabilityByState();
        }
        UpdateAvailableField();
    }

    public override void SetMenuButtonInteractabilityByState() {
        levelEditor.levelDataManager.SyncWorkingSpawnData();
        bool isItemSelectedAndNotKeira = selectedPLEItem != null && SelectedSpawn.spawnType != SpawnType.Keira;
        allSelectables.ForEach(selectable => { selectable.interactable = isItemSelectedAndNotKeira; });

        decreaseSpawnCountButton.interactable = isItemSelectedAndNotKeira && SelectedSpawn.totalSpawnAmount > minSpawns;
        increaseSpawnCountButton.interactable = isItemSelectedAndNotKeira && NumberSpawnsInCurrentWave < MaxSpawnsAllowedInWave;
        spawnCountAmountField.interactable = decreaseSpawnCountButton.interactable || increaseSpawnCountButton.interactable;

        bool isLastWaveAndFinite = PLESpawnManager.Instance.CurrentWaveIndex == PLESpawnManager.Instance.MaxWaveIndex && !PLESpawnManager.Instance.InfiniteWavesEnabled;
        decreaseRequiredKillCountButton.interactable = isItemSelectedAndNotKeira && GetRequiredKillCountInCurrentWave(SelectedSpawn) > 0 && !(isLastWaveAndFinite);
        increaseRequiredKillCountButton.interactable = isItemSelectedAndNotKeira && GetRequiredKillCountInCurrentWave(SelectedSpawn) < GetMaxMinSpawnsAllowedInCurrentWave(SelectedSpawn);
        requiredKillCountAmountField.interactable = decreaseRequiredKillCountButton.interactable || increaseRequiredKillCountButton.interactable;

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
            existingMatchingType.MinSpawns = Mathf.Clamp(existingMatchingType.MinSpawns, 0, GetMaxMinSpawnsAllowedInCurrentWave(existingMatchingType));
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
            UpdateMinSpawnAmounts(newSpawn);
            int requiredAmount = GetRequiredKillCountInCurrentWave(newSpawn);
            ChangeSpawnAmountsInternally(spawnAmount, requiredAmount, false);

            int remainingSpawns = RemainingSpawnsInCurrentWave(newSpawn);
            UpdateAvailableField(remainingSpawns);
            SelectGameItem(newSpawn);

            //if (!SpawnManager.SpawnsUnderMaximum(newSpawn)) {
            //    DeselectItem();
            //    DeselectUIItem();//destroys preview gameobject too
            //}

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
        int requiredAmount = GetRequiredKillCountInCurrentWave(SelectedSpawn);
        UpdateAllFields(spawnAmount, requiredAmount, SelectedSpawn.DisplayName.ToUpper(), SelectedSpawn.iconPreview);
        ChangeSpawnAmountsInternally(spawnAmount, requiredAmount, false);
    }

    protected override void DeselectItem() {
        base.DeselectItem();
        ForceMenuButtonInteractability(false);
        UpdateAllFields(0, 0, "-", null, true);        
    }
    protected override void HideStarted() {
        CheckToSetRequiredKillsTo0();
        base.HideStarted();
    }




    protected override void PostSelectUIItemMenuSpecific(GameObject newItem) {
        base.PostSelectUIItemMenuSpecific(newItem);
        PLESpawn spawn = newItem.GetComponent<PLESpawn>();
        UpdateAllFields(spawn.totalSpawnAmount, spawn.MinSpawns, spawn.DisplayName.ToUpper(), spawn.iconPreview);
        int remainingSpawns = RemainingSpawnsInCurrentWave(spawn);
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
    private void ChangeSpawnAmountsInternally(int newSpawnAmount, int newRequiredKillCount, bool playSound = true) {
        UpdateAmountFields(newSpawnAmount, newRequiredKillCount);
        SetSpawnAmountsOnSelectedSpawn(playSound);
        mainPLEMenu.SetMenuButtonInteractabilityByState();
    }

    private void UpdateAllFields(int spawnAmount, int requiredKillCount, string newName, Sprite iconPreview, bool isAmountEmpty = false) {
        UpdateAmountFields(spawnAmount, requiredKillCount, isAmountEmpty);
        nameText.text = newName;

        previewImageGameObject.SetActive(iconPreview != null);
        selectedPreviewImage.sprite = iconPreview;
    }

    private void UpdateAmountFields(int spawnAmount, int requiredKillCount, bool makeEmpty = false) {
        if (makeEmpty) {
            spawnCountAmountField.text = "";
            requiredKillCountAmountField.text = "";
            amountAvailable.text = "-";
        }
        else {
            string newAmount = Convert.ToString(spawnAmount);
            string newRequiredKillCount = Convert.ToString(requiredKillCount);
            spawnCountAmountField.text = newAmount;
            requiredKillCountAmountField.text = newRequiredKillCount;
        }
    }
    private void UpdateAvailableField(int available) {
        string availableAmount = available.ToString();
        amountAvailable.text = availableAmount;
    }
    private void UpdateAvailableField() {
        if (SelectedSpawn != null) {
            string remainingAmount = Convert.ToString(RemainingSpawnsInCurrentWave(SelectedSpawn));
            amountAvailable.text = remainingAmount;
        }
    }

    #endregion

}

