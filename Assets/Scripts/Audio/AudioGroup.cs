// AudioGroup.cs
// Author: Aaron

using ModMan;

using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.Audio;

namespace Turing.Audio
{
    /// <summary>
    /// Audio class to emulate FMOD-style functionality.
    /// </summary>
    [ExecuteInEditMode]
    [RequireComponent(typeof(AudioSource))]
    public sealed class AudioGroup : MonoBehaviour
    {
        #region Serialized Unity Inspector Fields

        /// <summary>
        /// List of AudioChannels.
        /// </summary>
        [SerializeField]
        [Tooltip("List of AudioChannels.")]
        List<AudioChannel> channels;

        /// <summary>
        /// Will playback loop?
        /// </summary>
        [SerializeField]
        [Tooltip("Will playback loop?")]
        bool loop = false;

        /// <summary>
        /// Should this AudioGroup play on start?
        /// </summary>
        [SerializeField]
        [Tooltip("Should this AudioGroup play on start?")]
        bool playOnStart = false;
        
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
        /// Change volume each loop (when using randomized volume)?
        /// </summary>
        [SerializeField]
        [Tooltip("Randomize volume on each loop?")]
        bool changeVolumeEachLoop = false;

        /// <summary>
        /// Uniform volume to use with this AudioGroup.
        /// </summary>
        [SerializeField][Range(0f, 1f)]
        [Tooltip("Uniform volume to use with this AudioGroup.")]
        float uniformVolume = 1f;

        /// <summary>
        /// Randomize volume?
        /// </summary>
        [SerializeField]
        [Tooltip("Randomize volume?")]
        bool useRandomVolume = false;

        /// <summary>
        /// Range of values to use for volume.
        /// </summary>
        [SerializeField][FixedFloatRange(0f, 1f)]
        [Tooltip("Range of values to use for volume.")]
        FloatRange randomVolumeRange = new FloatRange();

        /// <summary>
        /// Randomize pitch on each playback?
        /// </summary>
        [SerializeField]
        [Tooltip("Randomize pitch on each playback?")]
        bool randomPitch = false;

        /// <summary>
        /// Change pitch each loop (when using randomized pitch)?
        /// </summary>
        [SerializeField]
        [Tooltip("Change pitch each loop (when using randomized pitch)?")]
        bool changePitchEachLoop = false;

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

        #endregion
        #region Private Fields

        /// <summary>
        /// The AudioSource attached to this AudioGroup.
        /// </summary>
        AudioSource audioSource;

        /// <summary>
        /// The active AudioListener.
        /// </summary>
        AudioListener audioListener;

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

        static Camera mainCamera;

        #endregion
        #region Unity Lifecycle

        private void OnEnable()
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
                audioSource = gameObject.AddComponent<AudioSource>();
            channels = GetComponents<AudioChannel>().
                ToList<AudioChannel>();
        }

        private void Start()
        {
            if (Application.isPlaying && playOnStart) Play();
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
            get
            {
                return (1f - spatialBlend) + 
                    CalculateSpatializedVolumeScale();
            }
        }

        public void AddChannel ()
        {
            channels.Add (gameObject.AddComponent<AudioChannel>());
        }

        public void RemoveChannel (int i)
        {
            DestroyImmediate (channels[i]);
            channels.RemoveAt(i);
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

            audioSource.pitch = pitch;

            longestClipLength = 0f;
            foreach (AudioChannel channel in channels)
            {
                float clipLength;
                PlayChannel (channel, out clipLength);
                if (clipLength > longestClipLength)
                    longestClipLength = channel.ClipLength;
            }
        }

        /// <summary>
        /// Plays the specified AudioChannel.
        /// </summary>
        public void PlayChannel (AudioChannel channel, out float clipLength)
        {
            AudioClip clip = channel.GetRandomClip();
            if (clip == null)
            {
                clipLength = float.NaN;
                return;
            }
            clipLength = clip.length;
            if (audioSource == null)
            {
                audioSource = GetComponent<AudioSource>();
                if (audioSource == null)
                    audioSource = gameObject.AddComponent<AudioSource>();
            }

            if (Application.isPlaying)
            {
                audioSource.panStereo = Mathf.Lerp(0.0f, GetPanPosition(), spatialBlend);
                float threeDVolume = channel.GetVolume() * Mathf.Sqrt(1.0f - Mathf.Abs(audioSource.panStereo));
                audioSource.volume = Mathf.Lerp(channel.GetVolume() * uniformVolume, threeDVolume, spatialBlend);
            }
            
            audioSource.PlayOneShot (clip, 
                channel.GetVolume() * uniformVolume);
        }

        float GetPanPosition ()
        {
            if (mainCamera == null || !mainCamera.isActiveAndEnabled)
                mainCamera = Camera.main;

            Vector3 screenPos = mainCamera.WorldToViewportPoint(transform.position);
            screenPos = screenPos - Vector3.one + 2.0f * screenPos;
            return screenPos.x / 3.0f;
        }

        public void SetMixer(AudioMixer i_mixer)
        {
            //mixer.FindMatchingGroups("Master")[0]

            //audioSource.outputAudioMixerGroup = i_mixer.outputAudioMixerGroup;
            //audioSource.outputAudioMixerGroup = i_mixer.FindMatchingGroups("Master")[0];
        }

        /// <summary>
        /// Stops all playback on this AudioElement.
        /// </summary>
        public void Stop()
        {
            playing = false;
            audioSource.Stop();
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
        AudioListener AudioListener
        {
            get
            {
                if (!audioListener)
                    audioListener = 
                        FindObjectOfType<AudioListener>().
                        GetComponent<AudioListener>();

                return audioListener;
            }
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
    }
}
