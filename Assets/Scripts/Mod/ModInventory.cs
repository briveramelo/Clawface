using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModInventory : MonoBehaviour {

    private Dictionary<ModType, InventoryItem> modInventory;


	// Use this for initialization
	void Start () {
		modInventory = new Dictionary<ModType, InventoryItem>() {
            {ModType.ArmBlaster, new InventoryItem() },
            {ModType.Boomerang, new InventoryItem() },
            {ModType.Dice, new InventoryItem() },
            {ModType.ForceSegway, new InventoryItem() },
            {ModType.Geyser, new InventoryItem() },
            {ModType.Grappler, new InventoryItem() },
            {ModType.StunBaton, new InventoryItem() },
            {ModType.TankTreads, new InventoryItem() },
        };
	}

    public bool IsModAvailable(ModType modType) {
        if (modInventory.ContainsKey(modType)) {
            return modInventory[modType].isCollected;
        }
        return false;
    }

    public void CollectMod(ModType modType) {
        if (!modInventory.ContainsKey(modType)) {
            modInventory.Add(modType, new InventoryItem());
        }        
        modInventory[modType].isCollected = true;
    }

    class InventoryItem {
        public bool isCollected;
    }
}
