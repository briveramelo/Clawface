using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer (typeof(CameraTrack.PositionInfo))]
public class CameraTrackPositionDrawer : PropertyDrawer {

    const float _PROPERTY_WIDTH = 256f;
    const float TRANSFORM_LABEL_WIDTH = 32f;
    const float TRANSFORM_FIELD_WIDTH = 64f;
    const float FOV_LABEL_WIDTH = 44f;
    const float FOV_FIELD_WIDTH = 40f;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        //base.OnGUI(position, property, label);
        EditorGUI.BeginProperty (position, label, property);

        /*EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth (256f), GUILayout.ExpandWidth (false));
        EditorGUILayout.PropertyField (property.FindPropertyRelative ("transform"), GUILayout.MinWidth (TRANSFORM_FIELD_WIDTH));
        EditorGUILayout.PropertyField (property.FindPropertyRelative("fov"), GUILayout.MinWidth (FOV_FIELD_WIDTH));

        EditorGUILayout.EndHorizontal();*/
        
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
