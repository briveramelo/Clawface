// ObjectDatabaseDrawer.cs

using UnityEngine;
using UnityEditor;

namespace Turing.LevelEditor
{
    /// <summary>
    /// Custom PropertyDrawer for ObjectDatabases.
    /// </summary>
    [CustomPropertyDrawer(typeof(ObjectDatabase))]
    public class ObjectDatabaseDrawer : PropertyDrawer
    {
        #region Vars

        /// <summary>
        /// Target ObjectDatabase.
        /// </summary>
        ObjectDatabase _target;

        /// <summary>
        /// Database SerializedProperty.
        /// </summary>
        SerializedProperty _dataProp;

        float _partWidth;

        #endregion
        #region Overrides

        public override void OnGUI(Rect position, SerializedProperty property, 
            GUIContent label) 
        {
            _partWidth = position.width / (
                ObjectDataDrawer.INDEX_LABEL_PARTS + 
                ObjectDataDrawer.PREFAB_FIELD_PARTS + 
                ObjectDataDrawer.LIMIT_FIELD_PARTS + 
                ObjectDataDrawer.CATEGORY_DROPDOWN_PARTS + 
                ObjectDataDrawer.SNAPMODE_DROPDOWN_PARTS);

            EditorGUILayout.BeginHorizontal();

            // Index field
            EditorGUILayout.LabelField("Index", EditorStyles.boldLabel, 
                GUILayout.MinWidth(ObjectDataDrawer.INDEX_LABEL_PARTS * _partWidth));

            // Prefab field
            EditorGUILayout.LabelField("Prefab", EditorStyles.boldLabel, 
                GUILayout.MinWidth(ObjectDataDrawer.PREFAB_FIELD_PARTS * _partWidth));

            // Limit field
            EditorGUILayout.LabelField("Limit", EditorStyles.boldLabel, 
                GUILayout.MinWidth(ObjectDataDrawer.LIMIT_FIELD_PARTS * _partWidth));

            // Category field
            EditorGUILayout.LabelField("Category", EditorStyles.boldLabel, 
                GUILayout.MinWidth(ObjectDataDrawer.CATEGORY_DROPDOWN_PARTS * _partWidth));

            // Snap mode field
            EditorGUILayout.LabelField("Snap Mode", EditorStyles.boldLabel, 
                GUILayout.MinWidth(ObjectDataDrawer.SNAPMODE_DROPDOWN_PARTS * _partWidth));

            EditorGUILayout.EndHorizontal();

            // Draw data
            EditorGUILayout.BeginVertical();

            _dataProp = property.FindPropertyRelative("_data");
            for (int i = 0; i < _dataProp.arraySize; i++)
                EditorGUILayout.PropertyField(_dataProp.GetArrayElementAtIndex(i));

            EditorGUILayout.EndVertical();
        }

        #endregion
    }
}