// ObjectDatabaseManagerEditor.cs
// Author: Aaron

using Turing.LevelEditor;

using UnityEditor;

using UnityEngine;

/// <summary>
/// Custom editor for OBJDB manager.
/// </summary>
[CustomEditor(typeof(ObjectDatabaseManager))]
public sealed class ObjectDatabaseManagerEditor : Editor
{
    #region Private Fields

    /// <summary>
    /// Target OBJDB manager.
    /// </summary>
    ObjectDatabaseManager objDBTarget;

    SerializedObject serializedTarget;

    SerializedProperty databaseProp;

    #endregion
    #region Unity Lifecycle

    void OnEnable()
    {
        objDBTarget = target as ObjectDatabaseManager;
        serializedTarget = new SerializedObject(objDBTarget);
        databaseProp = serializedTarget.FindProperty("database");
    }

    public override void OnInspectorGUI()
    {
        serializedTarget.Update();

        EditorGUILayout.BeginVertical(EditorStyles.helpBox);

        // Rebuild categories button
        if (GUILayout.Button("Rebuild categories"))
        {
            objDBTarget.RebuildCategories();
        }

        // JSON save button
        if (GUILayout.Button("Save database to JSON"))
        {
            objDBTarget.SaveToJSON();
        }

        // JSON load button
        if (GUILayout.Button("Load database from JSON"))
        {
            objDBTarget.LoadFromJSON();
        }

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginVertical(EditorStyles.helpBox);

        EditorGUILayout.PropertyField(databaseProp);

        GUILayout.Space(4);

        //if (GUILayout.Button("Add object")) _target.AddObject();

        EditorGUILayout.EndVertical();

        serializedTarget.ApplyModifiedProperties();
    }

    #endregion
}