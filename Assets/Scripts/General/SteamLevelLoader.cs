using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteamLevelLoader : MonoBehaviour {

    public bool IsLevelsLoaded { get; private set; }
    private System.Action onLevelsLoadedExternal;

    public void LoadSteamworkshopFiles(System.Action onLevelsLoadedExternal=null) {
        IsLevelsLoaded = false;
        this.onLevelsLoadedExternal = onLevelsLoadedExternal;
        SteamAdapter.LoadSteamLevelData(OnAllLevelsLoaded);
    }

    private void OnAllLevelsLoaded() {
        IsLevelsLoaded = true;
        if (onLevelsLoadedExternal!=null) {
            onLevelsLoadedExternal();
        }
    }
}
