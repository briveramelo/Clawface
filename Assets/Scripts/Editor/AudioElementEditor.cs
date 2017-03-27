using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(AudioElement))]
public class AudioElementEditor : Editor {

    AudioElement _target;

    SerializedObject _serializedTarget;
    SerializedProperty _loopProp;
    //SerializedProperty _changeVolumeEachLoopProp;
    SerializedProperty _changePitchEachLoopProp;
    SerializedProperty _layeredProp;
    SerializedProperty _bassChannelProp;
    SerializedProperty _midChannelProp;
    SerializedProperty _trebleChannelProp;
    SerializedProperty _standardChannelProp;
    //SerializedProperty _bassAudioClipsProp;
    //SerializedProperty _midAudioClipsProp;
    //SerializedProperty _trebleAudioClipsProp;
    //SerializedProperty _audioClipsProp;
    //SerializedProperty _randomVolumeProp;
    //SerializedProperty _volumeProp;
    //SerializedProperty _volumeRangeProp;
    SerializedProperty _randomPitchProp;
    SerializedProperty _pitchProp;
    SerializedProperty _pitchRangeProp;

    private void OnEnable() {
        _target = target as AudioElement;
        _serializedTarget = new SerializedObject (_target);
        _loopProp = _serializedTarget.FindProperty("_loop");
        //_changeVolumeEachLoopProp = _serializedTarget.FindProperty("_changeVolumeEachLoop");
        _changePitchEachLoopProp  = _serializedTarget.FindProperty("_changePitchEachLoop");
        _layeredProp              = _serializedTarget.FindProperty("_layered");
        _bassChannelProp          = _serializedTarget.FindProperty("_bassChannel");
        _midChannelProp           = _serializedTarget.FindProperty("_midChannel");
        _trebleChannelProp        = _serializedTarget.FindProperty("_trebleChannel");
        _standardChannelProp      = _serializedTarget.FindProperty("_standardChannel");
        //_bassAudioClipsProp       = _serializedTarget.FindProperty("_bassAudioClips");
        //_midAudioClipsProp        = _serializedTarget.FindProperty("_midAudioClips");
        //_trebleAudioClipsProp     = _serializedTarget.FindProperty("_trebleAudioClips");
        //_audioClipsProp           = _serializedTarget.FindProperty("_audioClips");
        //_randomVolumeProp         = _serializedTarget.FindProperty("_randomVolume");
        //_volumeProp               = _serializedTarget.FindProperty("_volume");
        //_volumeRangeProp          = _serializedTarget.FindProperty("_volumeRange");
        _randomPitchProp          = _serializedTarget.FindProperty("_randomPitch");
        _pitchProp                = _serializedTarget.FindProperty("_pitch");
        _pitchRangeProp           = _serializedTarget.FindProperty("_pitchRange");
    }

    public override void OnInspectorGUI() {

        _serializedTarget.Update();

        if (GUILayout.Button ("Play Sound"))
            _target.Play();

        _loopProp.boolValue = EditorGUILayout.Toggle ("Looped", _loopProp.boolValue);
        EditorGUILayout.Space();
        _layeredProp.boolValue = EditorGUILayout.Toggle ("Layered", _layeredProp.boolValue);
        //EditorGUILayout.Space();

        EditorGUI.indentLevel++;
        if (_layeredProp.boolValue) {
            //DrawList (_bassAudioClipsProp);
            //DrawList (_midAudioClipsProp);
            //DrawList (_trebleAudioClipsProp);
            EditorGUILayout.PropertyField (_bassChannelProp);
            EditorGUILayout.PropertyField (_midChannelProp);
            EditorGUILayout.PropertyField (_trebleChannelProp);
        } else {
            //DrawList (_audioClipsProp);
            EditorGUILayout.PropertyField (_standardChannelProp);
        }
        EditorGUI.indentLevel--;
        EditorGUILayout.Space();

        //_randomVolumeProp.boolValue = EditorGUILayout.Toggle ("Random Volume", _randomVolumeProp.boolValue);
        //EditorGUI.BeginDisabledGroup (!_loopProp.boolValue || !_randomVolumeProp.boolValue);
        //_changeVolumeEachLoopProp.boolValue = EditorGUILayout.Toggle ("Change volume each loop", _changeVolumeEachLoopProp.boolValue);
        //EditorGUI.EndDisabledGroup();

        //EditorGUI.indentLevel++;
        //if (_randomVolumeProp.boolValue) {
        //    EditorGUILayout.PropertyField (_volumeRangeProp, new GUIContent("Random Volume"));
        //} else {
        //    _volumeProp.floatValue = EditorGUILayout.FloatField ("Volume", _volumeProp.floatValue);
        //}
        //EditorGUI.indentLevel--;
        //EditorGUILayout.Space();

        _randomPitchProp.boolValue = EditorGUILayout.Toggle ("Random Pitch", _randomPitchProp.boolValue);
        EditorGUI.BeginDisabledGroup (!_loopProp.boolValue || !_randomPitchProp.boolValue);
        _changePitchEachLoopProp.boolValue = EditorGUILayout.Toggle ("Change pitch each loop", _changePitchEachLoopProp.boolValue);
        EditorGUI.EndDisabledGroup();

        EditorGUI.indentLevel++;
        if (_randomPitchProp.boolValue) {
            EditorGUILayout.PropertyField (_pitchRangeProp, new GUIContent("Random Pitch"));
        } else {
            _pitchProp.floatValue = EditorGUILayout.FloatField ("Pitch", _pitchProp.floatValue);
        }
        EditorGUI.indentLevel--;

        _serializedTarget.ApplyModifiedProperties();
    }
}
