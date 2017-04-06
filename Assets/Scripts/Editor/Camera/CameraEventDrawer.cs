using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using ModMan;

[CustomPropertyDrawer(typeof(CameraTrack.CameraEvent))]
public class CameraEventDrawer : PropertyDrawer {

    CameraTrack.CameraEvent _event;

    const float _TIME_FIELD_PERCENTAGE = 0.5f;

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
        return 128f;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        var time = property.FindPropertyRelative ("time");
        var e = property.FindPropertyRelative ("onTrigger");

        float w = position.width * _TIME_FIELD_PERCENTAGE;

        EditorGUI.PropertyField (new Rect (position.x, position.y, w, position.height), time);
        EditorGUI.PropertyField (new Rect (position.x + w, position.y, position.width - w, position.height), e);

        //base.OnGUI(position, property, label);
    }
}
