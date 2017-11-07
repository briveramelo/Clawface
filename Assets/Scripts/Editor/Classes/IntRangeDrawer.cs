// IntRangeDrawer.cs
// Author: Brandon, Aaron

using UnityEditor;

using UnityEngine;

/// <summary>
/// Custom PropertyDrawer for IntRange attributes.
/// </summary>
[CustomPropertyDrawer(typeof(IntRangeAttribute))]
public class IntRangeDrawer : PropertyDrawer
{
    #region Private Fields

    const float _X_DIV_VALUE = 0.33f;
    const float _Y_DIV_VALUE = 0.5f;
    protected float attribMin, attribMax;

    #endregion
    #region Pubilc Methods

    public override float GetPropertyHeight(SerializedProperty property, 
        GUIContent label)
    {
        return base.GetPropertyHeight(property, label) + 16f;
    }

    public override void OnGUI(Rect position, SerializedProperty property, 
        GUIContent label)
    {
        var min = property.FindPropertyRelative("min");
        float minValue = min.intValue;
        var max = property.FindPropertyRelative("max");
        float maxValue = max.intValue;

        var xDiv = position.width * _X_DIV_VALUE;
        var yDiv = position.height * _Y_DIV_VALUE;

        EditorGUI.LabelField(new Rect(position.x, position.y, xDiv, yDiv), label);

        EditorGUI.indentLevel++;
            EditorGUI.LabelField(new Rect(position.x, position.y + yDiv, position.width, yDiv), attribMin.ToString("0"));
            EditorGUI.MinMaxSlider(new Rect(position.x + 24f, position.y + yDiv, position.width - 56f, yDiv), ref minValue, ref maxValue, attribMin, attribMax);
            EditorGUI.LabelField(new Rect(position.x + position.width - 60f, position.y + yDiv, position.width, yDiv), attribMax.ToString("0"));
        EditorGUI.indentLevel--;


        EditorGUI.LabelField(new Rect(position.x + xDiv * 2f, position.y, xDiv, yDiv), "To:");
        minValue = Mathf.Clamp(EditorGUI.IntField(new Rect(position.x + xDiv + 30f, position.y, xDiv - 30f, yDiv), (int)minValue), attribMin, maxValue);        
        maxValue = Mathf.Clamp(EditorGUI.IntField(new Rect(position.x + xDiv * 2f + 24f, position.y, xDiv - 30f, yDiv), (int)maxValue), minValue, attribMax);

        min.intValue = Mathf.RoundToInt(minValue);
        max.intValue = Mathf.RoundToInt(maxValue);
    }

    #endregion
}
