// MusicChannelDrawer.cs
// Author: Aaron

using Turing.Audio;

using UnityEditor;

using UnityEngine;

/// <summary>
/// Custom PropertyDraw for MusicChannels.
/// </summary>
[CustomPropertyDrawer(typeof(MusicChannel))]
public sealed class MusicChannelDrawer : PropertyDrawer
{
    #region Private Fields

    /// <summary>
    /// Is this drawer currently expanded?
    /// </summary>
    bool expanded = false;

    MusicChannel targetChannel;

    SerializedObject serializedChannel;

    SerializedProperty volumeProp;
    SerializedProperty clipsProp;

    #endregion
    #region Public Methods

    public override void OnGUI(Rect position, SerializedProperty property,
        GUIContent label)
    {
        // Acquire target channel if necessary
        if (targetChannel == null)
            targetChannel = property.objectReferenceValue 
                as MusicChannel;

        // If channel is still null, channel was deleted
        if (targetChannel == null) return;

        // Get serialized reference to target channel if necessary
        if (serializedChannel == null)
            serializedChannel = new SerializedObject(property.objectReferenceValue);

        serializedChannel.Update();

        // Get serialized properties
        volumeProp = serializedChannel.FindProperty("_volume");

        // Clips box
        GUILayout.BeginVertical(EditorStyles.helpBox);
        if (expanded = EditorGUILayout.Foldout(expanded, 
            "Channel: " + targetChannel.name, true))
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Clips");
            EditorGUI.indentLevel++;

            clipsProp = serializedChannel.FindProperty ("_clips");

            for (int i = 0; i < clipsProp.arraySize; i++) {
                var clip = clipsProp.GetArrayElementAtIndex(i);
                var clipProp = clip.FindPropertyRelative("_clip");
                var barsProp = clip.FindPropertyRelative("_bars");
                EditorGUILayout.BeginHorizontal();
                //var changedClip = EditorGUILayout.ObjectField(clip.Clip, typeof(AudioClip), false) as AudioClip;
                EditorGUILayout.PropertyField(clipProp);
                if (clipProp.objectReferenceValue == null) targetChannel.Clips.RemoveAt(i);
                //else _targetChannel.Clips[i].Clip = changedClip;
                //clip.Bars = EditorGUILayout.IntField (clip.Bars);
                EditorGUILayout.PropertyField (barsProp);
                EditorGUILayout.EndHorizontal();
            }
            var newClip = EditorGUILayout.ObjectField(null, typeof(AudioClip), false) as AudioClip;
            if (newClip != null) targetChannel.AddClip(newClip);

            EditorGUI.indentLevel--;

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(volumeProp);
        }
        EditorGUILayout.EndVertical();

        serializedChannel.ApplyModifiedProperties();
    }

    #endregion
}
