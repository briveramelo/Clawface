using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer (typeof(CameraTrack.PositionInfo))]
public class CameraTrackPositionDrawer : PropertyDrawer {

    const float _PROPERTY_WIDTH = 256f;
    const float _TRANSFORM_LABEL_WIDTH = 44f;
    ////const float TRANSFORM_FIELD_WIDTH = 64f;
    const float _FOV_LABEL_WIDTH = 44f;
    const float _FOV_FIELD_WIDTH = 40f;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        EditorGUI.BeginProperty (position, label, property);
        
        var x = position.x;
        var transformLabelRect = new Rect (x, position.y, _TRANSFORM_LABEL_WIDTH, position.height);
        var w = position.width - _TRANSFORM_LABEL_WIDTH - _FOV_FIELD_WIDTH - _FOV_LABEL_WIDTH;
        var transformFieldRect = new Rect (x += _TRANSFORM_LABEL_WIDTH, position.y, w, position.height);
        var fovLabelRect = new Rect (x += w, position.y, _FOV_LABEL_WIDTH, position.height);
        var fovFieldRect = new Rect (x += _FOV_LABEL_WIDTH, position.y, _FOV_FIELD_WIDTH, position.height);
        EditorGUI.LabelField (transformLabelRect, "POS");
        EditorGUI.PropertyField (transformFieldRect, property.FindPropertyRelative ("transform"), GUIContent.none);
        EditorGUI.LabelField (fovLabelRect, "FOV");
        EditorGUI.PropertyField (fovFieldRect, property.FindPropertyRelative ("fov"), GUIContent.none);

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
        return base.GetPropertyHeight(property, label);
    }
}
