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
        #region Private Fields

        const float _REMOVE_CHANNEL_BUTTON_WIDTH = 20f;

        /// <summary>
        /// Target AudioGroup being edited.
        /// </summary>
        AudioGroup AGTarget;

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
        #region Unity Lifecycle

        void OnEnable()
        {
            AGTarget = target as AudioGroup;
            serializedTarget = new SerializedObject(AGTarget);
            groupTypeProp = serializedTarget.FindProperty("groupType");
            loopProp = serializedTarget.FindProperty("loop");
            playOnAwakeProp = serializedTarget.FindProperty("playOnAwake");
            spatialBlendProp = serializedTarget.FindProperty("spatialBlend");
            minDistanceProp = serializedTarget.FindProperty ("minDistance");
            maxDistanceProp = serializedTarget.FindProperty ("maxDistance");
            changePitchEachLoopProp = serializedTarget.FindProperty("changePitchEachLoop");
            bassChannelProp = serializedTarget.FindProperty("bassChannel");
            midChannelProp = serializedTarget.FindProperty("midChannel");
            trebleChannelProp = serializedTarget.FindProperty("trebleChannel");
            standardChannelProp = serializedTarget.FindProperty("standardChannel");
            elementChannelsProp = serializedTarget.FindProperty("elementChannels");
            useVolumeEnvelopeProp = serializedTarget.FindProperty("useVolumeEnvelope");
            volumeEnvelopeProp = serializedTarget.FindProperty("volumeEnvelope");
            randomPitchProp = serializedTarget.FindProperty("randomPitch");
            pitchProp = serializedTarget.FindProperty("pitch");
            pitchRangeProp = serializedTarget.FindProperty("pitchRange");
        }

        #endregion
        #region Public Methods

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
        #region Private Methods

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
