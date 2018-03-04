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

    #region Private Interface
    
    private void UpdateAmtField(int i_amt, bool makeEmpty = false)
    {
        if(makeEmpty)
        {
            amountField.text = "";
        }
        else
        {
            string toSet = Convert.ToString(i_amt);
            amountField.text = toSet;
        }
    }

   
    private Transform TryCreateWaveParent(int i)
    {
        string waveName = levelEditor.GetWaveName(i);
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
        int currentWave = PLESpawnManager.Instance.CurrentWave;
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
        amountField.interactable = true;
        UpdateAmtField(selectedSpawn.totalSpawnAmount);
    }
    protected override void DeselectItem() {
        amountField.interactable = false;
        UpdateAmtField(0, true);
        if (selectedSpawn!=null) {
            selectedSpawn.Deselect();
            selectedSpawn = null;
        }
    }


    #endregion
}
