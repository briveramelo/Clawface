using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ScreenshotWindow : EditorWindow {

    const string SCREENSHOT_FOLDER_NAME = "Screenshots";

	static ScreenshotWindow Instance;

    static string Filepath;
    string filename;

    [MenuItem("Window/ScreenshotWindow")]
    static void Init()
    {
        if (Instance == null) Instance = ScreenshotWindow.GetWindow<ScreenshotWindow>();
        Filepath = string.Format("{0}/{1}/", Application.dataPath, SCREENSHOT_FOLDER_NAME);
        Instance.Show();
    }

    private void OnFocus()
    {
        Filepath = string.Format("{0}/{1}/", Application.dataPath, SCREENSHOT_FOLDER_NAME);
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField(string.Format("Full path: {0}{1}.png", Filepath, filename));
        filename = EditorGUILayout.TextField("Filename", filename);
        if (GUILayout.Button("Capture")) ScreenCapture.CaptureScreenshot(string.Format("{0}{1}.png", Filepath, filename));
    }
}
