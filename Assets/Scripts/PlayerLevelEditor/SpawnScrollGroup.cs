using UnityEngine.UI;
using UnityEngine;

public class SpawnScrollGroup : ScrollGroup {    

    protected override string ResourcesPath { get { return Strings.Editor.SPAWN_OBJECTS_PATH; } }
    protected override string IconImagePath { get { return Strings.Editor.SPAWN_ICON_IMAGE_PREVIEW_PATH; } }

    public virtual void SetSpawnUIInteractability(int currentWave) {
        LevelData activeLevelData = DataPersister.ActiveDataSave.ActiveLevelData;
        pleUIItems.ForEach(item => {
            PLESpawn spawn = item.pleItem as PLESpawn;
            int numberOfSpawns = activeLevelData.NumSpawns(spawn.spawnType, currentWave);
            bool isInteractable = spawn.MaxPerWave > numberOfSpawns;
            item.ToggleInteractable(isInteractable);
        });
    }

    public void SelectKeira() {
        int keiraIndex = pleUIItems.FindIndex(item => {
            return (item.pleItem as PLESpawn).spawnType == SpawnType.Keira;
        });
        SelectItem(keiraIndex);
    }
}
