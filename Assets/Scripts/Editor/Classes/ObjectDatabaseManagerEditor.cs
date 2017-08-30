// ObjectDatabaseManagerEditor.cs
// Author: Aaron

using Turing.LevelEditor;

using UnityEditor;

using UnityEngine;

/// <summary>
/// Custom editor for OBJDB manager.
/// </summary>
[CustomEditor(typeof(ObjectDatabaseManager))]
public class ObjectDatabaseManagerEditor : Editor
{
    #region Vars

    /// <summary>
    /// Target OBJDB manager.
    /// </summary>
    ObjectDatabaseManager objDBTarget;

    SerializedObject serializedTarget;

    #endregion
    #region Serialized Properties

    SerializedProperty databaseProp;

    #endregion
    #region Unity Callbacks

    void OnEnable()
    {
        target = target as ObjectDatabaseManager;
        serializedTarget = new SerializedObject(target);
        databaseProp = serializedTarget.FindProperty("_database");
    }

    #endregion
    #region Unity Overrides

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