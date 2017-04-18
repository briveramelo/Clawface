// ObjectDataDrawer.cs

using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

/// <summary>
/// Custom proerty drawer for ObjectData class.
/// </summary>
[CustomPropertyDrawer(typeof(ObjectData))]
public class ObjectDataDrawer : PropertyDrawer {

    #region Constants

    const int _INDEX_LABEL_WIDTH = 64;
    const int _PATH_LABEL_WIDTH = 256;
    const int _PREFAB_FIELD_WIDTH = 128;
    const int _LIMIT_FIELD_WIDTH = 128;
    const int _CATEGORY_DROPDOWN_WIDTH = 128;
    const int _SNAPMODE_DROPDOWN_WIDTH = 128;

    public const float INDEX_LABEL_PERCENT = 0.15f;
    public const float PATH_LABEL_PERCENT = 0.3f;
    public const float PREFAB_FIELD_PERCENT = 0.25f;
    public const float LIMIT_FIELD_PERCENT = 0.15f;
    public const float CATEGORY_DROPDOWN_PERCENT = 0.15f;
    public const float SNAPMODE_DROPDOWN_PERCENT = 0.15f;

    public const float INDEX_LABEL_PARTS = 1f;
    public const float PREFAB_FIELD_PARTS = 2f;
    public const float LIMIT_FIELD_PARTS = 1f;
    public const float CATEGORY_DROPDOWN_PARTS = 2f;
    public const float SNAPMODE_DROPDOWN_PARTS = 2f;

    float _partWidth;
    float _indexLabelWidth = 0f;
    float _prefabFieldWidth = 0f;
    float _limitFieldWidth = 0f;
    float _categoryDropdownWidth = 0f;
    float _snapModeDropdownWidth = 0f;

    #endregion
    #region Overrides

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {

        _partWidth = position.width / (INDEX_LABEL_PARTS + PREFAB_FIELD_PARTS + LIMIT_FIELD_PARTS + CATEGORY_DROPDOWN_PARTS + SNAPMODE_DROPDOWN_PARTS);

        EditorGUI.BeginChangeCheck();

        var currentX = position.x;
        //var indexLabelWidth = position.width * INDEX_LABEL_PERCENT;
        _indexLabelWidth = INDEX_LABEL_PARTS * _partWidth;

        // Index property
        var indexProperty = property.FindPropertyRelative("index");
        var value = indexProperty.intValue;
        if (value == (int)byte.MaxValue) {
            EditorGUI.LabelField(new Rect(currentX, position.y, position.width, position.height),
                "***Index 255 is reserved for empty tiles!***");
            return;
        }

        string index = value.ToString();
        //EditorGUI.LabelField (new Rect (currentX, position.y, _INDEX_LABEL_WIDTH, position.height), index);
        EditorGUI.LabelField(new Rect(currentX, position.y, _indexLabelWidth, position.height), index);

        //currentX += _INDEX_LABEL_WIDTH;
        currentX += _indexLabelWidth;
        //var pathLabelWidth = position.width * PATH_LABEL_PERCENT;

        // Path property
        var pathProperty = property.FindPropertyRelative("path");
        //string path = pathProperty.stringValue;
        //EditorGUI.LabelField (new Rect (currentX, position.y, _PATH_LABEL_WIDTH, position.height), path);
        //EditorGUI.LabelField(new Rect(currentX, position.y, pathLabelWidth, position.height), path);

        //currentX += _PATH_LABEL_WIDTH;
        //currentX += pathLabelWidth;
        //var prefabFieldWidth = position.width * PREFAB_FIELD_PERCENT;
        _prefabFieldWidth = PREFAB_FIELD_PARTS * _partWidth;

        // Prefab property
        var prefabProperty = property.FindPropertyRelative("prefab");
        GameObject currentPrefab = (GameObject)prefabProperty.objectReferenceValue;
        //GameObject pickedPrefab = (GameObject)EditorGUI.ObjectField (new Rect (currentX, position.y, _PREFAB_FIELD_WIDTH, position.height), currentPrefab, typeof(GameObject), false);
        GameObject pickedPrefab = (GameObject)EditorGUI.ObjectField(new Rect(currentX, position.y, _prefabFieldWidth, position.height), currentPrefab, typeof(GameObject), false);
        if (currentPrefab != pickedPrefab) {
            prefabProperty.objectReferenceValue = pickedPrefab;
            if (pickedPrefab == null) pathProperty.stringValue = "UNUSED";
            else pathProperty.stringValue = AssetDatabase.GetAssetPath(pickedPrefab);
        }

        //currentX += _PREFAB_FIELD_WIDTH;
        currentX += _prefabFieldWidth;
        //var limitFieldWidth = position.width * LIMIT_FIELD_PERCENT;
        _limitFieldWidth = LIMIT_FIELD_PARTS * _partWidth;

        // Limit property
        var limitProperty = property.FindPropertyRelative("limit");
        //limitProperty.intValue = EditorGUI.IntField (new Rect (currentX, position.y, _LIMIT_FIELD_WIDTH, position.height), "Limit",  limitProperty.intValue);
        limitProperty.intValue = EditorGUI.IntField(new Rect(currentX, position.y, _limitFieldWidth, position.height), limitProperty.intValue);

        //currentX += _LIMIT_FIELD_WIDTH;
        currentX += _limitFieldWidth;
        //var categoryDropdownWidth = position.width * CATEGORY_DROPDOWN_PERCENT;
        _categoryDropdownWidth = CATEGORY_DROPDOWN_PARTS * _partWidth;

        // Category property
        var categoryProperty = property.FindPropertyRelative("category");
        /*categoryProperty.enumValueIndex = (int)(ObjectDatabase.Category)EditorGUI.EnumPopup (
            new Rect (currentX, position.y,
            _CATEGORY_DROPDOWN_WIDTH, position.height), (ObjectDatabase.Category)categoryProperty.enumValueIndex);*/
        categoryProperty.enumValueIndex = (int)(ObjectDatabase.Category)EditorGUI.EnumPopup(
            new Rect(currentX, position.y,
            _categoryDropdownWidth, position.height), (ObjectDatabase.Category)categoryProperty.enumValueIndex);

        currentX += _categoryDropdownWidth;
        //var snapModeDropdownWidth = position.width * SNAPMODE_DROPDOWN_PERCENT;
        _snapModeDropdownWidth = SNAPMODE_DROPDOWN_PARTS * _partWidth;

        var snapModeProperty = property.FindPropertyRelative("snapMode");
        snapModeProperty.enumValueIndex = (int)(ObjectDatabase.SnapMode)EditorGUI.EnumPopup(
            new Rect(currentX, position.y, _snapModeDropdownWidth, position.height), (ObjectDatabase.SnapMode)snapModeProperty.enumValueIndex);

        EditorGUI.EndChangeCheck();
    }

    #endregion
}
#endif
