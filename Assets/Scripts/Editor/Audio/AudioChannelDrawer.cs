// AudioChannelDrawer.cs
// Author: Aaron

using UnityEditor;

using UnityEngine;

using Turing.Audio;

/// <summary>
/// Custom property drawer for AudioChannels.
/// </summary>
[CustomPropertyDrawer(typeof(AudioChannel))]
public sealed class AudioChannelDrawer : PropertyDrawer
{
    #region Private Fields

    /// <summary>
    /// Is the clip list expanded?
    /// </summary>
    bool expanded = false;

    /// <summary>
    /// Target AudioElementChannel.
    /// </summary>
    AudioChannel targetChannel;

    /// <summary>
    /// Serialized version of target AudioElement Channel.
    /// Needed for finding SerializedProperties.
    /// </summary>
    SerializedObject serializedChannel;

    SerializedProperty volumeProp;
    SerializedProperty randomVolumeProp;
    SerializedProperty volumeRangeProp;
    SerializedProperty changeVolumeEachLoopProp;

    #endregion
    #region Public Methods

    public override float GetPropertyHeight(SerializedProperty property, 
        GUIContent label)
    {
        return base.GetPropertyHeight(property, label) - 16f;
    }

    public override void OnGUI(Rect position, SerializedProperty property, 
        GUIContent label)
    {
        // Acquire target channel if necessary
        if (targetChannel == null)
            targetChannel = property.objectReferenceValue as AudioChannel;

        // If channel is still null, channel was deleted
        if (targetChannel == null) return;

        // Get serialized reference to target channel if necessary
        if (serializedChannel == null)
            serializedChannel = 
                new SerializedObject(property.objectReferenceValue);

        serializedChannel.Update();

        // Get serialized properties
        volumeProp = serializedChannel.FindProperty("uniformVolume");
        randomVolumeProp = serializedChannel.FindProperty("useRandomVolume");
        volumeRangeProp = serializedChannel.FindProperty("randomVolumeRange");
        changeVolumeEachLoopProp = 
            serializedChannel.FindProperty("changeVolumeEachLoop");

        // Clips box
        GUILayout.BeginVertical(EditorStyles.helpBox);
        if (expanded = EditorGUILayout.Foldout(expanded, 
            "Channel: " + targetChannel.name, true))
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Clips");
            EditorGUI.indentLevel++;

            var clips = targetChannel.Clips;

            for (int i = 0; i < targetChannel.Clips.Count; i++)
            {
                var clip = targetChannel.Clips[i];
                var changedClip = EditorGUILayout.ObjectField(clip, 
                    typeof(AudioClip), false) as AudioClip;
                if (changedClip == null) targetChannel.Clips.RemoveAt(i);
                else targetChannel.Clips[i] = changedClip;
            }
            var newClip = EditorGUILayout.ObjectField(null, 
                typeof(AudioClip), false) as AudioClip;
            if (newClip != null) targetChannel.AddClip(newClip);

            EditorGUI.indentLevel--;

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(randomVolumeProp);
            if (targetChannel.UseRandomVolume)
                EditorGUILayout.PropertyField(volumeRangeProp);
            else
                EditorGUILayout.PropertyField(volumeProp);

            EditorGUI.BeginDisabledGroup(!targetChannel.Parent.Loop ||
                !randomVolumeProp.boolValue);
            EditorGUILayout.PropertyField(changeVolumeEachLoopProp);
            EditorGUI.EndDisabledGroup();
        }

        if (GUILayout.Button ("Preview")) targetChannel.PlaySound(1f, false);
        EditorGUILayout.EndVertical();

        serializedChannel.ApplyModifiedProperties();
    }

    #endregion
}
