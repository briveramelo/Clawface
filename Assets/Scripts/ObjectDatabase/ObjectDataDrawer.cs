using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(ObjectData))]
public class ObjectDataDrawer : PropertyDrawer {

    const int _INDEX_LABEL_WIDTH = 64;
    const int _PATH_LABEL_WIDTH = 256;
    const int _PREFAB_FIELD_WIDTH = 128;
    const int _CATEGORY_DROPDOWN_WIDTH = 128;

    public static int PropertyWidth {
        get {
            return _INDEX_LABEL_WIDTH + _PATH_LABEL_WIDTH +
                _PREFAB_FIELD_WIDTH + _CATEGORY_DROPDOWN_WIDTH;
        }
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {

        EditorGUI.BeginChangeCheck();

        // Index property
        var indexProperty = property.FindPropertyRelative("index");
        var value = indexProperty.intValue;
        if (value == (int)byte.MaxValue) {
            EditorGUI.LabelField (new Rect (position.x, position.y, position.width, position.height), 
                "***Index 255 is reserved for empty tiles!***");
            return;
        }

        string index = value.ToString();
        EditorGUI.LabelField (new Rect (position.x, position.y, _INDEX_LABEL_WIDTH, position.height), index);

        // Path property
        var pathProperty = property.FindPropertyRelative("path");
        string path = pathProperty.stringValue;
        EditorGUI.LabelField (new Rect (position.x + _INDEX_LABEL_WIDTH, position.y, _PATH_LABEL_WIDTH, position.height), path);

        // Prefab property
        var prefabProperty = property.FindPropertyRelative("prefab");
        GameObject currentPrefab = (GameObject)prefabProperty.objectReferenceValue;
        GameObject pickedPrefab = (GameObject)EditorGUI.ObjectField (new Rect (position.x + _INDEX_LABEL_WIDTH +_PATH_LABEL_WIDTH, position.y, _PREFAB_FIELD_WIDTH, position.height), currentPrefab, typeof(GameObject), false);
        if (currentPrefab != pickedPrefab) {
            prefabProperty.objectReferenceValue = pickedPrefab;
            if (pickedPrefab == null) pathProperty.stringValue = "UNUSED";
            else pathProperty.stringValue = AssetDatabase.GetAssetPath (pickedPrefab);
        }

        // Category property
        var categoryProperty = property.FindPropertyRelative("category");
        categoryProperty.enumValueIndex = (int)(ObjectDatabase.Category)EditorGUI.EnumPopup (
            new Rect (position.x + _INDEX_LABEL_WIDTH + _PATH_LABEL_WIDTH + _PREFAB_FIELD_WIDTH, position.y,
            _CATEGORY_DROPDOWN_WIDTH, position.height), (ObjectDatabase.Category)categoryProperty.enumValueIndex);

        EditorGUI.EndChangeCheck();
    }
}
