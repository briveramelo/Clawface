// IntRangeDrawer.cs
// Author: Aaron

using UnityEditor;

using UnityEngine;

/// <summary>
/// Custom PropertyDrawer for FloatRange.
/// </summary>
[CustomPropertyDrawer(typeof(IntRangeAttribute))]
public sealed class IntRangeDrawer : PropertyDrawer
{
    #region Private Fields

    const float X_DIV_VALUE = 0.33f;
    const float Y_DIV_VALUE = 0.5f;

    #endregion
    #region Unity Lifecycle

    public override void OnGUI(Rect position, SerializedProperty property, 
        GUIContent label)
    {
        var attrib = attribute as IntRangeAttribute;
        var min = property.FindPropertyRelative ("min");
        var minValue = min.intValue;
        var max = property.FindPropertyRelative ("max");
        var maxValue = max.intValue;

        var minFloat = (float)minValue;
        var maxFloat = (float)maxValue;

        var xDiv = position.width * X_DIV_VALUE;
        var yDiv = position.height * Y_DIV_VALUE;

        EditorGUI.LabelField (new Rect (position.x, position.y, xDiv, yDiv), label);

        EditorGUI.LabelField (new Rect (position.x, position.y + yDiv, position.width, yDiv), min.intValue.ToString());
        EditorGUI.LabelField (new Rect (position.x + position.width - 40f, position.y + yDiv, position.width, yDiv), max.intValue.ToString());
        EditorGUI.MinMaxSlider (new Rect (position.x + 24f, position.y + yDiv, position.width - 56f, yDiv), ref minFloat, ref maxFloat, attrib.min, attrib.max);

        EditorGUI.LabelField(new Rect (position.x + xDiv * 2f, position.y, xDiv, yDiv), "To:");
        minValue = (int)Mathf.Clamp( EditorGUI.IntField(new Rect(position.x + xDiv + 30f, position.y, xDiv - 30f, yDiv), minValue), attrib.min, maxValue );
        EditorGUI.LabelField(new Rect(position.x + xDiv * 2f, position.y, xDiv, yDiv), "To: " );
        maxValue = (int)Mathf.Clamp(EditorGUI.IntField(new Rect(position.x + xDiv * 2f + 24f, position.y, xDiv - 24f, yDiv), maxValue), minValue, attrib.max);

        min.intValue = minValue;
        max.intValue = maxValue;
    }

    #endregion
    #region Public Methods

    public override float GetPropertyHeight(SerializedProperty property, 
        GUIContent label)
    {
        return base.GetPropertyHeight(property, label) + 16f;
    }

    #endregion
}
