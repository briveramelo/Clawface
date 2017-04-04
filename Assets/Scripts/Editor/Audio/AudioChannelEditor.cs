﻿// AudioChannelEditor.cs

using UnityEditor;
using Turing.Audio;

/// <summary>
/// Custom editor for AudioChannels.
/// </summary>
[CustomEditor(typeof(AudioChannel))]
public class AudioChannelEditor : Editor {

    /// <summary>
    /// Removes editor from AudioChannels, forcing them to be edited
    /// from their parent AudioGroup.
    /// </summary>
    public override void OnInspectorGUI() {}
}