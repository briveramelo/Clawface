// AudioChannel.cs
// Author: Aaron

using ModMan;

using System;
using System.Collections.Generic;

using UnityEngine;

namespace Turing.Audio
{
    /// <summary>
    /// Audio class for an individual channel attached to an AudioGroup.
    /// </summary>
    [Serializable]
    [ExecuteInEditMode]
    public sealed class AudioChannel : MonoBehaviour
    {
        #region Serialized Unity Inspector Fields

        /// <summary>
        /// List of AudioClips to play in this channel.
        /// </summary>
        [SerializeField]
        [Tooltip("List of AudioClips to play in this channel.")]
        List<AudioClip> clips = new List<AudioClip>();

        /// <summary>
        /// Change volume each loop (when using randomized volume)?
        /// </summary>
        [SerializeField]
        [Tooltip("Randomize volume on each loop?")]
        bool changeVolumeEachLoop = false;

        /// <summary>
        /// Uniform volume to use with this channel.
        /// </summary>
        [SerializeField]
        [Range(0f, 1f)]
        [Tooltip("Uniform volume to use with this channel.")]
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
        [SerializeField]
        [FixedFloatRange(0f, 1f)]
        [Tooltip("Range of values to use for volume.")]
        FloatRange randomVolumeRange = new FloatRange();

        #endregion
        #region Private Fields

        /// <summary>
        /// AudioSource to play from (should be attached to gameObject).
        /// </summary>
        AudioSource audioSource;

        /// <summary>
        /// AudioElement that uses this AudioElementChannel.
        /// </summary>
        AudioGroup parent;

        /// <summary>
        /// Will this AudioChannel loop?
        /// </summary>
        bool loop = false;

        /// <summary>
        /// Is this AudioChannel playing?
        /// </summary>
        bool playing = false;

        /// <summary>
        /// The length of the current audio clip.
        /// </summary>
        float clipLength;

        /// <summary>
        /// Volume scale (from parent AudioGroup).
        /// </summary>
        float volumeScale = 1f;

        /// <summary>
        /// Currently chosen randomized volume.
        /// </summary>
        float randomizedVolume = 1f;

        /// <summary>
        /// Timer for playback looping.
        /// </summary>
        float loopTimer;

        #endregion
        #region Unity Lifecycle

        void Awake()
        {
            audioSource = GetComponentInParent<AudioSource>();

            if (parent == null)
                parent = GetComponentInParent<AudioGroup>();
            if (parent == null)
                Debug.LogError("Failed to find parent AudioGroup!", gameObject);
        }

        void Update()
        {
            if (Application.isPlaying)
            {
                if (playing)
                {
                    if (loop)
                    {
                        loopTimer -= Time.deltaTime;
                        if (loopTimer <= 0f)
                            LoopSound();
                    }
                }

                audioSource.volume =
                    (useRandomVolume ? randomizedVolume : uniformVolume) *
                    parent.VolumeScale;
            }
        }

        #endregion
        #region Public Methods

        /// <summary>
        /// Returns this AudioChannel's parent AudioGroup (read-only).
        /// </summary>
        public AudioGroup Parent { get { return parent; } }

        /// <summary>
        /// Returns all AudioClips used in this AudioChannel (read-only).
        /// </summary>
        public List<AudioClip> Clips { get { return clips; } }

        /// <summary>
        /// Returns the length in seconds of the current clip (read-only).
        /// </summary>
        public float ClipLength { get { return clipLength; } }

        /// <summary>
        /// Gets/sets the volume scale value of this channel.
        /// </summary>
        public float VolumeScale
        {
            get { return volumeScale; }
            set { volumeScale = value; }
        }

        /// <summary>
        /// Gets/sets the uniform volume to use.
        /// </summary>
        public float UniformVolume
        {
            get { return uniformVolume; }
            set { uniformVolume = value; }
        }

        /// <summary>
        /// Gets/sets the range of volumes to use.
        /// </summary>
        public FloatRange RandomVolumeRange
        {
            get { return randomVolumeRange; }
            set { randomVolumeRange = value; }
        }

        /// <summary>
        /// Gets/sets whether or not to randomize volume.
        /// </summary>
        public bool UseRandomVolume
        {
            get { return useRandomVolume; }
            set { useRandomVolume = value; }
        }

        /// <summary>
        /// Gets/sets whether or not to randomize volume on each loop.
        /// </summary>
        public bool ChangeVolumeEachLoop
        {
            get { return changeVolumeEachLoop; }
            set { changeVolumeEachLoop = value; }
        }

        /// <summary>
        /// Sets the parent AudioGroup of this AudioChannel.
        /// </summary>
        /// <param name="parent"></param>
        public void SetParent(AudioGroup parent)
        {
            this.parent = parent;
        }

        /// <summary>
        /// Plays a sound from this channel.
        /// </summary>
        public void PlaySound(float pitch, bool loop = false)
        {
            // If no clips to play, return
            if (clips.Count <= 0) return;

            // If parent is null, attempt to find
            if (parent == null)
                parent = GetComponentInParent<AudioGroup>();

            volumeScale = parent.VolumeScale;

            this.loop = loop;

            if (useRandomVolume) randomizedVolume = randomVolumeRange.GetRandomValue();

            float twoDVolume = (useRandomVolume ? randomizedVolume : uniformVolume) * volumeScale;

            audioSource.volume = twoDVolume * volumeScale;
            audioSource.pitch = pitch;

            var clip = clips.GetRandom();
            if (clip)
            {
                clipLength = clip.length;
                audioSource.clip = clip;
                audioSource.Play();

                playing = true;

                if (loop) loopTimer = clip.length;
            }
        }

        /// <summary>
        /// Stops this AudioChannel's playback.
        /// </summary>
        public void Stop()
        {
            audioSource.Stop();
            playing = false;
        }

        /// <summary>
        /// Adds an AudioClip to this channel.
        /// </summary>
        public void AddClip(AudioClip clip)
        {
            clips.Add(clip);
        }

        #endregion
        #region Private Methods

        /// <summary>
        /// Loops the sound.
        /// </summary>
        void LoopSound()
        {
            var clip = clips.GetRandom();
            audioSource.PlayOneShot(clip);
            loopTimer = clip.length;
        }

        #endregion
    }
}
