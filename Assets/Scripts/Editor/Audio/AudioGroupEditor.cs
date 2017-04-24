// AudioGroupEditor.cs

using UnityEngine;
using UnityEditor;

namespace Turing.Audio {

    /// <summary>
    /// Custom editor for AudioGroups.
    /// </summary>
    [CustomEditor(typeof(AudioGroup))]
    public sealed class AudioGroupEditor : Editor {

        #region Consts

        const float _REMOVE_CHANNEL_BUTTON_WIDTH = 20f;

        #endregion
        #region Vars

        /// <summary>
        /// Target AudioGroup being edited.
        /// </summary>
        AudioGroup _target;

        #endregion
        #region Serialized Properties

        SerializedObject _serializedTarget;
        SerializedProperty _groupTypeProp;
        SerializedProperty _loopProp;
        SerializedProperty _playOnAwakeProp;
        SerializedProperty _spatialBlendProp;
        SerializedProperty _minDistanceProp;
        SerializedProperty _maxDistanceProp;
        SerializedProperty _changePitchEachLoopProp;
        SerializedProperty _bassChannelProp;
        SerializedProperty _midChannelProp;
        SerializedProperty _trebleChannelProp;
        SerializedProperty _standardChannelProp;
        SerializedProperty _elementChannelsProp;
        SerializedProperty _useVolumeEnvelopeProp;
        SerializedProperty _volumeEnvelopeProp;
        SerializedProperty _randomPitchProp;
        SerializedProperty _pitchProp;
        SerializedProperty _pitchRangeProp;

        #endregion
        #region Unity Callbacks

        void OnEnable() {
            _target = target as AudioGroup;
            _serializedTarget = new SerializedObject(_target);
            _groupTypeProp = _serializedTarget.FindProperty("_groupType");
            _loopProp = _serializedTarget.FindProperty("_loop");
            _playOnAwakeProp = _serializedTarget.FindProperty("_playOnAwake");
            _spatialBlendProp = _serializedTarget.FindProperty("_spatialBlend");
            _minDistanceProp = _serializedTarget.FindProperty ("_minDistance");
            _maxDistanceProp = _serializedTarget.FindProperty ("_maxDistance");
            _changePitchEachLoopProp = _serializedTarget.FindProperty("_changePitchEachLoop");
            _bassChannelProp = _serializedTarget.FindProperty("_bassChannel");
            _midChannelProp = _serializedTarget.FindProperty("_midChannel");
            _trebleChannelProp = _serializedTarget.FindProperty("_trebleChannel");
            _standardChannelProp = _serializedTarget.FindProperty("_standardChannel");
            _elementChannelsProp = _serializedTarget.FindProperty("_elementChannels");
            _useVolumeEnvelopeProp = _serializedTarget.FindProperty("_useVolumeEnvelope");
            _volumeEnvelopeProp = _serializedTarget.FindProperty("_volumeEnvelope");
            _randomPitchProp = _serializedTarget.FindProperty("_randomPitch");
            _pitchProp = _serializedTarget.FindProperty("_pitch");
            _pitchRangeProp = _serializedTarget.FindProperty("_pitchRange");
        }

        public override void OnInspectorGUI() {
            // Update serialized object
            _serializedTarget.Update();

            EditorGUILayout.Space();

            // Button group
            EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);

            // Show play sound button
            if (GUILayout.Button("Play Sound", GUILayout.Width(96f)))
                _target.Play();

            // Show stop sound button
            if (GUILayout.Button("Stop Sound", GUILayout.Width(96f)))
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

                // Group type enum
                EditorGUILayout.PropertyField(_groupTypeProp);

                // Looped toggle
                EditorGUILayout.PropertyField(_loopProp);

                // Play on awake toggle
                EditorGUILayout.PropertyField(_playOnAwakeProp);

                EditorGUILayout.PropertyField(_spatialBlendProp);

                EditorGUILayout.PropertyField(_maxDistanceProp);

                // Use volume envelope toggle
                EditorGUILayout.PropertyField(_useVolumeEnvelopeProp);

                // Volume envelope
                EditorGUI.BeginDisabledGroup(!_useVolumeEnvelopeProp.boolValue);
                EditorGUILayout.PropertyField(_volumeEnvelopeProp);
                EditorGUI.EndDisabledGroup();
                EditorGUILayout.TextArea("Looping and volume envelope functionality are not available in the editor. Test them in play mode.", EditorStyles.helpBox);
            }
            EditorGUILayout.Space();

            // Pitch settings
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUI.indentLevel++;
            if (_randomPitchProp.boolValue)
                EditorGUILayout.PropertyField(_pitchRangeProp, new GUIContent("Random Pitch"));
            else
                EditorGUILayout.PropertyField(_pitchProp);
            EditorGUI.indentLevel--;

            EditorGUILayout.PropertyField(_randomPitchProp);
            EditorGUI.BeginDisabledGroup(!_loopProp.boolValue || !_randomPitchProp.boolValue);
            EditorGUILayout.PropertyField(_changePitchEachLoopProp);
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.EndVertical();

            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();

            // Channels
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("Channels", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            switch ((AudioGroup.GroupType)_groupTypeProp.enumValueIndex) {
                case AudioGroup.GroupType.Standard:
                    EditorGUILayout.PropertyField(_standardChannelProp);
                    break;
                case AudioGroup.GroupType.Layered:
                    EditorGUILayout.PropertyField(_bassChannelProp);
                    EditorGUILayout.PropertyField(_midChannelProp);
                    EditorGUILayout.PropertyField(_trebleChannelProp);
                    break;
                case AudioGroup.GroupType.Elements:
                    int channelCount = _elementChannelsProp.arraySize;
                    for (int i = 0; i < channelCount; i++) {
                        // Check if elements have been added
                        if (_elementChannelsProp.arraySize != channelCount)
                            break;

                        // Draw remove channel button
                        var element = _elementChannelsProp.GetArrayElementAtIndex(i);
                        EditorGUILayout.PropertyField(element);
                        var lastRect = GUILayoutUtility.GetLastRect();
                        lastRect.x = lastRect.x + lastRect.width - _REMOVE_CHANNEL_BUTTON_WIDTH;
                        lastRect.width = _REMOVE_CHANNEL_BUTTON_WIDTH;
                        lastRect.height = _REMOVE_CHANNEL_BUTTON_WIDTH;
                        if (GUI.Button(lastRect, "x", EditorStyles.toolbarButton)) {

                            // KEEP BOTH OF THESE FUNCTIONS
                            // First one: sets array element to null
                            // Second one: actually removes it
                            if (element.objectReferenceValue != null)
                                _elementChannelsProp.DeleteArrayElementAtIndex(i);
                            _elementChannelsProp.DeleteArrayElementAtIndex(i);
                            break;
                        }
                    }

                    // Draw add new element channel button
                    if (GUILayout.Button("Add new element channel")) {
                        _target.AddElementChannel();
                        _serializedTarget.Update();
                    }
                    break;

            }

            EditorGUI.indentLevel--;
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();

            // Apply modified properties
            _serializedTarget.ApplyModifiedProperties();
        }

        #endregion
        #region Methods

        [MenuItem("Audio/Create audio group")]
        static void CreateAudioGroup () {
            new GameObject("New audio group", typeof(AudioGroup));
        }

        #endregion
    }
}
