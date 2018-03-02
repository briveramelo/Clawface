using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class SceneTracker {

    public static string CurrentLevelName { get { return SceneManager.GetActiveScene().name; } }
    public static bool IsCurrentSceneEditor { get { return CurrentLevelName == Strings.Scenes.SceneNames.Editor; } }
    public static bool IsCurrentScene80sShit { get { return CurrentLevelName == Strings.Scenes.SceneNames.Arena; } }
    public static bool IsCurrentSceneMain { get { return CurrentLevelName == Strings.Scenes.SceneNames.MainMenu; } }
    public static bool IsCurrentScenePlayerLevels { get { return CurrentLevelName == Strings.Scenes.SceneNames.PlayerLevels; } }
}
