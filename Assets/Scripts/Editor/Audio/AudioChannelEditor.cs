// AudioChannelEditor.cs
// Author: Aaron

using Turing.Audio;

using UnityEditor;

/// <summary>
/// Custom editor for AudioChannels.
/// </summary>
[CustomEditor(typeof(AudioChannel))]
public sealed class AudioChannelEditor : Editor
{
    #region Public Methods

    /// <summary>
    /// Removes editor from AudioChannels, forcing them to be edited
    /// from their parent AudioGroup.
    /// </summary>
    public override void OnInspectorGUI() {}

    #endregion
}
