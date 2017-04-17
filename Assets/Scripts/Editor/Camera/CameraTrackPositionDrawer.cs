using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer (typeof(CameraTrack.PositionInfo))]
public class CameraTrackPositionDrawer : PropertyDrawer {

    const float _PROPERTY_WIDTH = 256f;
    //const float _TRANSFORM_LABEL_WIDTH = 44f;
    ////const float TRANSFORM_FIELD_WIDTH = 64f;
    const float _FOV_LABEL_WIDTH = 44f;
    const float _FOV_FIELD_WIDTH = 40f;
    const float _FIELD_HEIGHT = 16f;
    const float _EVENT_FIELD_WIDTH = 64f;
    const float _EVENT_FIELD_HEIGHT_BASE = 84f;
    const float _EVENT_FIELD_EVENT_HEIGHT = 44f;
    const float _PADDING = 4f;

    int _listeners = 0;

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
        _listeners = property.FindPropertyRelative("onReachPosition").FindPropertyRelative("m_PersistentCalls.m_Calls").arraySize;
        return _EVENT_FIELD_HEIGHT_BASE + (_listeners > 0 ? _listeners - 1 : _listeners) * _EVENT_FIELD_EVENT_HEIGHT;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        var fovProperty = property.FindPropertyRelative ("fov");
        var eventProperty = property.FindPropertyRelative ("onReachPosition");

        var x = position.x;
        //var transformLabelRect = new Rect (x, position.y, _TRANSFORM_LABEL_WIDTH, position.height);
        //var w = position.width - _TRANSFORM_LABEL_WIDTH - _FOV_FIELD_WIDTH - _FOV_LABEL_WIDTH;
        var w = position.width - _FOV_FIELD_WIDTH - _FOV_LABEL_WIDTH - _PADDING;
        //var transformFieldRect = new Rect (x += _TRANSFORM_LABEL_WIDTH, position.y, w, position.height);
        var fovLabelRect = new Rect (x, position.y, _FOV_LABEL_WIDTH, _FIELD_HEIGHT);
        var fovFieldRect = new Rect (x += _FOV_LABEL_WIDTH, position.y, _FOV_FIELD_WIDTH, _FIELD_HEIGHT);
        //EditorGUI.LabelField (transformLabelRect, "POS");
        //EditorGUI.PropertyField (transformFieldRect, property.FindPropertyRelative ("transform"), GUIContent.none);
        EditorGUI.LabelField (fovLabelRect, "FOV");
        EditorGUI.PropertyField (fovFieldRect, fovProperty, GUIContent.none);
        var eventFieldRect = new Rect (x += _FOV_FIELD_WIDTH + _PADDING, position.y, w, position.height);
        EditorGUI.PropertyField (eventFieldRect, eventProperty);
    }
}
