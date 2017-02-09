//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEditor;

//[CustomPropertyDrawer(typeof(ObjectData))]
//public class ObjectDataDrawer : PropertyDrawer {

//    const int _INDEX_LABEL_WIDTH = 64;
//    const int _PATH_LABEL_WIDTH = 256;
//    const int _PREFAB_FIELD_WIDTH = 128;
//    const int _LIMIT_FIELD_WIDTH = 128;
//    const int _CATEGORY_DROPDOWN_WIDTH = 128;

//    const float _INDEX_LABEL_PERCENT = 0.15f;
//    const float _PATH_LABEL_PERCENT = 0.3f;
//    const float _PREFAB_FIELD_PERCENT = 0.25f;
//    const float _LIMIT_FIELD_PERCENT = 0.15f;
//    const float _CATEGORY_DROPDOWN_PERCENT = 0.15f;

//    public static int PropertyWidth {
//        get {
//            return _INDEX_LABEL_WIDTH + _PATH_LABEL_WIDTH +
//                _PREFAB_FIELD_WIDTH + _LIMIT_FIELD_WIDTH + _CATEGORY_DROPDOWN_WIDTH;
//        }
//    }

//    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {

//        EditorGUI.BeginChangeCheck();

//        var currentX = position.x;
//        var indexLabelWidth = position.width * _INDEX_LABEL_PERCENT;

//        // Index property
//        var indexProperty = property.FindPropertyRelative("index");
//        var value = indexProperty.intValue;
//        if (value == (int)byte.MaxValue) {
//            EditorGUI.LabelField (new Rect (currentX, position.y,  position.width, position.height), 
//                "***Index 255 is reserved for empty tiles!***");
//            return;
//        }

//        string index = value.ToString();
//        //EditorGUI.LabelField (new Rect (currentX, position.y, _INDEX_LABEL_WIDTH, position.height), index);
//        EditorGUI.LabelField (new Rect (currentX, position.y, indexLabelWidth, position.height), index);

//        //currentX += _INDEX_LABEL_WIDTH;
//        currentX += indexLabelWidth;
//        var pathLabelWidth = position.width * _PATH_LABEL_PERCENT;

//        // Path property
//        var pathProperty = property.FindPropertyRelative("path");
//        string path = pathProperty.stringValue;
//        //EditorGUI.LabelField (new Rect (currentX, position.y, _PATH_LABEL_WIDTH, position.height), path);
//        EditorGUI.LabelField (new Rect (currentX, position.y, pathLabelWidth, position.height), path);

//        //currentX += _PATH_LABEL_WIDTH;
//        currentX += pathLabelWidth;
//        var prefabFieldWidth = position.width * _PREFAB_FIELD_PERCENT;

//        // Prefab property
//        var prefabProperty = property.FindPropertyRelative("prefab");
//        GameObject currentPrefab = (GameObject)prefabProperty.objectReferenceValue;
//        //GameObject pickedPrefab = (GameObject)EditorGUI.ObjectField (new Rect (currentX, position.y, _PREFAB_FIELD_WIDTH, position.height), currentPrefab, typeof(GameObject), false);
//        GameObject pickedPrefab = (GameObject)EditorGUI.ObjectField (new Rect (currentX, position.y, prefabFieldWidth, position.height), currentPrefab, typeof(GameObject), false);
//        if (currentPrefab != pickedPrefab) {
//            prefabProperty.objectReferenceValue = pickedPrefab;
//            if (pickedPrefab == null) pathProperty.stringValue = "UNUSED";
//            else pathProperty.stringValue = AssetDatabase.GetAssetPath (pickedPrefab);
//        }

//        //currentX += _PREFAB_FIELD_WIDTH;
//        currentX += prefabFieldWidth;
//        var limitFieldWidth = position.width * _LIMIT_FIELD_PERCENT;

//        // Limit property
//        var limitProperty = property.FindPropertyRelative("limit");
//        //limitProperty.intValue = EditorGUI.IntField (new Rect (currentX, position.y, _LIMIT_FIELD_WIDTH, position.height), "Limit",  limitProperty.intValue);
//        limitProperty.intValue = EditorGUI.IntField (new Rect (currentX, position.y, limitFieldWidth, position.height),  limitProperty.intValue);

//        //currentX += _LIMIT_FIELD_WIDTH;
//        currentX += limitFieldWidth;
//        var categoryDropdownWidth = position.width * _CATEGORY_DROPDOWN_PERCENT;

//        // Category property
//        var categoryProperty = property.FindPropertyRelative("category");
//        /*categoryProperty.enumValueIndex = (int)(ObjectDatabase.Category)EditorGUI.EnumPopup (
//            new Rect (currentX, position.y,
//            _CATEGORY_DROPDOWN_WIDTH, position.height), (ObjectDatabase.Category)categoryProperty.enumValueIndex);*/
//        categoryProperty.enumValueIndex = (int)(ObjectDatabase.Category)EditorGUI.EnumPopup (
//            new Rect (currentX, position.y,
//            categoryDropdownWidth, position.height), (ObjectDatabase.Category)categoryProperty.enumValueIndex);

//        EditorGUI.EndChangeCheck();
//    }
//}
