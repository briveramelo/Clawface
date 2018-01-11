//BRANDON RIVERA-MELO

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ModMan;
using System.Linq;

public class ModInventory : MonoBehaviour {

    public GameObject blaster, boomerang, missile, geyser, grappler, segway; 
    private Dictionary<ModType, ModInventoryPack> modInventory;
    private List<ModType> availableMods = new List<ModType>();


    // Use this for initialization
    void Awake () {
		modInventory = new Dictionary<ModType, ModInventoryPack>() {
            {ModType.Blaster, new ModInventoryPack(transform, blaster) },
            {ModType.Boomerang, new ModInventoryPack(transform, boomerang) },
            {ModType.Missile, new ModInventoryPack(transform, missile) },
            {ModType.SpreadGun, new ModInventoryPack(transform, segway) },
            {ModType.Geyser, new ModInventoryPack(transform, geyser) },
            {ModType.LightningGun, new ModInventoryPack(transform, grappler) },          
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

    
    public List<ModType> GetAvailableModTypes() {
        return availableMods;
    }

    public Transform GetModParent(ModType modType) {
        if (modInventory.ContainsKey(modType)) {
            return modInventory[modType].parent;
        }
        return null;
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
            availableMods.Add(modType);
        }
    }

    public bool IsModCollected(ModType modType) {
        if (modInventory.ContainsKey(modType)) {
            return modInventory[modType].isCollected;
        }
        return false;
    }

    class ModInventoryPack {
        public Transform parent;
        public GameObject modPrefab;
        public Dictionary<ModSpot, ModContainer> modContainers;
        public bool isCollected;

        public ModInventoryPack(Transform parent, GameObject modPrefab) {
            this.parent = parent;
            this.modPrefab=modPrefab;
            Initialize();
        }


        private void Initialize() {
            GameObject baby = new GameObject(modPrefab.name);
            baby.transform.SetParent(parent);
            Transform babyTran =baby.transform;
            modContainers=new Dictionary<ModSpot, ModContainer>() {
                {ModSpot.ArmL, new ModContainer(Instantiate(modPrefab, babyTran)) },
                {ModSpot.ArmR, new ModContainer(Instantiate(modPrefab, babyTran)) },
                //{ModSpot.Legs, new ModContainer(Instantiate(modPrefab, babyTran)) },
            };
            modContainers.ForEach((key, value)=>value.modObject.SetActive(false));
        }

        public Mod GetMod(ModSpot modSpot) {            
            if (!modContainers[modSpot].isEquipped && modContainers[modSpot].modObject) {
                modContainers[modSpot].modObject.SetActive(true);
                return modContainers[modSpot].mod;
            }
            return null;
        }

        public void HideMod(ModSpot modSpot) {
            modContainers[modSpot].modObject.SetActive(true);
        }
    }

    private class ModContainer {
        public GameObject modObject;
        public Mod mod;
        public bool isEquipped {
            get {
                if (modObject)
                {
                    return modObject.activeSelf;
                }
                else
                {
                    return false;
                }
            }
        }

        public ModContainer(GameObject modObject) {
            this.modObject=modObject;
            mod=modObject.GetComponent<Mod>();
        }
    }
}
