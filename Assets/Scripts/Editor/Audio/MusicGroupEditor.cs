using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Turing.Audio;

[CustomEditor(typeof(MusicGroup))]
public class MusicGroupEditor : Editor {

    const float _REMOVE_INSTRUMENT_BUTTON_WIDTH = 20f;

    MusicGroup _target;

    SerializedObject _serializedTarget;
    SerializedProperty _playOnAwakeProp;
    SerializedProperty _bpmProp;
    SerializedProperty _instrumentChannelsProp;

    void OnEnable() {
        _target = target as MusicGroup;
        _serializedTarget = new SerializedObject(_target);
        _playOnAwakeProp = _serializedTarget.FindProperty("_playOnAwake");
        _bpmProp = _serializedTarget.FindProperty("_bpm");
        _instrumentChannelsProp = _serializedTarget.FindProperty("_instrumentChannels");
    }

    public override void OnInspectorGUI() {
        // Update serialized object
        _serializedTarget.Update();

        EditorGUILayout.Space();

        // Button group
        EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);

        // Show play sound button
        if (GUILayout.Button("Play Track", GUILayout.Width(96f)))
            _target.Play();

        // Show stop sound button
        if (GUILayout.Button("Stop Track", GUILayout.Width(96f)))
            _target.Stop();

        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();

        EditorGUILayout.BeginVertical(EditorStyles.helpBox);

        // Global settings header
        EditorGUILayout.LabelField("Global Settings", EditorStyles.boldLabel);

        if (Application.isPlaying) {
            EditorGUILayout.LabelField(_target.IsPlaying ? "Playing" : "Stopped");
            EditorGUILayout.LabelField("Current volume scale: " + _target.VolumeScale);
        } else {
            EditorGUILayout.PropertyField(_playOnAwakeProp);
            EditorGUILayout.PropertyField(_bpmProp);
            EditorGUILayout.TextArea("Looping functionality is not available in the editor. Test it in play mode.", EditorStyles.helpBox);
        }

        EditorGUILayout.EndVertical();
        EditorGUILayout.Space();

        // Channels
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.LabelField("Channels", EditorStyles.boldLabel);
        EditorGUI.indentLevel++;

        int instrumentCount = _instrumentChannelsProp.arraySize;
        for (int i = 0; i < instrumentCount; i++) {
            // Check if instruments have been added
            if (_instrumentChannelsProp.arraySize != instrumentCount)
                break;

            // Draw remove instrument button
            var instrument = _instrumentChannelsProp.GetArrayElementAtIndex(i);
            EditorGUILayout.PropertyField(instrument);
            var lastRect = GUILayoutUtility.GetLastRect();
            lastRect.x = lastRect.x + lastRect.width - _REMOVE_INSTRUMENT_BUTTON_WIDTH;
            lastRect.width = _REMOVE_INSTRUMENT_BUTTON_WIDTH;
            lastRect.height = _REMOVE_INSTRUMENT_BUTTON_WIDTH;
            if (GUI.Button(lastRect, "x", EditorStyles.toolbarButton)) {

                // KEEP BOTH OF THESE FUNCTIONS
                // First one: sets array element to null
                // Second one: actually removes it
                if (instrument.objectReferenceValue != null)
                    _instrumentChannelsProp.DeleteArrayElementAtIndex(i);
                _instrumentChannelsProp.DeleteArrayElementAtIndex(i);
                break;
            }
        }

        // Draw add new element channel button
        if (GUILayout.Button("Add new element channel")) {
            _target.AddInstrumentChannel();
            _serializedTarget.Update();
        }
        EditorGUI.indentLevel--;
        EditorGUILayout.EndVertical();
        EditorGUILayout.Space();

        // Apply modified properties
        _serializedTarget.ApplyModifiedProperties();

    }

    [MenuItem("Audio/Create music group")]
    static void CreateMusicGroup() {
        new GameObject("New music group", typeof(MusicGroup));
    }
}
