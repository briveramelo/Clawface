using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer (typeof(CameraTrack.PositionInfo))]
public class CameraTrackPositionDrawer : PropertyDrawer {

    const float TRANSFORM_LABEL_WIDTH = 44f;
    const float TRANSFORM_FIELD_WIDTH = 128f;
    const float FOV_LABEL_WIDTH = 44f;
    const float FOV_FIELD_WIDTH = 40f;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        //base.OnGUI(position, property, label);
        EditorGUI.BeginProperty (position, label, property);

        var transformLabelRect = new Rect (position.x, position.y, TRANSFORM_LABEL_WIDTH, position.height);
        var transformFieldRect = new Rect (position.x + TRANSFORM_LABEL_WIDTH, position.y, TRANSFORM_FIELD_WIDTH, position.height);
        var fovLabelRect = new Rect (transformFieldRect.x + TRANSFORM_FIELD_WIDTH, position.y, FOV_LABEL_WIDTH, position.height);
        var fovFieldRect = new Rect (fovLabelRect.x + FOV_LABEL_WIDTH, position.y, FOV_FIELD_WIDTH, position.height);
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
