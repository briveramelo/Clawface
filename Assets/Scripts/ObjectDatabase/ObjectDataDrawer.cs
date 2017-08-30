// ObjectDataDrawer.cs
// Author: Aaron

using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

namespace Turing.LevelEditor
{
    /// <summary>
    /// Custom proerty drawer for ObjectData class.
    /// </summary>
    [CustomPropertyDrawer(typeof(ObjectData))]
    public class ObjectDataDrawer : PropertyDrawer
    {

        #region Constants

        const int INDEX_LABEL_WIDTH = 64;
        const int PATH_LABEL_WIDTH = 256;
        const int PREFAB_FIELD_WIDTH = 128;
        const int LIMIT_FIELD_WIDTH = 128;
        const int CATEGORY_DROPDOWN_WIDTH = 128;
        const int SNAPMODE_DROPDOWN_WIDTH = 128;

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

        #endregion
        #region Vars

        float partWidth;
        float indexLabelWidth = 0f;
        float prefabFieldWidth = 0f;
        float limitFieldWidth = 0f;
        float categoryDropdownWidth = 0f;
        float snapModeDropdownWidth = 0f;

        #endregion
        #region Overrides

        public override void OnGUI(Rect position, SerializedProperty property,
            GUIContent label) 
        {
            // Calculate the width of a part
            partWidth = position.width / (INDEX_LABEL_PARTS + 
                PREFAB_FIELD_PARTS + LIMIT_FIELD_PARTS + 
                CATEGORY_DROPDOWN_PARTS + SNAPMODE_DROPDOWN_PARTS);

            EditorGUI.BeginChangeCheck();

            var currentX = position.x;
            
            // Index property
            indexLabelWidth = INDEX_LABEL_PARTS * partWidth;
            var indexProperty = property.FindPropertyRelative("index");
            var value = indexProperty.intValue;
            if (value == (int)byte.MaxValue) 
            {
                EditorGUI.LabelField(new Rect(currentX, position.y, 
                    position.width, position.height),
                    "***Index 255 is reserved for empty tiles!***");
                return;
            }

            string index = value.ToString();
            EditorGUI.LabelField(new Rect(currentX, position.y, 
                indexLabelWidth, position.height), index);

            currentX += indexLabelWidth;

            var pathProperty = property.FindPropertyRelative("path");

            // Prefab property
            prefabFieldWidth = PREFAB_FIELD_PARTS * partWidth;
            var prefabProperty = property.FindPropertyRelative("prefab");
            GameObject currentPrefab = 
                (GameObject)prefabProperty.objectReferenceValue;
            GameObject pickedPrefab = (GameObject)EditorGUI.ObjectField(
                new Rect(currentX, position.y, prefabFieldWidth, 
                position.height), currentPrefab, typeof(GameObject), false);
            if (currentPrefab != pickedPrefab) 
            {
                prefabProperty.objectReferenceValue = pickedPrefab;
                if (pickedPrefab == null) pathProperty.stringValue = "UNUSED";
                else pathProperty.stringValue = AssetDatabase.GetAssetPath(pickedPrefab);
            }

            currentX += prefabFieldWidth;
            limitFieldWidth = LIMIT_FIELD_PARTS * partWidth;

            // Limit property
            var limitProperty = property.FindPropertyRelative("limit");
            limitProperty.intValue = EditorGUI.IntField(new Rect(currentX, 
                position.y, limitFieldWidth, position.height), 
                limitProperty.intValue);

            currentX += limitFieldWidth;
            categoryDropdownWidth = CATEGORY_DROPDOWN_PARTS * partWidth;

            // Category property
            var categoryProperty = property.FindPropertyRelative("category");
            categoryProperty.enumValueIndex = 
                (int)(ObjectDatabase.Category)EditorGUI.EnumPopup(
                new Rect(currentX, position.y, categoryDropdownWidth, 
                position.height), 
                (ObjectDatabase.Category)categoryProperty.enumValueIndex);

            currentX += categoryDropdownWidth;
            snapModeDropdownWidth = SNAPMODE_DROPDOWN_PARTS * partWidth;

            // Snap mode property
            var snapModeProperty = property.FindPropertyRelative("snapMode");
            snapModeProperty.enumValueIndex = 
                (int)(ObjectDatabase.SnapMode)EditorGUI.EnumPopup(
                new Rect(currentX, position.y, snapModeDropdownWidth, 
                position.height), 
                (ObjectDatabase.SnapMode)snapModeProperty.enumValueIndex);

            EditorGUI.EndChangeCheck();
        }

        #endregion
    }
#endif
}