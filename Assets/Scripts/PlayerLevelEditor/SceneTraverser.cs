using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneTraverser : MonoBehaviour {


    private void OnDestroy() {
        if (EventSystem.Instance) {
            EventSystem.Instance.UnRegisterEvent(Strings.Events.SCENE_LOADED, OnSceneChangeComplete);
        }
    }

    public List<string> scenesAllowedToLiveIn = new List<string>();

    public void Initialize(params string[] allowedSceneNames) {
        EventSystem.Instance.RegisterEvent(Strings.Events.SCENE_LOADED, OnSceneChangeComplete);
        foreach (string allowedScene in allowedSceneNames) {
            this.scenesAllowedToLiveIn.Add(allowedScene);
        }
        DontDestroyOnLoad(gameObject);
    }

    private void OnSceneChangeComplete(params object[] parameters) {
        if (!scenesAllowedToLiveIn.Contains(SceneTracker.CurrentSceneName)) {
            Destroy(gameObject);
        }
    }
}
