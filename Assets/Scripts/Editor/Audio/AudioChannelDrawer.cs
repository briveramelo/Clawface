// AudioChannelDrawer.cs

using UnityEngine;
using UnityEditor;
using Turing.Audio;

/// <summary>
/// Custom property drawer for AudioChannels.
/// </summary>
[CustomPropertyDrawer(typeof(AudioChannel))]
public sealed class AudioChannelDrawer : PropertyDrawer {

    #region Vars

    /// <summary>
    /// Is the clip list expanded?
    /// </summary>
    bool _expanded = false;

    /// <summary>
    /// Target AudioElementChannel.
    /// </summary>
    AudioChannel _targetChannel;

    /// <summary>
    /// Serialized version of target AudioElement Channel.
    /// Needed for finding SerializedProperties.
    /// </summary>
    SerializedObject _serializedChannel;

    SerializedProperty _volumeProp;
    SerializedProperty _randomVolumeProp;
    SerializedProperty _volumeRangeProp;
    SerializedProperty _changeVolumeEachLoopProp;

    #endregion
    #region Unity Overrides

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
        return base.GetPropertyHeight(property, label) - 16f;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {

        // Acquire target channel if necessary
        if (_targetChannel == null)
            _targetChannel = property.objectReferenceValue as AudioChannel;

        // If channel is still null, channel was deleted
        if (_targetChannel == null) return;

        // Get serialized reference to target channel if necessary
        if (_serializedChannel == null)
            _serializedChannel = new SerializedObject(property.objectReferenceValue);

        _serializedChannel.Update();

        // Get serialized properties
        _volumeProp = _serializedChannel.FindProperty("_uniformVolume");
        _randomVolumeProp = _serializedChannel.FindProperty("_useRandomVolume");
        _volumeRangeProp = _serializedChannel.FindProperty("_randomVolumeRange");
        _changeVolumeEachLoopProp = _serializedChannel.FindProperty("_changeVolumeEachLoop");

        // Clips box
        GUILayout.BeginVertical(EditorStyles.helpBox);
        if (_expanded = EditorGUILayout.Foldout(_expanded, "Channel: " + _targetChannel.name, true)) {

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Clips");
            EditorGUI.indentLevel++;

            var clips = _targetChannel.Clips;

            for (int i = 0; i < _targetChannel.Clips.Count; i++) {
                var clip = _targetChannel.Clips[i];
                var changedClip = EditorGUILayout.ObjectField(clip, typeof(AudioClip), false) as AudioClip;
                if (changedClip == null) _targetChannel.Clips.RemoveAt(i);
                else _targetChannel.Clips[i] = changedClip;
            }
            var newClip = EditorGUILayout.ObjectField(null, typeof(AudioClip), false) as AudioClip;
            if (newClip != null) _targetChannel.AddClip(newClip);

            EditorGUI.indentLevel--;

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(_randomVolumeProp);
            if (_targetChannel.UseRandomVolume)
                EditorGUILayout.PropertyField(_volumeRangeProp);
            else
                EditorGUILayout.PropertyField(_volumeProp);

            EditorGUI.BeginDisabledGroup(!_targetChannel.Parent.Loop || !_randomVolumeProp.boolValue);
            EditorGUILayout.PropertyField(_changeVolumeEachLoopProp);
            EditorGUI.EndDisabledGroup();
        }

        if (GUILayout.Button ("Preview")) _targetChannel.PlaySound(1f, false);
        EditorGUILayout.EndVertical();



        _serializedChannel.ApplyModifiedProperties();
    }

    #endregion
}
