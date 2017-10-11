// AudioGroup.cs
// Author: Aaron

using ModMan;

using System.Collections.Generic;
using System.Linq;

using UnityEngine;

namespace Turing.Audio
{
    /// <summary>
    /// Audio class to emulate FMOD-style functionality.
    /// </summary>
    [ExecuteInEditMode]
    public sealed class AudioGroup : MonoBehaviour
    {
        #region Public Fields



        #endregion
        #region Serialized Unity Inspector Fields

        /// <summary>
        /// Type of group setup to use.
        /// </summary>
        [SerializeField]
        [Tooltip("Type of group setup to use.")]
        GroupType groupType = GroupType.Standard;

        /// <summary>
        /// AudioChannel object for standard (non-layered) playback.
        /// </summary>
        [SerializeField]
        [Tooltip("Audio channel object for standard (non-layered) playback.")]
        AudioChannel standardChannel;

        /// <summary>
        /// AudioChannel objects for layered playback.
        /// </summary>
        [SerializeField]
        [Tooltip("Audio channel objects for layered playback.")]
        AudioChannel bassChannel, midChannel, trebleChannel;

        /// <summary>
        /// List of element AudioChannels.
        /// </summary>
        [SerializeField]
        [Tooltip("List of element AudioChannels.")]
        List<AudioChannel> elementChannels = new List<AudioChannel>();

        /// <summary>
        /// Will playback loop?
        /// </summary>
        [SerializeField]
        [Tooltip("Will playback loop?")]
        bool loop = false;

        /// <summary>
        /// Should this AudioGroup play on awake?
        /// </summary>
        [SerializeField]
        [Tooltip("Should this AudioGroup play on awake?")]
        bool playOnAwake = false;
        
        /// <summary>
        /// The spatial blend of this AudioGroup (0 = 2D, 1 = 3D).
        /// </summary>
        [SerializeField][Range(0f, 1f)]
        [Tooltip("The spatial blend of this AudioGroup (0 = 2D, 1 = 3D).")]
        float spatialBlend = 0f;

        /// <summary>
        /// The max distance at which this AudioGroup can be heard (UU).
        /// </summary>
        [SerializeField]
        [Tooltip("The max distance at which this AudioGroup can be heard (UU).")]
        float maxDistance = 500f;

        /// <summary>
        /// Change pitch each loop (when using randomized pitch)?
        /// </summary>
        [SerializeField]
        [Tooltip("Change pitch each loop (when using randomized pitch)?")]
        bool changePitchEachLoop = false;

        /// <summary>
        /// Use the volume envelope for this AudioGroup?
        /// </summary>
        [SerializeField]
        [Tooltip("Use the volume envelope for this AudioGroup?")]
        bool useVolumeEnvelope = false;

        /// <summary>
        /// Volume envelope for this AudioGroup.
        /// </summary>
        [SerializeField]
        [Tooltip("Volume envelope for this AudioGroup.")]
        AnimationCurve volumeEnvelope;

        /// <summary>
        /// Randomize pitch on each playback?
        /// </summary>
        [SerializeField]
        [Tooltip("Randomize pitch on each playback?")]
        bool randomPitch = false;

        /// <summary>
        /// Uniform pitch to use.
        /// </summary>
        [SerializeField][Range(0f, 3f)]
        [Tooltip("Uniform pitch to use.")]
        float pitch = 1f;

        /// <summary>
        /// Range of pitches to use.
        /// </summary>
        [SerializeField][FixedFloatRange(0f, 3f)]
        [Tooltip("Range of pitches to use.")]
        FloatRange pitchRange = new FloatRange();

        /// <summary>
        /// Index of next element channel (for naming).
        /// </summary>
        [SerializeField]
        int elementChannelIndex = 0;

        /// <summary>
        /// GameObject to which to parent element AudioChannels.
        /// </summary>
        [SerializeField]
        Transform elementParent;

        #endregion
        #region Private Fields

        const string STANDARD_CHANNEL_NAME = "STANDARD";
        const string BASS_CHANNEL_NAME = "BASS";
        const string MID_CHANNEL_NAME = "MID";
        const string TREBLE_CHANNEL_NAME = "TREBLE";
        const string ELEMENTS_PARENT_NAME = "ELEMENTS";
        const HideFlags CHANNEL_HIDE_FLAGS = HideFlags.HideInHierarchy;

        AudioListener al;

        /// <summary>
        /// If looped, is this AudioGroup currently playing?
        /// </summary>
        bool playing = false;

        /// <summary>
        /// Current playback time of the AudioGroup (seconds).
        /// </summary>
        float playbackTime = 0f;

        /// <summary>
        /// Length of the longest clip in this AudioGroup (used for looping).
        /// </summary>
        float longestClipLength;

        #endregion
        #region Unity Lifecycle

        private void OnEnable()
        {
            if (!Application.isPlaying)
            {
                var channels = GetComponentsInChildren<AudioChannel>();
                if (channels == null)
                {
                    if (Application.isEditor || Debug.isDebugBuild)
                        Debug.LogWarning(
                            "No AudioChannels found on this AudioGroup! Generating now.", 
                            gameObject);

                    GenerateAudioChannels();
                } 
                
                else if (InvalidAudioChannels())
                {
                    if (Application.isEditor || Debug.isDebugBuild)
                        Debug.LogWarning("Fixing AudioChannels...", gameObject);

                    GenerateAudioChannels();
                }
            } 
        }

        private void Start()
        {
            if (Application.isPlaying && playOnAwake) Play();
        } 

        void Update()
        {
            if (playing)
            {
                playbackTime += Time.deltaTime;
                if (playbackTime >= longestClipLength)
                {
                    if (loop) Play(true);
                    else playing = false;
                } 
                
                else
                {
                    if (useVolumeEnvelope)
                    {
                        if (volumeEnvelope != null && volumeEnvelope.length > 0)
                        {
                            SetVolumeScale(volumeEnvelope.Evaluate(playbackTime));
                        }
                    }
                }
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere (transform.position, maxDistance);
        }

        #endregion
        #region Public Methods

        /// <summary>
        /// Returns true if this AudioGroup loops (read-only).
        /// </summary>
        public bool Loop { get { return loop; } }

        /// <summary>
        /// Returns true if this AudioGroup is playing (read-only).
        /// </summary>
        public bool IsPlaying { get { return playing; } }

        /// <summary>
        /// Returns the current volume scale of this AudioGroup (read-only).
        /// </summary>
        public float VolumeScale
        {
            get {
                float v = useVolumeEnvelope ? volumeEnvelope.Evaluate(playbackTime) : 1f;
                v *= (1f - spatialBlend) + CalculateSpatializedVolumeScale();
                return v;
            }
        }

        /// <summary>
        /// Plays this AudioElement.
        /// </summary>
        public void Play(bool loop=false)
        {
            playing = true;
            playbackTime = 0f;
            if (loop && randomPitch && changePitchEachLoop) pitch = pitchRange.GetRandomValue();
            else pitch = randomPitch ? pitchRange.GetRandomValue() : pitch;

            if (useVolumeEnvelope) {
                if (!Application.isPlaying) SetVolumeScale(1f);
                else {
                    if (volumeEnvelope == null || volumeEnvelope.length == 0) SetVolumeScale(VolumeScale);
                    SetVolumeScale(VolumeScale);
                }
            }

            switch (groupType)
            {
                case GroupType.Standard:
                    StandardChannel.PlaySound(pitch);
                    longestClipLength = standardChannel.ClipLength;
                    break;

                case GroupType.Layered:
                    bassChannel.PlaySound(pitch);
                    midChannel.PlaySound(pitch);
                    trebleChannel.PlaySound(pitch);
                    longestClipLength = Mathf.Max(bassChannel.ClipLength,
                        midChannel.ClipLength, trebleChannel.ClipLength);
                    break;

                case GroupType.Elements:
                    float maxLength = 0f;
                    foreach (var channel in ElementChannels)
                    {
                        channel.PlaySound(pitch);
                        if (channel.ClipLength > maxLength)
                            maxLength = channel.ClipLength;
                    }
                    longestClipLength = maxLength;
                    break;
            }
        }

        /// <summary>
        /// Stops all playback on this AudioElement.
        /// </summary>
        public void Stop()
        {
            playing = false;

            switch (groupType)
            {
                case GroupType.Standard:
                    standardChannel.Stop();
                    break;

                case GroupType.Layered:
                    bassChannel.Stop();
                    midChannel.Stop();
                    trebleChannel.Stop();
                    break;

                case GroupType.Elements:
                    foreach (var channel in elementChannels)
                        channel.Stop();
                    break;
            }
        }

        /// <summary>
        /// Sets the volume scale of this AudioGroup.
        /// </summary>
        void SetVolumeScale(float volumeScale)
        {
            switch (groupType)
            {
                case GroupType.Standard:
                    standardChannel.VolumeScale = volumeScale;
                    break;

                case GroupType.Layered:
                    bassChannel.VolumeScale = volumeScale;
                    midChannel.VolumeScale = volumeScale;
                    trebleChannel.VolumeScale = volumeScale;
                    break;

                case GroupType.Elements:
                    foreach (var channel in elementChannels)
                        channel.VolumeScale = volumeScale;
                    break;
            }
        }

        /// <summary>
        /// Generates all audio channels with default settings.
        /// </summary>
        void GenerateAudioChannels()
        {
            standardChannel = GenerateAudioChannel(STANDARD_CHANNEL_NAME, transform);
            bassChannel = GenerateAudioChannel(BASS_CHANNEL_NAME, transform);
            midChannel = GenerateAudioChannel(MID_CHANNEL_NAME, transform);
            trebleChannel = GenerateAudioChannel(TREBLE_CHANNEL_NAME, transform);
            if (elementParent == null)
            {
                elementParent = new GameObject(ELEMENTS_PARENT_NAME).transform;
                elementParent.hideFlags = CHANNEL_HIDE_FLAGS;
                elementParent.SetParent(transform);
                elementParent.Reset();
            }
        }

        /// <summary>
        /// Generates a default audio channel with the given name.
        /// </summary>
        AudioChannel GenerateAudioChannel(string channelName, Transform parent)
        {
            var channelObj = gameObject.FindInChildren(channelName);
            AudioChannel channel;
            if (channelObj == null)
            {
                channelObj = new GameObject(
                    channelName,
                    typeof(AudioSource),
                    typeof(AudioChannel)
                    );
                channelObj.hideFlags = CHANNEL_HIDE_FLAGS;
                channelObj.transform.SetParent(parent);
                channelObj.transform.Reset();
                channelObj.GetComponent<AudioSource>().playOnAwake = false;
                channel = channelObj.GetComponent<AudioChannel>();
            } 
            
            else
            {
                channel = channelObj.GetComponent<AudioChannel>();
                if (channel == null) channel = channelObj.AddComponent<AudioChannel>();
            }

            channel.SetParent(this);
            return channel;
        }

        /// <summary>
        /// Adds an element channel to this AudioGroup.
        /// </summary>
        public void AddElementChannel()
        {
            var channel = GenerateAudioChannel(ELEMENTS_PARENT_NAME + 
                elementChannelIndex++, elementParent.transform);
            channel.SetParent(this);
            elementChannels.Add(channel);
        }

        /// <summary>
        /// Removes an elementchannel from this AudioGroup.
        /// </summary>
        /// <param name="index"></param>
        public void RemoveElementChannel(int index)
        {
            elementChannels.RemoveAt(index);
        }

        /// <summary>
        /// Returns the longest clip length in this AudioGroup.
        /// </summary>
        public float GetClipLength()
        {
            return longestClipLength;
        }

        #endregion
        #region Private Methods

        /// <summary>
        /// Returns the current AudioListener in the scene.
        /// </summary>
        AudioListener AudioListener {
            get {
                if (al==null) {
                    al = FindObjectOfType<AudioListener>().GetComponent<AudioListener>();
                }
                return al;
            }
        }

        AudioChannel StandardChannel
        {
            get
            {
                if (standardChannel != null)
                    return standardChannel;

                standardChannel = gameObject.
                    FindInChildren(STANDARD_CHANNEL_NAME).
                    GetComponent<AudioChannel>();

                return standardChannel;
            }
        }

        List<AudioChannel> ElementChannels
        {
            get
            {
                if (elementChannels != null)
                    return elementChannels;

                elementChannels = 
                    gameObject.FindInChildren (ELEMENTS_PARENT_NAME).
                    GetComponentsInChildren<AudioChannel>().ToList<AudioChannel>();

                return elementChannels;
            }
        }

        /// <summary>
        /// Returns true if any audio channels are invalid.
        /// </summary>
        /// <returns></returns>
        bool InvalidAudioChannels()
        {
            return standardChannel == null || bassChannel == null ||
                midChannel == null || trebleChannel == null || elementParent == null;
        }

        /// <summary>
        /// Calculates the spatialized (3D) volume of this AudioGroup.
        /// </summary>
        float CalculateSpatializedVolumeScale ()
        {
            return spatialBlend * CalculateRolloff();
        }

        /// <summary>
        /// Calculates the 3D rolloff factor of this AudioGroup.
        /// </summary>
        float CalculateRolloff ()
        {
            float d = Vector3.Distance (AudioListener.transform.position, transform.position);
            float min = 0.01f;
            float max = maxDistance;
            float factor = 1f - (d - min) / (max - min);
            return Mathf.Clamp01(factor);
        }

        #endregion
        #region Public Structures

        /// <summary>
        /// Enum for type of AudioGroup.
        /// </summary>
        public enum GroupType
        {
            Standard = 0, // Single channel
            Layered  = 1, // Bass/mid/treble channels
            Elements = 2  // Per-element channels
        }

        #endregion
    }
}
