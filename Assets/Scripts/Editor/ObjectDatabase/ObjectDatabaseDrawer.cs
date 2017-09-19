// ObjectDatabaseDrawer.cs
// Author: Aaron

using Turing.LevelEditor;

using UnityEditor;

using UnityEngine;

/// <summary>
/// Custom PropertyDrawer for ObjectDatabases.
/// </summary>
[CustomPropertyDrawer(typeof(ObjectDatabase))]
public sealed class ObjectDatabaseDrawer : PropertyDrawer
{
    #region Private Fields

    /// <summary>
    /// Target ObjectDatabase.
    /// </summary>
    ObjectDatabase target;

    /// <summary>
    /// Database SerializedProperty.
    /// </summary>
    SerializedProperty dataProp;

    float partWidth;

    #endregion
    #region Unity Lifecycle

    public override void OnGUI(Rect position, SerializedProperty property,
        GUIContent label)
    {
        partWidth = position.width / (
            ObjectDataDrawer.INDEX_LABEL_PARTS +
            ObjectDataDrawer.PREFAB_FIELD_PARTS +
            ObjectDataDrawer.LIMIT_FIELD_PARTS +
            ObjectDataDrawer.CATEGORY_DROPDOWN_PARTS +
            ObjectDataDrawer.SNAPMODE_DROPDOWN_PARTS);

        EditorGUILayout.BeginHorizontal();

        // Index field
        EditorGUILayout.LabelField("Index", EditorStyles.boldLabel,
            GUILayout.MinWidth(ObjectDataDrawer.INDEX_LABEL_PARTS * partWidth));

        // Prefab field
        EditorGUILayout.LabelField("Prefab", EditorStyles.boldLabel,
            GUILayout.MinWidth(ObjectDataDrawer.PREFAB_FIELD_PARTS * partWidth));

        // Limit field
        EditorGUILayout.LabelField("Limit", EditorStyles.boldLabel,
            GUILayout.MinWidth(ObjectDataDrawer.LIMIT_FIELD_PARTS * partWidth));

        // Category field
        EditorGUILayout.LabelField("Category", EditorStyles.boldLabel,
            GUILayout.MinWidth(ObjectDataDrawer.CATEGORY_DROPDOWN_PARTS * partWidth));

        // Snap mode field
        EditorGUILayout.LabelField("Snap Mode", EditorStyles.boldLabel,
            GUILayout.MinWidth(ObjectDataDrawer.SNAPMODE_DROPDOWN_PARTS * partWidth));

        EditorGUILayout.EndHorizontal();

        // Draw data
        EditorGUILayout.BeginVertical();

        dataProp = property.FindPropertyRelative("data");
        for (int i = 0; i < dataProp.arraySize; i++)
            EditorGUILayout.PropertyField(dataProp.GetArrayElementAtIndex(i));

        EditorGUILayout.EndVertical();
    }

    #endregion
}
