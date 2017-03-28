// AudioGroup.cs

using System.Collections.Generic;
using UnityEngine;
using ModMan;

/// <summary>
/// Audio class to emulate FMOD-style functionality.
/// </summary>
[ExecuteInEditMode]
[RequireComponent(typeof(AudioSource), typeof(AudioChannel))]
public class AudioGroup : MonoBehaviour {

    #region Consts

    const string _STANDARD_CHANNEL_NAME = "STANDARD";
    const string _BASS_CHANNEL_NAME     = "BASS";
    const string _MID_CHANNEL_NAME      = "MID";
    const string _TREBLE_CHANNEL_NAME   = "TREBLE";
    const string _ELEMENTS_PARENT_NAME  = "ELEMENTS";

    #endregion
    #region Vars

    /// <summary>
    /// Type of group setup to use.
    /// </summary>
    [SerializeField]
    [Tooltip("Type of group setup to use.")]
    GroupType _groupType = GroupType.Standard;

    /// <summary>
    /// AudioChannel object for standard (non-layered) playback.
    /// </summary>
    [SerializeField]
    [Tooltip("Audio channel object for standard (non-layered) playback.")]
    AudioChannel _standardChannel;

    /// <summary>
    /// AudioChannel objects for layered playback.
    /// </summary>
    [SerializeField]
    [Tooltip("Audio channel objects for layered playback.")]
    AudioChannel _bassChannel, _midChannel, _trebleChannel;

    /// <summary>
    /// GameObject to which to parent element AudioChannels.
    /// </summary>
    Transform _elementParent;

    /// <summary>
    /// List of element AudioChannels.
    /// </summary>
    [SerializeField]
    [Tooltip("List of element AudioChannels.")]
    List<AudioChannel> _elementChannels = new List<AudioChannel>();

    /// <summary>
    /// Will playback loop?
    /// </summary>
    [SerializeField]
    [Tooltip("Will playback loop?")]
    bool _loop = false;

    /// <summary>
    /// Change pitch each loop (when using randomized pitch)?
    /// </summary>
    [SerializeField]
    [Tooltip("Change pitch each loop (when using randomized pitch)?")]
    bool _changePitchEachLoop = false;

    /// <summary>
    /// If looped, is this AudioGroup currently playing?
    /// </summary>
    bool _playing = false;

    /// <summary>
    /// Randomize volume on each playback?
    /// </summary>
    [SerializeField]
    [Tooltip("Randomize volume on each playback?")]
    bool _randomVolume = false;

    /// <summary>
    /// Uniform volume to use.
    /// </summary>
    [SerializeField]
    [Tooltip("Uniform volume to use.")]
    float _volume = 1f;

    /// <summary>
    /// Range of volumes to randomize.
    /// </summary>
    [SerializeField][FloatRange (0f, 1f)]
    [Tooltip("Range of volumes to randomize.")]
    FloatRange _volumeRange = new FloatRange();

    /// <summary>
    /// Randomize pitch on each playback?
    /// </summary>
    [SerializeField]
    [Tooltip("Randomize pitch on each playback?")]
    bool _randomPitch = false;

    /// <summary>
    /// Uniform pitch to use.
    /// </summary>
    [SerializeField][Range(0f, 3f)]
    [Tooltip("Uniform pitch to use.")]
    float _pitch = 1f;

    /// <summary>
    /// Range of pitches to use.
    /// </summary>
    [SerializeField][FloatRange(0f, 3f)]
    [Tooltip("Range of pitches to use.")]
    FloatRange _pitchRange = new FloatRange();

    /// <summary>
    /// Index of next element channel (for naming).
    /// </summary>
    [SerializeField]
    int _elementChannelIndex = 0;

    #endregion
    #region Unity Callbacks

    private void OnEnable() {
        var channels = GetComponentsInChildren<AudioChannel>();
        if (channels == null) {
            Debug.LogWarning ("No AudioChannels found on this AudioGroup! Generating now.", gameObject);
            GenerateAudioChannels();
        } else if (InvalidAudioChannels()) {
            Debug.LogWarning ("Fixing AudioChannels...", gameObject);
            GenerateAudioChannels();
        }
    }

    #endregion
    #region Methods

    /// <summary>
    /// Plays this AudioElement.
    /// </summary>
    public void Play () {
        if (_loop) _playing = true;
        var pitch = _randomPitch ? _pitchRange.GetRandomValue() : _pitch;

        switch (_groupType) {
            case GroupType.Standard:
                _standardChannel.PlaySound(pitch);
                break;
            case GroupType.Layered:
                _bassChannel.PlaySound(pitch);
                _midChannel.PlaySound(pitch);
                _trebleChannel.PlaySound(pitch);
                break;
            case GroupType.Elements:
                foreach (var channel in _elementChannels)
                    channel.PlaySound(pitch);
                break;
        }
    } 

    /// <summary>
    /// Stops all playback on this AudioElement.
    /// </summary>
    public void Stop () {
        _playing = false;

        switch (_groupType) {
            case GroupType.Standard:
                _standardChannel.Stop();
                break;
            case GroupType.Layered:
                _bassChannel.Stop();
                _midChannel.Stop();
                _trebleChannel.Stop();
                break;
            case GroupType.Elements:
                foreach (var channel in _elementChannels)
                    channel.Stop();
                break;
        }
    }

    /// <summary>
    /// Generates all audio channels with default settings.
    /// </summary>
    void GenerateAudioChannels () {
        _standardChannel = GenerateAudioChannel(_STANDARD_CHANNEL_NAME, transform);
        _bassChannel     = GenerateAudioChannel (_BASS_CHANNEL_NAME, transform);
        _midChannel      = GenerateAudioChannel (_MID_CHANNEL_NAME, transform);
        _trebleChannel   = GenerateAudioChannel (_TREBLE_CHANNEL_NAME, transform);
        if (_elementParent == null) {
            _elementParent = new GameObject(_ELEMENTS_PARENT_NAME).transform;
            _elementParent.hideFlags = HideFlags.HideInHierarchy;
            _elementParent.SetParent (transform);
            _elementParent.Reset();
        }
    }

    /// <summary>
    /// Generates a default audio channel with the given name.
    /// </summary>
    AudioChannel GenerateAudioChannel (string channelName, Transform parent) {
        var channelObj = gameObject.FindInChildren (channelName);
        if (channelObj == null) {
            channelObj = new GameObject (
                channelName, 
                typeof (AudioSource),
                typeof (AudioChannel)
                );
            channelObj.hideFlags = HideFlags.HideInHierarchy;
            channelObj.transform.SetParent (parent);
            channelObj.transform.Reset();
        } else {
            var channel = channelObj.GetComponent<AudioChannel>();
            if (channel == null) return channelObj.AddComponent<AudioChannel>();
        }
        return channelObj.GetComponent<AudioChannel>();
    }

    public void AddElementChannel() {
        var channel = GenerateAudioChannel (_ELEMENTS_PARENT_NAME + _elementChannelIndex++, _elementParent.transform);
        _elementChannels.Add (channel);
    }

    public void RemoveElementChannel (int index) {
        _elementChannels.RemoveAt (index);
    }

    /// <summary>
    /// Returns true if any audio channels are invalid.
    /// </summary>
    /// <returns></returns>
    bool InvalidAudioChannels () {
        return _standardChannel == null || _bassChannel == null || 
            _midChannel == null || _trebleChannel == null || _elementParent == null;
    }

    #endregion
    #region Enums

    /// <summary>
    /// Enum for type of AudioGroup.
    /// </summary>
    public enum GroupType {
        Standard = 0, // Single channel
        Layered  = 1, // Bass/mid/treble channels
        Elements = 2  // Per-element channels
    }

    #endregion
}
