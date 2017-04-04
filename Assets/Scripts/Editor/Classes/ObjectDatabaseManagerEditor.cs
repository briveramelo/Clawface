// ObjectDatabaseManagerEditor.cs

using UnityEngine;
using UnityEditor;

/// <summary>
/// Custom editor for OBJDB manager.
/// </summary>
[CustomEditor(typeof(ObjectDatabaseManager))]
public class ObjectDatabaseManagerEditor : Editor {

    #region Vars

    /// <summary>
    /// Target OBJDB manager.
    /// </summary>
    ObjectDatabaseManager _target;

    SerializedObject _serializedTarget;

    #endregion
    #region Serialized Properties

    SerializedProperty _databaseProp;

    #endregion
    #region Unity Callbacks

    void OnEnable() {
        _target = target as ObjectDatabaseManager;
        _serializedTarget = new SerializedObject (_target);
        _databaseProp = _serializedTarget.FindProperty("_database");
    }

    #endregion
    #region Unity Overrides

    public override void OnInspectorGUI() {

        _serializedTarget.Update();

        EditorGUILayout.BeginVertical (EditorStyles.helpBox);

        // Rebuild categories button
        if (GUILayout.Button("Rebuild categories")) {
            _target.RebuildCategories();
        }

        // JSON save button
        if (GUILayout.Button("Save database to JSON")) {
            _target.SaveToJSON();
        }

        // JSON load button
        if (GUILayout.Button("Load database from JSON")) {
            _target.LoadFromJSON();
        }

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginVertical (EditorStyles.helpBox);

        EditorGUILayout.PropertyField(_databaseProp);

        GUILayout.Space(4);

        //if (GUILayout.Button("Add object")) _target.AddObject();

        EditorGUILayout.EndVertical();

        _serializedTarget.ApplyModifiedProperties();
    }

    #endregion
}
