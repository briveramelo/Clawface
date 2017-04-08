// ObjectDatabaseDrawer.cs

using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(ObjectDatabase))]
public class ObjectDatabaseDrawer : PropertyDrawer {

    ObjectDatabase _target;
    SerializedProperty _dataProp;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField ("Index", GUILayout.MinWidth(position.width * ObjectDataDrawer.INDEX_LABEL_PERCENT));
        EditorGUILayout.LabelField ("Path", GUILayout.MinWidth(position.width * ObjectDataDrawer.PATH_LABEL_PERCENT));
        EditorGUILayout.LabelField ("Prefab", GUILayout.MinWidth(position.width * ObjectDataDrawer.PREFAB_FIELD_PERCENT));
        EditorGUILayout.LabelField ("Limit", GUILayout.MinWidth(position.width * ObjectDataDrawer.LIMIT_FIELD_PERCENT));
        EditorGUILayout.LabelField ("Category", GUILayout.MinWidth(position.width * ObjectDataDrawer.CATEGORY_DROPDOWN_PERCENT));
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginVertical();

        _dataProp = property.FindPropertyRelative ("_data");
        for (int i = 0; i < _dataProp.arraySize; i++)
            EditorGUILayout.PropertyField (_dataProp.GetArrayElementAtIndex(i));

        EditorGUILayout.EndVertical();
    }
}
