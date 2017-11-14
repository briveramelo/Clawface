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
        SerializedProperty channelsProp;
        SerializedProperty loopProp;
        SerializedProperty playOnStartProp;
        SerializedProperty spatialBlendProp;
        SerializedProperty maxDistanceProp;
        SerializedProperty changeVolumeEachLoopProp;
        SerializedProperty uniformVolumeProp;
        SerializedProperty useRandomVolumeProp;
        SerializedProperty randomVolumeRangeProp;
        SerializedProperty changePitchEachLoopProp;
        SerializedProperty randomPitchProp;
        SerializedProperty pitchProp;
        SerializedProperty pitchRangeProp;

        #endregion
        #region Unity Lifecycle

        void OnEnable()
        {
            AGTarget = target as AudioGroup;
            serializedTarget = new SerializedObject(AGTarget);
            channelsProp = serializedTarget.FindProperty("channels");
            loopProp = serializedTarget.FindProperty("loop");
            playOnStartProp = serializedTarget.FindProperty("playOnStart");
            spatialBlendProp = serializedTarget.FindProperty("spatialBlend");
            maxDistanceProp = serializedTarget.FindProperty ("maxDistance");
            changeVolumeEachLoopProp = serializedTarget.FindProperty("changeVolumeEachLoop");
            uniformVolumeProp = serializedTarget.FindProperty("uniformVolume");
            useRandomVolumeProp = serializedTarget.FindProperty("useRandomVolume");
            randomVolumeRangeProp = serializedTarget.FindProperty("randomVolumeRange");
            changePitchEachLoopProp = serializedTarget.FindProperty("changePitchEachLoop");
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

                // Looped toggle
                EditorGUILayout.PropertyField(loopProp);

                // Play on awake toggle
                EditorGUILayout.PropertyField(playOnStartProp);

                // Spatial blend slider
                EditorGUILayout.PropertyField(spatialBlendProp);

                // Max distance value
                EditorGUILayout.PropertyField(maxDistanceProp);

                // Edit mode warning
                EditorGUILayout.TextArea(@"Looping functionality are not 
                    available in the editor. Test them in play mode.", 
                    EditorStyles.helpBox);
            }
            EditorGUILayout.Space();

            // Volume settings
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField ("Volume Settings");
            EditorGUI.indentLevel++;

            if (useRandomVolumeProp.boolValue)
                EditorGUILayout.PropertyField (randomVolumeRangeProp, 
                    new GUIContent("Random Volume"));
            else EditorGUILayout.PropertyField (uniformVolumeProp);
            EditorGUI.indentLevel--;
            EditorGUILayout.PropertyField (useRandomVolumeProp);
            EditorGUILayout.EndVertical();

            // Pitch settings
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField ("Pitch Settings");
            EditorGUI.indentLevel++;
            if (randomPitchProp.boolValue)
                EditorGUILayout.PropertyField(pitchRangeProp, 
                    new GUIContent("Random Pitch"));
            else EditorGUILayout.PropertyField(pitchProp);
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

            int channelCount = channelsProp.arraySize;
            for (int i = 0; i < channelCount; i++)
            {
                // Check if elements have been added
                if (channelsProp.arraySize != channelCount)
                    break;

                // Draw remove channel button
                var element = channelsProp.GetArrayElementAtIndex(i);
                EditorGUILayout.PropertyField(element);
                var lastRect = GUILayoutUtility.GetLastRect();
                lastRect.x = lastRect.x + lastRect.width - _REMOVE_CHANNEL_BUTTON_WIDTH;
                lastRect.width = _REMOVE_CHANNEL_BUTTON_WIDTH;
                lastRect.height = _REMOVE_CHANNEL_BUTTON_WIDTH;
                if (GUI.Button(lastRect, "x", EditorStyles.toolbarButton))
                {
                    // KEEP BOTH OF THESE FUNCTIONS
                    // First one: sets array element to null
                    // Second one: actually removes it
                    if (element.objectReferenceValue != null)
                    {
                        //channelsProp.DeleteArrayElementAtIndex(i);
                        //channelsProp.DeleteArrayElementAtIndex(i);
                        AGTarget.RemoveChannel(i);
                        break;
                    }
                }
            }

            // Draw add new element channel button
            if (GUILayout.Button("Add new element channel"))
            {
                AGTarget.AddChannel();
                serializedTarget.Update();
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
