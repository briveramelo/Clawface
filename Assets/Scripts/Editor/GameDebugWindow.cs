using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public class GameDebugWindow : EditorWindow {

    static GameDebugWindow Instance;

    [MenuItem("Window/Game Debug Window")]
	static void Init ()
    {
        Instance = GetWindow<GameDebugWindow>();
        Instance.Show();
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("Play Scenes", EditorStyles.boldLabel);

        if (GUILayout.Button("Play from Main Menu"))
        {
            EditorSceneManager.OpenScene("Assets/Scenes/Build Scenes/MainMenu.unity");
            EditorApplication.ExecuteMenuItem("Edit/Play");
        }

        if (GUILayout.Button("Play from In-Game"))
        {
            EditorSceneManager.OpenScene("Assets/Scenes/Build Scenes/80s shit.unity");
            EditorApplication.ExecuteMenuItem("Edit/Play");
        }

        EditorGUILayout.Separator();

        EditorGUILayout.LabelField("Load Scenes", EditorStyles.boldLabel);

        if (GUILayout.Button("Load Main Menu"))
        {
            EditorSceneManager.OpenScene("Assets/Scenes/Build Scenes/MainMenu.unity");
        }

        if (GUILayout.Button("Load 80s Shit"))
        {
            EditorSceneManager.OpenScene("Assets/Scenes/Build Scenes/80s shit.unity");
        }
    }
}
