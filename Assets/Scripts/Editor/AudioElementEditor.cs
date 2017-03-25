using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(AudioElement))]
public class AudioElementEditor : Editor {

    AudioElement _target;

    SerializedObject _serializedTarget;
    SerializedProperty _loopProp;
    SerializedProperty _layeredProp;
    SerializedProperty _bassAudioClipsProp;
    SerializedProperty _midAudioClipsProp;
    SerializedProperty _trebleAudioClipsProp;
    SerializedProperty _audioClipsProp;
    SerializedProperty _randomVolumeProp;
    SerializedProperty _volumeProp;
    SerializedProperty _volumeRangeProp;
    SerializedProperty _randomPitchProp;
    SerializedProperty _pitchProp;
    SerializedProperty _pitchRangeProp;

    private void OnEnable() {
        _target = target as AudioElement;
        _serializedTarget = new SerializedObject (_target);
        _loopProp = _serializedTarget.FindProperty("_loop");
        _layeredProp = _serializedTarget.FindProperty ("_layered");
        _bassAudioClipsProp = _serializedTarget.FindProperty("_bassAudioClips");
        _midAudioClipsProp = _serializedTarget.FindProperty("_midAudioClips");
        _trebleAudioClipsProp = _serializedTarget.FindProperty("_trebleAudioClips");
        _audioClipsProp = _serializedTarget.FindProperty("_audioClips");
        _randomVolumeProp = _serializedTarget.FindProperty("_randomVolume");
        _volumeProp = _serializedTarget.FindProperty("_volume");
        _volumeRangeProp = _serializedTarget.FindProperty("_volumeRange");
        _randomPitchProp = _serializedTarget.FindProperty("_randomPitch");
        _pitchProp = _serializedTarget.FindProperty("_pitch");
        _pitchRangeProp = _serializedTarget.FindProperty("_pitchRange");
    }

    public override void OnInspectorGUI() {

        _loopProp.boolValue = EditorGUILayout.Toggle("Loop", _loopProp.boolValue);
 
        _layeredProp.boolValue = EditorGUILayout.Toggle ("Layered", _layeredProp.boolValue);
        EditorGUILayout.Space();

        EditorGUI.indentLevel++;
        if (_layeredProp.boolValue) {
            DrawList (_bassAudioClipsProp);
            DrawList (_midAudioClipsProp);
            DrawList (_trebleAudioClipsProp);
        } else {
            DrawList (_audioClipsProp);
        }
        EditorGUI.indentLevel--;
        EditorGUILayout.Space();

        _randomVolumeProp.boolValue = EditorGUILayout.Toggle ("Random Volume", _randomVolumeProp.boolValue);
        EditorGUI.indentLevel++;
        if (_randomVolumeProp.boolValue) {
            EditorGUILayout.PropertyField (_volumeRangeProp, new GUIContent("Random Volume"));
        } else {
            _volumeProp.floatValue = EditorGUILayout.FloatField ("Volume", _volumeProp.floatValue);
        }
        EditorGUI.indentLevel--;
        EditorGUILayout.Space();

        _randomPitchProp.boolValue = EditorGUILayout.Toggle ("Random Pitch", _randomPitchProp.boolValue);
        EditorGUI.indentLevel++;
        if (_randomPitchProp.boolValue) {
            EditorGUILayout.PropertyField (_pitchRangeProp, new GUIContent("Random Pitch"));
        } else {
            _pitchProp.floatValue = EditorGUILayout.FloatField ("Pitch", _pitchProp.floatValue);
        }
        EditorGUI.indentLevel--;
    }

    void DrawList (SerializedProperty listProp) {
        if (EditorGUILayout.PropertyField (listProp)) {
            EditorGUI.indentLevel++;
            for (int i = 0; i < listProp.arraySize; i++) {
                var element = listProp.GetArrayElementAtIndex(i);
                if (element == null) continue;
                EditorGUILayout.PropertyField (element);
            }
            EditorGUI.indentLevel--;
        }
    }
}
