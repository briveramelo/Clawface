using UnityEngine.UI;
using UnityEngine;

public class SpawnScrollGroup : ScrollGroup {    

    protected override string ResourcesPath { get { return Strings.Editor.SPAWN_OBJECTS_PATH; } }
    DataSave ActiveDataSave { get { return DataPersister.ActiveDataSave; } }

    public virtual void SetSpawnUIInteractability(int currentWave) {
        LevelData workingLevelData = ActiveDataSave.workingLevelData;
        pleUIItems.ForEach(item => {
            PLESpawn spawn = item.pleItem as PLESpawn;
            int numberOfSpawns = workingLevelData.NumSpawns(spawn.spawnType, currentWave);
            bool isInteractable = spawn.MaxPerWave > numberOfSpawns;
            item.ToggleInteractable(isInteractable);
        });
    }

    public PLEUIItem SelectKeira() {
        PLEUIItem keiraItem = pleUIItems.Find(item => {
            return (item.pleItem as PLESpawn).spawnType == SpawnType.Keira;
        });
        SelectUIItem(keiraItem.ItemIndex);
        return keiraItem;
    }
}
