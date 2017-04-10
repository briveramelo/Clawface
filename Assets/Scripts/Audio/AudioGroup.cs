// AudioGroup.cs

using System.Collections.Generic;
using UnityEngine;
using ModMan;

namespace Turing.Audio {

    /// <summary>
    /// Audio class to emulate FMOD-style functionality.
    /// </summary>
    [ExecuteInEditMode]
    [RequireComponent(typeof(AudioSource), typeof(AudioChannel))]
    public sealed class AudioGroup : MonoBehaviour {

        #region Consts

        const string _STANDARD_CHANNEL_NAME = "STANDARD";
        const string _BASS_CHANNEL_NAME = "BASS";
        const string _MID_CHANNEL_NAME = "MID";
        const string _TREBLE_CHANNEL_NAME = "TREBLE";
        const string _ELEMENTS_PARENT_NAME = "ELEMENTS";
        const HideFlags _CHANNEL_HIDE_FLAGS = HideFlags.HideInHierarchy;

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

        [SerializeField]
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

        [SerializeField]
        bool _playOnAwake = false;

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

        float _playbackTime = 0f;

        float _longestClipLength;

        [SerializeField]
        bool _useVolumeEnvelope = false;

        /// <summary>
        /// Volume envelope for this AudioGroup.
        /// </summary>
        [SerializeField]
        [Tooltip("Volume envelope for this AudioGroup.")]
        AnimationCurve _volumeEnvelope;

        /// <summary>
        /// Randomize pitch on each playback?
        /// </summary>
        [SerializeField]
        [Tooltip("Randomize pitch on each playback?")]
        bool _randomPitch = false;

        /// <summary>
        /// Uniform pitch to use.
        /// </summary>
        [SerializeField]
        [Range(0f, 3f)]
        [Tooltip("Uniform pitch to use.")]
        float _pitch = 1f;

        /// <summary>
        /// Range of pitches to use.
        /// </summary>
        [SerializeField]
        [FloatRange(0f, 3f)]
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
            if (!Application.isPlaying) {
                var channels = GetComponentsInChildren<AudioChannel>();
                if (channels == null) {
                    Debug.LogWarning("No AudioChannels found on this AudioGroup! Generating now.", gameObject);
                    GenerateAudioChannels();
                } else if (InvalidAudioChannels()) {
                    Debug.LogWarning("Fixing AudioChannels...", gameObject);
                    GenerateAudioChannels();
                }
            } else {
                if (_playOnAwake) Play();
            }
        }

        void Update() {
            if (_playing) {
                _playbackTime += Time.deltaTime;
                if (_playbackTime >= _longestClipLength) {
                    if (_loop) Play(true);
                    else _playing = false;

                } else {
                    if (_useVolumeEnvelope) {
                        if (_volumeEnvelope != null && _volumeEnvelope.length > 0) {
                            SetVolumeScale(_volumeEnvelope.Evaluate(_playbackTime));
                        }
                    }
                }
            }
        }

        #endregion
            #region Properties

            /// <summary>
            /// Returns true if this AudioGroup loops (read-only).
            /// </summary>
        public bool Loop { get { return _loop; } }

        public bool IsPlaying { get { return _playing; } }

        public float VolumeScale {
            get {
                if (_useVolumeEnvelope) return _volumeEnvelope.Evaluate(_playbackTime);
                else return 1f;
            }
        }

        #endregion
        #region Methods

        /// <summary>
        /// Plays this AudioElement.
        /// </summary>
        public void Play(bool loop=false) {
            _playing = true;
            _playbackTime = 0f;
            float pitch;
            if (loop && _randomPitch && _changePitchEachLoop) pitch = _pitchRange.GetRandomValue();
            else pitch = _randomPitch ? _pitchRange.GetRandomValue() : _pitch;

            if (_useVolumeEnvelope) {
                if (!Application.isPlaying) SetVolumeScale(1f);
                else {
                    if (_volumeEnvelope == null || _volumeEnvelope.length == 0) SetVolumeScale(1f);
                    SetVolumeScale(_volumeEnvelope.Evaluate(0f));
                }
            }

            switch (_groupType) {
                case GroupType.Standard:
                    _standardChannel.PlaySound(pitch);
                    _longestClipLength = _standardChannel.ClipLength;
                    break;
                case GroupType.Layered:
                    _bassChannel.PlaySound(pitch);
                    _midChannel.PlaySound(pitch);
                    _trebleChannel.PlaySound(pitch);
                    _longestClipLength = Mathf.Max(_bassChannel.ClipLength,
                        _midChannel.ClipLength, _trebleChannel.ClipLength);
                    break;
                case GroupType.Elements:
                    float maxLength = 0f;
                    foreach (var channel in _elementChannels) {
                        channel.PlaySound(pitch);
                        if (channel.ClipLength > maxLength)
                            maxLength = channel.ClipLength;
                    }
                    _longestClipLength = maxLength;
                    break;
            }
        }

        /// <summary>
        /// Stops all playback on this AudioElement.
        /// </summary>
        public void Stop() {
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

        void SetVolumeScale(float volumeScale) {
            switch (_groupType) {
                case GroupType.Standard:
                    _standardChannel.VolumeScale = volumeScale;
                    break;
                case GroupType.Layered:
                    _bassChannel.VolumeScale = volumeScale;
                    _midChannel.VolumeScale = volumeScale;
                    _trebleChannel.VolumeScale = volumeScale;
                    break;
                case GroupType.Elements:
                    foreach (var channel in _elementChannels)
                        channel.VolumeScale = volumeScale;
                    break;
            }
        }

        /// <summary>
        /// Generates all audio channels with default settings.
        /// </summary>
        void GenerateAudioChannels() {
            _standardChannel = GenerateAudioChannel(_STANDARD_CHANNEL_NAME, transform);
            _bassChannel = GenerateAudioChannel(_BASS_CHANNEL_NAME, transform);
            _midChannel = GenerateAudioChannel(_MID_CHANNEL_NAME, transform);
            _trebleChannel = GenerateAudioChannel(_TREBLE_CHANNEL_NAME, transform);
            if (_elementParent == null) {
                _elementParent = new GameObject(_ELEMENTS_PARENT_NAME).transform;
                _elementParent.hideFlags = _CHANNEL_HIDE_FLAGS;
                _elementParent.SetParent(transform);
                _elementParent.Reset();
            }
        }

        /// <summary>
        /// Generates a default audio channel with the given name.
        /// </summary>
        AudioChannel GenerateAudioChannel(string channelName, Transform parent) {
            var channelObj = gameObject.FindInChildren(channelName);
            AudioChannel channel;
            if (channelObj == null) {
                channelObj = new GameObject(
                    channelName,
                    typeof(AudioSource),
                    typeof(AudioChannel)
                    );
                channelObj.hideFlags = _CHANNEL_HIDE_FLAGS;
                channelObj.transform.SetParent(parent);
                channelObj.transform.Reset();
                channel = channelObj.GetComponent<AudioChannel>();
            } else {
                channel = channelObj.GetComponent<AudioChannel>();
                if (channel == null) channel = channelObj.AddComponent<AudioChannel>();
            }
            channel.SetParent(this);
            return channel;
        }

        /// <summary>
        /// Adds an element channel to this AudioGroup.
        /// </summary>
        public void AddElementChannel() {
            var channel = GenerateAudioChannel(_ELEMENTS_PARENT_NAME + _elementChannelIndex++, _elementParent.transform);
            channel.SetParent(this);
            _elementChannels.Add(channel);
        }

        /// <summary>
        /// Removes an elementchannel from this AudioGroup.
        /// </summary>
        /// <param name="index"></param>
        public void RemoveElementChannel(int index) {
            _elementChannels.RemoveAt(index);
        }

        /// <summary>
        /// Returns true if any audio channels are invalid.
        /// </summary>
        /// <returns></returns>
        bool InvalidAudioChannels() {
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
            Layered = 1, // Bass/mid/treble channels
            Elements = 2  // Per-element channels
        }

        #endregion
    }
}
