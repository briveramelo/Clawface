using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Turing.Audio;

[CustomPropertyDrawer(typeof(MusicChannel))]
public class MusicChannelDrawer : PropertyDrawer {

    bool _expanded = false;

    MusicChannel _targetChannel;

    SerializedObject _serializedChannel;

    SerializedProperty _volumeProp;
    SerializedProperty _clipsProp;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {

        // Acquire target channel if necessary
        if (_targetChannel == null)
            _targetChannel = property.objectReferenceValue as MusicChannel;

        // If channel is still null, channel was deleted
        if (_targetChannel == null) return;

        // Get serialized reference to target channel if necessary
        if (_serializedChannel == null)
            _serializedChannel = new SerializedObject(property.objectReferenceValue);

        _serializedChannel.Update();

        // Get serialized properties
        _volumeProp = _serializedChannel.FindProperty("_volume");

        // Clips box
        GUILayout.BeginVertical(EditorStyles.helpBox);
        if (_expanded = EditorGUILayout.Foldout(_expanded, "Channel: " + _targetChannel.name, true)) {

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Clips");
            EditorGUI.indentLevel++;

            _clipsProp = _serializedChannel.FindProperty ("_clips");

            for (int i = 0; i < _clipsProp.arraySize; i++) {
                var clip = _clipsProp.GetArrayElementAtIndex(i);
                var clipProp = clip.FindPropertyRelative("_clip");
                var barsProp = clip.FindPropertyRelative("_bars");
                EditorGUILayout.BeginHorizontal();
                //var changedClip = EditorGUILayout.ObjectField(clip.Clip, typeof(AudioClip), false) as AudioClip;
                EditorGUILayout.PropertyField(clipProp);
                if (clipProp.objectReferenceValue == null) _targetChannel.Clips.RemoveAt(i);
                //else _targetChannel.Clips[i].Clip = changedClip;
                //clip.Bars = EditorGUILayout.IntField (clip.Bars);
                EditorGUILayout.PropertyField (barsProp);
                EditorGUILayout.EndHorizontal();
            }
            var newClip = EditorGUILayout.ObjectField(null, typeof(AudioClip), false) as AudioClip;
            if (newClip != null) _targetChannel.AddClip(newClip);

            EditorGUI.indentLevel--;

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(_volumeProp);
        }
        EditorGUILayout.EndVertical();

        _serializedChannel.ApplyModifiedProperties();
    }
}
