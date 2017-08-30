// AudioGroupEditor.cs
// Author: Aaron

using UnityEditor;

using UnityEngine;

namespace Turing.Audio
{
    /// <summary>
    /// Custom editor for AudioGroups.
    /// </summary>
    [CustomEditor(typeof(AudioGroup))]
    public sealed class AudioGroupEditor : Editor
    {
        #region Consts

        const float _REMOVE_CHANNEL_BUTTON_WIDTH = 20f;

        #endregion
        #region Vars

        /// <summary>
        /// Target AudioGroup being edited.
        /// </summary>
        AudioGroup AGTarget;

        #endregion
        #region Serialized Properties

        SerializedObject serializedTarget;
        SerializedProperty groupTypeProp;
        SerializedProperty loopProp;
        SerializedProperty playOnAwakeProp;
        SerializedProperty spatialBlendProp;
        SerializedProperty minDistanceProp;
        SerializedProperty maxDistanceProp;
        SerializedProperty changePitchEachLoopProp;
        SerializedProperty bassChannelProp;
        SerializedProperty midChannelProp;
        SerializedProperty trebleChannelProp;
        SerializedProperty standardChannelProp;
        SerializedProperty elementChannelsProp;
        SerializedProperty useVolumeEnvelopeProp;
        SerializedProperty volumeEnvelopeProp;
        SerializedProperty randomPitchProp;
        SerializedProperty pitchProp;
        SerializedProperty pitchRangeProp;

        #endregion
        #region Unity Callbacks

        void OnEnable()
        {
            AGTarget = target as AudioGroup;
            serializedTarget = new SerializedObject(AGTarget);
            groupTypeProp = serializedTarget.FindProperty("_groupType");
            loopProp = serializedTarget.FindProperty("_loop");
            playOnAwakeProp = serializedTarget.FindProperty("_playOnAwake");
            spatialBlendProp = serializedTarget.FindProperty("_spatialBlend");
            minDistanceProp = serializedTarget.FindProperty ("_minDistance");
            maxDistanceProp = serializedTarget.FindProperty ("_maxDistance");
            changePitchEachLoopProp = serializedTarget.FindProperty("_changePitchEachLoop");
            bassChannelProp = serializedTarget.FindProperty("_bassChannel");
            midChannelProp = serializedTarget.FindProperty("_midChannel");
            trebleChannelProp = serializedTarget.FindProperty("_trebleChannel");
            standardChannelProp = serializedTarget.FindProperty("_standardChannel");
            elementChannelsProp = serializedTarget.FindProperty("_elementChannels");
            useVolumeEnvelopeProp = serializedTarget.FindProperty("_useVolumeEnvelope");
            volumeEnvelopeProp = serializedTarget.FindProperty("_volumeEnvelope");
            randomPitchProp = serializedTarget.FindProperty("_randomPitch");
            pitchProp = serializedTarget.FindProperty("_pitch");
            pitchRangeProp = serializedTarget.FindProperty("_pitchRange");
        }

        public override void OnInspectorGUI()
        {
            // Update serialized object
            serializedTarget.Update();

            EditorGUILayout.Space();

            // Button group
            EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);

            // Show play sound button
            if (GUILayout.Button("Play Sound", GUILayout.Width(96f)))
                AGTarget.Play();

            // Show stop sound button
            if (GUILayout.Button("Stop Sound", GUILayout.Width(96f)))
                AGTarget.Stop();

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            // Global settings header
            EditorGUILayout.LabelField("Global Settings", EditorStyles.boldLabel);

            if (Application.isPlaying)
            {
                EditorGUILayout.LabelField(AGTarget.IsPlaying ? "Playing" : "Stopped");
                EditorGUILayout.LabelField("Current volume scale: " + AGTarget.VolumeScale);
            } 
            
            else
            {
                // Group type enum
                EditorGUILayout.PropertyField(groupTypeProp);

                // Looped toggle
                EditorGUILayout.PropertyField(loopProp);

                // Play on awake toggle
                EditorGUILayout.PropertyField(playOnAwakeProp);

                EditorGUILayout.PropertyField(spatialBlendProp);

                EditorGUILayout.PropertyField(maxDistanceProp);

                // Use volume envelope toggle
                EditorGUILayout.PropertyField(useVolumeEnvelopeProp);

                // Volume envelope
                EditorGUI.BeginDisabledGroup(!useVolumeEnvelopeProp.boolValue);
                EditorGUILayout.PropertyField(volumeEnvelopeProp);
                EditorGUI.EndDisabledGroup();
                EditorGUILayout.TextArea("Looping and volume envelope functionality are not available in the editor. Test them in play mode.", EditorStyles.helpBox);
            }
            EditorGUILayout.Space();

            // Pitch settings
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUI.indentLevel++;
            if (randomPitchProp.boolValue)
                EditorGUILayout.PropertyField(pitchRangeProp, new GUIContent("Random Pitch"));
            else
                EditorGUILayout.PropertyField(pitchProp);
            EditorGUI.indentLevel--;

            EditorGUILayout.PropertyField(randomPitchProp);
            EditorGUI.BeginDisabledGroup(!loopProp.boolValue || !randomPitchProp.boolValue);
            EditorGUILayout.PropertyField(changePitchEachLoopProp);
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.EndVertical();

            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();

            // Channels
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("Channels", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            switch ((AudioGroup.GroupType)groupTypeProp.enumValueIndex)
            {
                case AudioGroup.GroupType.Standard:
                    EditorGUILayout.PropertyField(standardChannelProp);
                    break;

                case AudioGroup.GroupType.Layered:
                    EditorGUILayout.PropertyField(bassChannelProp);
                    EditorGUILayout.PropertyField(midChannelProp);
                    EditorGUILayout.PropertyField(trebleChannelProp);
                    break;

                case AudioGroup.GroupType.Elements:
                    int channelCount = elementChannelsProp.arraySize;
                    for (int i = 0; i < channelCount; i++)
                    {
                        // Check if elements have been added
                        if (elementChannelsProp.arraySize != channelCount)
                            break;

                        // Draw remove channel button
                        var element = elementChannelsProp.GetArrayElementAtIndex(i);
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
                                elementChannelsProp.DeleteArrayElementAtIndex(i);
                            elementChannelsProp.DeleteArrayElementAtIndex(i);
                            break;
                        }
                    }

                    // Draw add new element channel button
                    if (GUILayout.Button("Add new element channel"))
                    {
                        AGTarget.AddElementChannel();
                        serializedTarget.Update();
                    }
                    break;

            }

            EditorGUI.indentLevel--;
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();

            // Apply modified properties
            serializedTarget.ApplyModifiedProperties();
        }

        #endregion
        #region Methods

        /// <summary>
        /// Creates a new audio group (accessible from editor).
        /// </summary>
        [MenuItem("Audio/Create audio group")]
        static void CreateAudioGroup ()
        {
            new GameObject("New audio group", typeof(AudioGroup));
        }

        #endregion
    }
}
