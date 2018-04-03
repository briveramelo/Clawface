using UnityEngine.UI;
using UnityEngine;
using System.Collections.Generic;

public class SpawnScrollGroup : ScrollGroup {    

    protected override string ResourcesPath { get { return Strings.Editor.SPAWN_OBJECTS_PATH; } }

    public void HandleSpawnUIInteractability() {
        TryInitialize();
        pleUIItems.ForEach(item => {
            PLESpawn spawn = item.pleItem as PLESpawn;
            bool isInteractable = PLESpawnManager.Instance.SpawnsUnderMaximum(spawn);
            item.ToggleInteractable(isInteractable);
        });

        bool keiraExists = SpawnMenu.playerSpawnInstance != null;
        KeiraUIItem.ToggleInteractable(!keiraExists);

        PLEUIItem lastItem = GetLastUIItem();
        if (!lastItem.isInteractable) {
            placementMenu.TrySelectFirstAvailable();
        }
    }

    protected override void InitializeUI() {
        base.InitializeUI();
        List<PLESpawn> spawns = new List<PLESpawn>();
        pleItems.ForEach(item => {
            PLESpawn spawn = item as PLESpawn;
            if (spawn!=null && spawn.spawnType!=SpawnType.Keira) {
                spawns.Add(spawn);
            }
        });
        PLESpawnManager.Instance.SetSpawnTypes(spawns);
    }

    PLEUIItem KeiraUIItem { get { TryInitialize(); return pleUIItems.Find(item => { return (item.pleItem as PLESpawn).spawnType == SpawnType.Keira; }); } }
}
