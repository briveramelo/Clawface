using UnityEngine.UI;
using UnityEngine;

public class SpawnScrollGroup : ScrollGroup {

    protected override string ResourcesPath { get { return Strings.Editor.SPAWN_OBJECTS_PATH; } }
    int keiraIndex=0;

    protected override void InitializeUI() {
        base.InitializeUI();
        keiraIndex = PutKeiraFirst();
        SelectItem(keiraIndex);
    }

    int PutKeiraFirst() {
        for (int i = 0; i < pleUIItems.Count; i++) {
            PLEUIItem item = pleUIItems[i];
            if (item.registeredItem.GetComponent<PLESpawn>().spawnType == SpawnType.Keira) {
                item.gameObject.transform.SetAsFirstSibling();
                return item.ItemIndex;
            }
        }
        return 0;
    }

}
