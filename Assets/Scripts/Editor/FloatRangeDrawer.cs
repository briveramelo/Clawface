using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(FloatRangeAttribute))]
public class FloatRangeDrawer : PropertyDrawer {

    const float _X_DIV_VALUE = 0.33f;
    const float _Y_DIV_VALUE = 0.5f;

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
        return base.GetPropertyHeight(property, label) + 16f;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        var attrib = attribute as FloatRangeAttribute;
        var min = property.FindPropertyRelative ("_min");
        var minValue = min.floatValue;
        var max = property.FindPropertyRelative ("_max");
        var maxValue = max.floatValue;

        var xDiv = position.width * _X_DIV_VALUE;
        var yDiv = position.height * _Y_DIV_VALUE;

        EditorGUI.LabelField (new Rect (position.x, position.y, xDiv, yDiv), label);

        EditorGUI.LabelField (new Rect (position.x, position.y + yDiv, position.width, yDiv), min.floatValue.ToString("0.#"));
        EditorGUI.LabelField (new Rect (position.x + position.width - 40f, position.y + yDiv, position.width, yDiv), max.floatValue.ToString("0.#"));
        EditorGUI.MinMaxSlider (new Rect (position.x + 24f, position.y + yDiv, position.width - 56f, yDiv), ref minValue, ref maxValue, attrib.min, attrib.max);

        EditorGUI.LabelField(new Rect (position.x + xDiv * 2f, position.y, xDiv, yDiv), "To:");
        minValue = Mathf.Clamp( EditorGUI.FloatField(new Rect(position.x + xDiv + 30f, position.y, xDiv - 30f, yDiv), minValue), attrib.min, maxValue );
        EditorGUI.LabelField(new Rect(position.x + xDiv * 2f, position.y, xDiv, yDiv), "To: " );
        maxValue = Mathf.Clamp(EditorGUI.FloatField(new Rect(position.x + xDiv * 2f + 24f, position.y, xDiv - 24f, yDiv), maxValue), minValue, attrib.max);

        min.floatValue = minValue;
        max.floatValue = maxValue;
    }
}
