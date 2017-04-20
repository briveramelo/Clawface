using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ModMan;
using System.Linq;

public class ModInventory : MonoBehaviour {

    [SerializeField] private GameObject baton, blaster, boomerang, dice, geyser, grappler, segway; 
    private Dictionary<ModType, InventoryItem> modInventory;


	// Use this for initialization
	void Awake () {
		modInventory = new Dictionary<ModType, InventoryItem>() {
            {ModType.ArmBlaster, new InventoryItem(transform, blaster) },
            {ModType.Boomerang, new InventoryItem(transform, boomerang) },
            {ModType.Dice, new InventoryItem(transform, dice) },
            {ModType.ForceSegway, new InventoryItem(transform, segway) },
            {ModType.Geyser, new InventoryItem(transform, geyser) },
            {ModType.Grappler, new InventoryItem(transform, grappler) },
            {ModType.StunBaton, new InventoryItem(transform, baton) },            
        };
	}

    void Update() {
        if (Input.GetKeyDown(KeyCode.Keypad1)) {
            UnlockAllMods();
        }
    }

    void UnlockAllMods() {
        modInventory.ForEach((key, value)=>value.isCollected=true);        
    }

    public bool IsModAvailable(ModType modType) {
        if (modInventory.ContainsKey(modType)) {
            return modInventory[modType].isCollected;
        }
        return false;
    }

    List<ModType> availableMods=new List<ModType>();
    public List<ModType> GetAvailableModTypes() {
        availableMods.Clear();
        foreach (KeyValuePair<ModType, InventoryItem> mod in modInventory) {
            if (mod.Value.isCollected) {
                availableMods.Add(mod.Key);
            }
        }
        return availableMods;
    }

    public Mod GetMod(ModType modType, ModSpot modSpot) {
        if (modInventory.ContainsKey(modType)) {
            return modInventory[modType].GetMod(modSpot);
        }
        return null;
    }

    public void CollectMod(ModType modType) {
        if (modInventory.ContainsKey(modType) && !modInventory[modType].isCollected) {            
            modInventory[modType].isCollected = true;
        }
    }

    class InventoryItem {
        public Transform parent;
        public GameObject modPrefab;
        public Dictionary<ModSpot, ModSocket> modSockets;
        public bool isCollected;

        public InventoryItem(Transform parent, GameObject modPrefab) {
            this.parent = parent;
            this.modPrefab=modPrefab;
            Initialize();
        }


        private void Initialize() {
            GameObject baby = new GameObject(modPrefab.name);
            baby.transform.SetParent(parent);
            Transform babyTran =baby.transform;
            modSockets=new Dictionary<ModSpot, ModSocket>() {
                {ModSpot.ArmL, new ModSocket(Instantiate(modPrefab, babyTran)) },
                {ModSpot.ArmR, new ModSocket(Instantiate(modPrefab, babyTran)) },
                {ModSpot.Legs, new ModSocket(Instantiate(modPrefab, babyTran)) },
            };
            modSockets.ForEach((key, value)=>value.modObject.SetActive(false));
        }

        public Mod GetMod(ModSpot modSpot) {            
            if (!modSockets[modSpot].isEquipped) {
                modSockets[modSpot].modObject.SetActive(true);
                return modSockets[modSpot].mod;
            }
            return null;
        }

        public void HideMod(ModSpot modSpot) {
            modSockets[modSpot].modObject.SetActive(true);
        }
    }

    private class ModSocket {
        public GameObject modObject;
        public Mod mod;
        public bool isEquipped { get{ return modObject.activeSelf;} }

        public ModSocket(GameObject modObject) {
            this.modObject=modObject;
            mod=modObject.GetComponent<Mod>();
        }
    }
}
