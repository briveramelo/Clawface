using UnityEngine.UI;
using UnityEngine;

public class SpawnScrollGroup : ScrollGroup {    

    protected override string ResourcesPath { get { return Strings.Editor.SPAWN_OBJECTS_PATH; } }
    DataSave ActiveDataSave { get { return DataPersister.ActiveDataSave; } }

    public virtual void HandleSpawnUIInteractability(int currentWave) {
        LevelData workingLevelData = ActiveDataSave.workingLevelData;
        pleUIItems.ForEach(item => {
            PLESpawn spawn = item.pleItem as PLESpawn;
            int numberOfSpawns = workingLevelData.NumSpawns(spawn.spawnType, currentWave);
            bool isInteractable = spawn.MaxPerWave > numberOfSpawns;
            item.ToggleInteractable(isInteractable);
        });

        bool keiraExists = SpawnMenu.playerSpawnInstance != null;
        KeiraUIItem.ToggleInteractable(!keiraExists);

        PLEUIItem lastItem = GetLastUIItem();
        if (!lastItem.isInteractable) {
            placementMenu.TrySelectFirstAvailable();
        }
    }

    public PLEUIItem SelectKeira() {
        PLEUIItem keiraItem = KeiraUIItem;
        SelectUIItem(keiraItem.ItemIndex);
        return keiraItem;
    }

    PLEUIItem KeiraUIItem { get { return pleUIItems.Find(item => { return (item.pleItem as PLESpawn).spawnType == SpawnType.Keira; }); } }
}
