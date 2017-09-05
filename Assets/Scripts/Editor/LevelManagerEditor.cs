// LevelManagerEditor.cs
// Author: Aaron

using Turing.LevelEditor;

using UnityEditor;

/// <summary>
/// Custom editor for LevelManager.
/// </summary>
[CustomEditor(typeof(LevelManager))]
public sealed class LevelManagerEditor : Editor
{
    #region Private Fields

    /// <summary>
    /// Target LevelManager.
    /// </summary>
    LevelManager LMTarget;

    #endregion
    #region Unity Lifecycle

    public override void OnInspectorGUI()
    {
        LMTarget = target as LevelManager;

        EditorGUILayout.LabelField("Level loaded: " +
            LMTarget.LevelLoaded.ToString());

        base.OnInspectorGUI();
    }

    #endregion
}
