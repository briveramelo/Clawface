// AudioElement.cs

using UnityEngine;
using ModMan;

/// <summary>
/// Audio class to emulate FMOD-style functionality.
/// </summary>
[ExecuteInEditMode]
[RequireComponent(typeof(AudioSource), typeof(AudioElementChannel))]
public class AudioElement : MonoBehaviour {

    #region Consts

    const string _STANDARD_CHANNEL_NAME = "STANDARD";
    const string _BASS_CHANNEL_NAME     = "BASS";
    const string _MID_CHANNEL_NAME      = "MID";
    const string _TREBLE_CHANNEL_NAME   = "TREBLE";

    #endregion
    #region Vars

    /// <summary>
    /// Audio channel object for standard (non-layered) playback.
    /// </summary>
    [SerializeField] AudioElementChannel _standardChannel;

    /// <summary>
    /// Audio channel object for layered playback.
    /// </summary>
    [SerializeField] AudioElementChannel _bassChannel, _midChannel, _trebleChannel;

    /// <summary>
    /// Will playback loop?
    /// </summary>
    [SerializeField] bool _loop = false;

    /// <summary>
    /// Change pitch each loop (when using randomized pitch)?
    /// </summary>
    [SerializeField] bool _changePitchEachLoop = false;

    /// <summary>
    /// If looped, is this AudioElement currently playing?
    /// </summary>
    bool _playing = false;

    /// <summary>
    /// Does this AudioElement use layered playback?
    /// </summary>
	[SerializeField] bool _layered = false;

    /// <summary>
    /// Randomize volume on each playback?
    /// </summary>
    [SerializeField] bool _randomVolume = false;

    /// <summary>
    /// Uniform volume to use.
    /// </summary>
    [SerializeField] float _volume = 1f;

    /// <summary>
    /// Range of volumes to randomize.
    /// </summary>
    [SerializeField] [FloatRange (0f, 1f)]
    FloatRange _volumeRange = new FloatRange();

    /// <summary>
    /// Randomize pitch on each playback?
    /// </summary>
    [SerializeField] bool _randomPitch = false;

    /// <summary>
    /// Uniform pitch to use.
    /// </summary>
    [SerializeField][Range(0f, 3f)]
    float _pitch = 1f;

    /// <summary>
    /// Range of pitches to use.
    /// </summary>
    [SerializeField][FloatRange(0f, 3f)]
    FloatRange _pitchRange = new FloatRange();

    #endregion
    #region Unity Callbacks

    private void OnEnable() {
        var channels = GetComponentsInChildren<AudioElementChannel>();
        if (channels == null) {
            Debug.LogWarning ("No AudioElementChannels found on this AudioElement! Generating now.", gameObject);
            GenerateAudioChannels();
        } else if (InvalidAudioChannels()) {
            Debug.LogWarning ("Fixing AudioElementChannels...", gameObject);
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
        if (_layered) {
            _bassChannel.PlaySound(pitch);
            _midChannel.PlaySound(pitch);
            _trebleChannel.PlaySound(pitch);
        } else _standardChannel.PlaySound(pitch);
    } 

    /// <summary>
    /// Stops all playback on this AudioElement.
    /// </summary>
    public void Stop () {
        _playing = false;
        if (_layered) {
            _bassChannel.Stop();
            _midChannel.Stop();
            _trebleChannel.Stop();
        } else _standardChannel.Stop();
    }

    /// <summary>
    /// Generates all audio channels with default settings.
    /// </summary>
    void GenerateAudioChannels () {
        _standardChannel = GenerateAudioChannel(_STANDARD_CHANNEL_NAME);
        _bassChannel     = GenerateAudioChannel (_BASS_CHANNEL_NAME);
        _midChannel      = GenerateAudioChannel (_MID_CHANNEL_NAME);
        _trebleChannel   = GenerateAudioChannel (_TREBLE_CHANNEL_NAME);
    }

    /// <summary>
    /// Generates a default audio channel with the given name.
    /// </summary>
    AudioElementChannel GenerateAudioChannel (string channelName) {
        var channelObj = gameObject.FindInChildren (channelName);
        if (channelObj == null) {
            channelObj = new GameObject (
                channelName, 
                typeof (AudioSource),
                typeof (AudioElementChannel)
                );
            channelObj.hideFlags = HideFlags.HideInHierarchy;
            channelObj.transform.SetParent (transform);
            channelObj.transform.Reset();
        } else {
            var channel = channelObj.GetComponent<AudioElementChannel>();
            if (channel == null) return channelObj.AddComponent<AudioElementChannel>();
        }
        return channelObj.GetComponent<AudioElementChannel>();
    }

    /// <summary>
    /// Returns true if any audio channels are invalid.
    /// </summary>
    /// <returns></returns>
    bool InvalidAudioChannels () {
        return _standardChannel == null || _bassChannel == null || 
            _midChannel == null || _trebleChannel == null;
    }

    #endregion
}
