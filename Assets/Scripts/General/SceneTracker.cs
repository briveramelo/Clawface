using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class SceneTracker {

    public static string CurrentSceneName { get { return SceneManager.GetActiveScene().name; } }
    public static bool IsCurrentSceneEditor { get { return CurrentSceneName == Strings.Scenes.SceneNames.Editor; } }
    public static bool IsCurrentScene80sShit { get { return CurrentSceneName == Strings.Scenes.SceneNames.Arena; } }
    public static bool IsCurrentSceneMain { get { return CurrentSceneName == Strings.Scenes.SceneNames.MainMenu; } }
    public static bool IsCurrentScenePlayerLevels { get { return CurrentSceneName == Strings.Scenes.SceneNames.PlayerLevels; } }

    public static bool IsSceneArena(string scenePath) {
        return scenePath == Strings.Scenes.ScenePaths.Arena || scenePath == Strings.Scenes.ScenePaths.PlayerLevels;
    }
}
