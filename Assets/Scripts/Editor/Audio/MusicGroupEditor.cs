// MusicGroupEditor.cs
// Author: Aaron

using Turing.Audio;

using UnityEditor;

using UnityEngine;

/// <summary>
/// Custom editor for MusicGroups.
/// </summary>
[CustomEditor(typeof(MusicGroup))]
public sealed class MusicGroupEditor : Editor
{
    #region Private Fields

    const float REMOVE_INSTRUMENT_BUTTON_WIDTH = 20f;

    MusicGroup MGTarget;

    SerializedObject serializedTarget;
    SerializedProperty playOnAwakeProp;
    SerializedProperty bpmProp;
    SerializedProperty instrumentChannelsProp;

    #endregion
    #region Unity Lifecycle

    void OnEnable()
    {
        MGTarget = target as MusicGroup;
        serializedTarget = new SerializedObject(MGTarget);
        playOnAwakeProp = serializedTarget.FindProperty("playOnAwake");
        bpmProp = serializedTarget.FindProperty("bpm");
        instrumentChannelsProp = serializedTarget.FindProperty("instrumentChannels");
    }

    public override void OnInspectorGUI()
    {
        // Update serialized object
        serializedTarget.Update();

        EditorGUILayout.Space();

        // Button group
        EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);

        // Show play sound button
        if (GUILayout.Button("Play Track", GUILayout.Width(96f)))
            MGTarget.Play();

        // Show stop sound button
        if (GUILayout.Button("Stop Track", GUILayout.Width(96f)))
            MGTarget.Stop();

        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();

        EditorGUILayout.BeginVertical(EditorStyles.helpBox);

        // Global settings header
        EditorGUILayout.LabelField("Global Settings", EditorStyles.boldLabel);

        if (Application.isPlaying)
        {
            EditorGUILayout.LabelField(MGTarget.IsPlaying ? "Playing" : "Stopped");
            EditorGUILayout.LabelField("Current volume scale: " + MGTarget.VolumeScale);
        } 
        
        else
        {
            EditorGUILayout.PropertyField(playOnAwakeProp);
            EditorGUILayout.PropertyField(bpmProp);
            EditorGUILayout.TextArea("Looping functionality is not available in the editor. Test it in play mode.", EditorStyles.helpBox);
        }

        EditorGUILayout.EndVertical();
        EditorGUILayout.Space();

        // Channels
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.LabelField("Channels", EditorStyles.boldLabel);
        EditorGUI.indentLevel++;

        int instrumentCount = instrumentChannelsProp.arraySize;
        for (int i = 0; i < instrumentCount; i++)
        {
            // Check if instruments have been added
            if (instrumentChannelsProp.arraySize != instrumentCount)
                break;

            // Draw remove instrument button
            var instrument = instrumentChannelsProp.GetArrayElementAtIndex(i);
            EditorGUILayout.PropertyField(instrument);
            var lastRect = GUILayoutUtility.GetLastRect();
            lastRect.x = lastRect.x + lastRect.width - REMOVE_INSTRUMENT_BUTTON_WIDTH;
            lastRect.width = REMOVE_INSTRUMENT_BUTTON_WIDTH;
            lastRect.height = REMOVE_INSTRUMENT_BUTTON_WIDTH;
            if (GUI.Button(lastRect, "x", EditorStyles.toolbarButton))
            {
                // KEEP BOTH OF THESE FUNCTIONS
                // First one: sets array element to null
                // Second one: actually removes it
                if (instrument.objectReferenceValue != null)
                    instrumentChannelsProp.DeleteArrayElementAtIndex(i);
                instrumentChannelsProp.DeleteArrayElementAtIndex(i);
                break;
            }
        }

        // Draw add new element channel button
        if (GUILayout.Button("Add new element channel"))
        {
            MGTarget.AddInstrumentChannel();
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
    /// Creates a new music group (accessible from editor toolbar).
    /// </summary>
    [MenuItem("Audio/Create music group")]
    static void CreateMusicGroup()
    {
        new GameObject("New music group", typeof(MusicGroup));
    }

    #endregion
}
