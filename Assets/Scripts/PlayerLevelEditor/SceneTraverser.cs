using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneTraverser : MonoBehaviour {

    public List<string> scenesAllowedToLiveIn = new List<string>();

    public void Initialize(params string[] allowedSceneNames) {
        foreach (string allowedScene in allowedSceneNames) {
            this.scenesAllowedToLiveIn.Add(allowedScene);
        }
        DontDestroyOnLoad(gameObject);
    }

    private void OnLevelWasLoaded(int level) {
        if (!scenesAllowedToLiveIn.Contains(SceneTracker.CurrentLevelName)) {
            Destroy(gameObject);
        }
    }
}
