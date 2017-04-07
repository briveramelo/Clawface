using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Events;
using ModMan;

[CustomPropertyDrawer(typeof(CameraTrack.CameraEvent))]
public class CameraEventDrawer : PropertyDrawer {

    CameraTrack.CameraEvent _event;

    const float _TIME_LABEL_WIDTH = 48f;
    const float _TIME_FIELD_HEIGHT = 16f;
    const float _TIME_FIELD_WIDTH = 64f;
    const float _PADDING = 4f;
    const float _EVENT_FIELD_WIDTH = 128f;
    const float _EVENT_FIELD_HEIGHT_BASE = 84f;
    const float _EVENT_FIELD_EVENT_HEIGHT = 44f;
    public const float REMOVE_EVENT_BUTTON_WIDTH = 16f;

    int _listeners = 0;

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
        return _EVENT_FIELD_HEIGHT_BASE + (_listeners > 0 ? _listeners - 1 : _listeners) * _EVENT_FIELD_EVENT_HEIGHT;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        var time = property.FindPropertyRelative ("time");
        var e = property.FindPropertyRelative ("onTrigger");
        _listeners = e.FindPropertyRelative("m_PersistentCalls.m_Calls").arraySize;

        var x = position.x;

        //float w = position.width * _TIME_FIELD_PERCENTAGE;
        EditorGUI.LabelField (new Rect (x, position.y, _TIME_LABEL_WIDTH, _TIME_FIELD_HEIGHT), "Time");
        EditorGUI.PropertyField (new Rect (position.x += _TIME_LABEL_WIDTH, position.y, _TIME_FIELD_WIDTH, _TIME_FIELD_HEIGHT), time, GUIContent.none);
        EditorGUI.PropertyField (new Rect (position.x += _TIME_FIELD_WIDTH + _PADDING, position.y, position.width - _TIME_FIELD_WIDTH - _TIME_LABEL_WIDTH - _PADDING, position.height), e);

        //base.OnGUI(position, property, label);
    }
}
