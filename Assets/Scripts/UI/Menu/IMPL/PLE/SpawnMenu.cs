//Garin + Brandon
using UnityEngine.EventSystems;
using UnityEngine;
using PlayerLevelEditor;
using UnityEngine.UI;
using System;
using ModMan;

public class SpawnMenu : PlacementMenu {

    private const int minSpawns = 1;
    private const int maxSpawns = 99;
    public SpawnMenu() : base(Strings.MenuStrings.LevelEditor.ADD_SPAWNS_PLE) { }    

    private PLESpawn SelectedSpawn { get { return selectedPLEItem as PLESpawn; } }        

    #region Serialized Unity Fields

    [SerializeField] private InputField amountField;
    [SerializeField] private Text nameText;

    #endregion

    #region Public Fields

    static public GameObject playerSpawnInstance = null;

    #endregion

    

    #region Public Interface
    public void SetAmtOnSelectedSpawn() {
        if (selectedPLEItem) {
            int finalAmt = 0;

            if (System.Int32.TryParse(amountField.text, out finalAmt)) {
                SelectedSpawn.totalSpawnAmount = finalAmt;
            }
        }
    }
    public void Increment() {
        int newAmount = Mathf.Clamp(SelectedSpawn.totalSpawnAmount + 1, minSpawns, maxSpawns);
        ChangeSpawnAmount(newAmount);
    }
    public void Decrement() {
        int newAmount = Mathf.Clamp(SelectedSpawn.totalSpawnAmount - 1, minSpawns, maxSpawns);
        ChangeSpawnAmount(newAmount);
    }    
    #endregion

    #region Protected Interface
    protected override bool SelectUI { get { return base.SelectUI && ScrollGroupHelper.currentUIItem != null; } }
    protected override bool SelectItem { get { return base.SelectUI && MouseHelper.currentSpawn != null; } }
    protected override bool CanDeletedHoveredItem { get { return base.CanDeletedHoveredItem && MouseHelper.currentSpawn; } }
    protected override bool Place { get { return base.Place && !MouseHelper.currentBlockUnit.HasActiveSpawn; } }
    protected override bool UpdatePreview { get { return base.UpdatePreview && !MouseHelper.currentBlockUnit.HasActiveSpawn; } }


    protected override void DeleteHoveredItem() {
        base.DeleteHoveredItem();
        levelEditor.SetMenuButtonInteractability();
    }

    protected override void ShowStarted() {
        base.ShowStarted();
        SetInteractability(false);
    }
    protected override void ShowComplete() {
        base.ShowComplete();
    }

    protected override void DeselectAllGameItems() {
        base.DeselectAllGameItems();
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
        //UNUSED
        //PLESpawn spawn = newItem.GetComponent<PLESpawn>();
        //if(spawn)
        //{
        //    //TODO: What happens if the registered wave is 'deleted'
        //    spawn.registeredWave = currentWaveIndex;
        //}

        if(newItem.CompareTag(Strings.Editor.PLAYER_SPAWN_TAG)) {
            if(playerSpawnInstance != null) {
                DestroyImmediate(playerSpawnInstance);
            }
            playerSpawnInstance = newItem;
            playerSpawnInstance.transform.SetParent(levelEditor.TryCreateWaveParent(0).parent);
        }
        levelEditor.SetMenuButtonInteractability();
    }    

    protected override void SelectGameItem() {
        base.SelectGameItem();
        MouseHelper.currentSpawn.Select();
        selectedPLEItem = MouseHelper.currentSpawn;
        UpdateFields(SelectedSpawn.totalSpawnAmount, SelectedSpawn.spawnType.SpawnDisplayName());
        SetInteractability();
    }
    protected override void DeselectItem() {
        base.DeselectItem();
        SetInteractability(false);
        UpdateFields(0, "-", true);        
    }
    protected override void InitializeSelectables() {
        base.InitializeSelectables();
        selectables.Add(amountField);
    }
    
    protected override void SetInteractability() {
        bool isItemSelectedAndNotKeira = selectedPLEItem != null && SelectedSpawn.spawnType != SpawnType.Keira;
        selectables.ForEach(selectable => { selectable.interactable = isItemSelectedAndNotKeira; });

        leftButton.interactable = isItemSelectedAndNotKeira && SelectedSpawn.totalSpawnAmount > minSpawns;
        rightButton.interactable = isItemSelectedAndNotKeira && SelectedSpawn.totalSpawnAmount < maxSpawns;
    }

    protected override void PostOnSelectUIItem(GameObject newItem) {
        base.PostOnSelectUIItem(newItem);
        PLESpawn spawn = newItem.GetComponent<PLESpawn>();
        UpdateFields(spawn.totalSpawnAmount, spawn.spawnType.SpawnDisplayName());
    }

    #endregion

    #region Private Interface
    private void UpdateAmtField(int i_amt, bool makeEmpty = false) {
        if (makeEmpty) {
            amountField.text = "";
        }
        else {
            string toSet = Convert.ToString(i_amt);
            amountField.text = toSet;
        }
    }

    private void UpdateFields(int spawnAmount, string newName, bool isAmountEmpty=false) {
        UpdateAmtField(spawnAmount, isAmountEmpty);
        nameText.text = newName;
    }

    private void ChangeSpawnAmount(int newAmount) {
        UpdateAmtField(newAmount);
        SetAmtOnSelectedSpawn();
        SetInteractability();
    }
    #endregion

}
