using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class SceneTracker {

    public static string CurrentSceneName { get { return SceneManager.GetActiveScene().name; } }
    public static bool IsCurrentSceneEditor { get { return CurrentSceneName == Strings.Scenes.SceneNames.Editor; } }
    public static bool IsCurrentScene80sShit { get { return CurrentSceneName == Strings.Scenes.SceneNames.Arena; } }
    public static bool IsCurrentSceneMain { get { return CurrentSceneName == Strings.Scenes.SceneNames.MainMenu; } }
    public static bool IsCurrentScenePlayerLevels { get { return CurrentSceneName == Strings.Scenes.SceneNames.PlayerLevels; } }
    public static bool IsCurrentSceneMovie { get { return CurrentSceneName == Strings.Scenes.SceneNames.Movie; } }

    public static bool IsSceneArena(string sceneNameOrPath) {
        return sceneNameOrPath == Strings.Scenes.ScenePaths.Arena || sceneNameOrPath == Strings.Scenes.ScenePaths.PlayerLevels ||
            sceneNameOrPath == Strings.Scenes.SceneNames.Arena || sceneNameOrPath == Strings.Scenes.SceneNames.PlayerLevels;
    }
    public static bool IsTargetSceneEditor(string sceneNameOrPath) {
        return sceneNameOrPath == Strings.Scenes.ScenePaths.Editor || sceneNameOrPath == Strings.Scenes.SceneNames.Editor;
    }
}
