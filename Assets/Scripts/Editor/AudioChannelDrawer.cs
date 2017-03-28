// AudioChannelDrawer.cs

using UnityEngine;
using UnityEditor;

/// <summary>
/// Custom property drawer for AudioChannels.
/// </summary>
[CustomPropertyDrawer(typeof(AudioChannel))]
public class AudioChannelDrawer : PropertyDrawer {

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

    SerializedProperty _clipsProp;
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
        _clipsProp                = _serializedChannel.FindProperty("_clips");
        _volumeProp               = _serializedChannel.FindProperty("_volume");
        _randomVolumeProp         = _serializedChannel.FindProperty("_randomVolume");
        _volumeRangeProp          = _serializedChannel.FindProperty("_volumeRange");
        _changeVolumeEachLoopProp = _serializedChannel.FindProperty("_changeVolumeEachLoop");

        GUILayout.BeginVertical (EditorStyles.helpBox);
        if (_expanded = EditorGUILayout.Foldout (_expanded, "Channel: " + _targetChannel.name, true)) {
            
            EditorGUILayout.Space();
            EditorGUILayout.LabelField ("Clips");
            EditorGUI.indentLevel++;

            var clips = _targetChannel.Clips;

            for (int i = 0; i < _targetChannel.Clips.Count; i++) {
                var clip = clips[i];
                var changedClip = EditorGUILayout.ObjectField (clip, typeof(AudioClip), false) as AudioClip;
                if (changedClip == null) clips.RemoveAt(i);
                else _targetChannel.Clips[i] = changedClip;
            }
            var newClip = EditorGUILayout.ObjectField(null, typeof(AudioClip), false) as AudioClip;
            if (newClip != null) clips.Add (newClip);

            EditorGUI.indentLevel--;

            EditorGUILayout.Space();

            _targetChannel.UseRandomVolume = EditorGUILayout.Toggle ("Random Volume", _targetChannel.UseRandomVolume);
            if (_targetChannel.UseRandomVolume) {
                EditorGUILayout.PropertyField (_volumeRangeProp);
                //EditorGUI.BeginDisabledGroup (
                _targetChannel.ChangeVolumeEachLoop = EditorGUILayout.Toggle ("Change volume each loop", _targetChannel.ChangeVolumeEachLoop);
            } else 
                //_targetChannel.Volume = EditorGUILayout.FloatField ("Volume", _targetChannel.Volume);
                EditorGUILayout.PropertyField (_volumeProp);

            _serializedChannel.ApplyModifiedProperties();

            //GUI.contentColor = backup;

            
        }
        EditorGUILayout.EndVertical();
    }

    #endregion
}
