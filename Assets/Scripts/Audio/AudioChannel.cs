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
        /// Randomize volume on each playback?
        /// </summary>
        [SerializeField]
        [Tooltip("Randomize volume on each playback?")]
        bool changeVolumeEachLoop = false;

        /// <summary>
        /// Uniform volume to use with this channel.
        /// </summary>
        [Tooltip("Uniform volume to use with this channel.")]
        [SerializeField][Range(0f, 1f)]
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
        [SerializeField][FixedFloatRange(0f, 1.0f)]
        [Tooltip("Range of values to use for volume.")]
        FloatRange randomVolumeRange = new FloatRange();

        #endregion
        #region Private Fields

        /// <summary>
        /// Parent AudioGroup;
        /// </summary>
        AudioGroup parent;

        /// <summary>
        /// The length of the current audio clip.
        /// </summary>
        float clipLength;

        /// <summary>
        /// Timer for playback looping.
        /// </summary>
        float loopTimer;

        #endregion
        #region Public Methods

        /// <summary>
        /// Returns the parent AudioGroup of this AudioChannel.
        /// </summary>
        public AudioGroup Parent
        {
            get
            {
                if (!parent)
                    parent = GetComponent<AudioGroup>();
                return parent;
            }
        }

        /// <summary>
        /// Returns all AudioClips used in this AudioChannel (read-only).
        /// </summary>
        public List<AudioClip> Clips { get { return clips; } }

        /// <summary>
        /// Returns the length in seconds of the current clip (read-only).
        /// </summary>
        public float ClipLength { get { return clipLength; } }

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
        /// Returns a volume value to use for this channel.
        /// </summary>
        public float GetVolume ()
        {
            if (useRandomVolume) return randomVolumeRange.GetRandomValue();
            else return uniformVolume;
        }

        /// <summary>
        /// Returns a random AudioClip from this channel.
        /// </summary>
        public AudioClip GetRandomClip ()
        {
            if (clips.Count == 0)
            {
                Debug.LogWarning ("No clips specified for this AudioChannel!",
                    gameObject);
                return null;
            }
            return clips.GetRandom();
        }

        /// <summary>
        /// Adds an AudioClip to this channel.
        /// </summary>
        public void AddClip(AudioClip clip) { clips.Add(clip); }

        #endregion
    }
}
