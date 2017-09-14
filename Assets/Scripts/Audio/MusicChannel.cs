// MusicChannel.cs
// Author: Aaron

using ModMan;

using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace Turing.Audio
{
    [Serializable]
    [ExecuteInEditMode]
    public sealed class MusicChannel : MonoBehaviour
    {
        #region Serialized Unity Inspector Fields

        /// <summary>
        /// The AudioSources attached to this MusicChannel.
        /// </summary>
        [SerializeField]
        [Tooltip("The AudioSources attached to this MusicChannel.")]
        AudioSource[] audioSources = new AudioSource[2];

        /// <summary>
        /// The current volume of this MusicChannel.
        /// </summary>
        [SerializeField][Range(0f, 1f)]
        [Tooltip("The current volume of this MusicChannel.")]
        float volume = 1f;

        /// <summary>
        /// All clips in this MusicChannel.
        /// </summary>
        [SerializeField]
        [Tooltip("All clips in this MusicChannel.")]
        List<ClipInfo> clips = new List<ClipInfo>();

        #endregion
        #region Private Fields

        /// <summary>
        /// The current volume scaler of this MusicChannel.
        /// </summary>
        float volumeScale = 1f;

        /// <summary>
        /// Number of beats left in playback.
        /// </summary>
        int beatsLeft;

        /// <summary>
        /// Is this MusicChannel playing?
        /// </summary>
        bool playing = false;

        /// <summary>
        /// Current clip being played.
        /// </summary>
        ClipInfo currentClip;

        /// <summary>
        /// Next clip to be played.
        /// </summary>
        ClipInfo nextClip;

        /// <summary>
        /// AudioSource index to use for next playback.
        /// </summary>
        int audioSourceToPlay = 0;

        /// <summary>
        /// Beats per minute in playback.
        /// </summary>
        float bpm;

        #endregion
        #region Unity Lifecycle

        private void Awake()
        {
            audioSources = GetComponentsInChildren<AudioSource>();
            if (audioSources.Length < 2) audioSources = new AudioSource[2];
            if (audioSources[0] == null) audioSources[0] = 
                    gameObject.AddComponent<AudioSource>();
            if (audioSources[1] == null) audioSources[1] = 
                    gameObject.AddComponent<AudioSource>();

            foreach (var a in audioSources)
            {
                a.loop = false;
                a.playOnAwake = false;
                a.volume = volume;
            }
        }

        #endregion
        #region Public Methods

        /// <summary>
        /// Returns a list of clip info for this channel (read-only).
        /// </summary>
        public List<ClipInfo> Clips { get { return clips; } }

        /// <summary>
        /// Gets/sets the volume scale for this channel.
        /// </summary>
        public float VolumeScale
        {
            get { return volumeScale; }
            set { volumeScale = value; }
        }

        /// <summary>
        /// Sets the beats per minute of music playback.
        /// </summary>
        public void SetBPM(float bpm) { this.bpm = bpm; }

        /// <summary>
        /// Plays this MusicChannel.
        /// </summary>
        /// <param name="isLoop"></param>
        public void PlayTrack(bool isLoop=true)
        {
            if (clips.Count <= 0) return;

            StartCoroutine(MusicLoop());
        }

        /// <summary>
        /// Stops playback.
        /// </summary>
        public void Stop()
        {
            StopAllCoroutines();
            audioSources[0].Stop();
            audioSources[1].Stop();
            playing = false;
        }

        /// <summary>
        /// Adds a clip to this channel.
        /// </summary>
        public void AddClip(AudioClip clip)
        {
            clips.Add(new ClipInfo(clip));
        }

        #endregion
        #region Private Methods

        /// <summary>
        /// Coroutine for music playback.
        /// </summary>
        IEnumerator MusicLoop()
        {
            while (true)
            {
                audioSourceToPlay = 1 - audioSourceToPlay;
                var audioSource = audioSources[audioSourceToPlay];
                audioSource.volume = volume * volumeScale;

                if (!playing)
                {
                    currentClip = clips.GetRandom();
                    audioSource.clip = currentClip.Clip;
                    audioSource.volume = volume * volumeScale;
                    audioSource.Play();
                    playing = true;
                } 
                
                else
                {
                    currentClip = nextClip;
                }

                nextClip = clips.GetRandom();
                double dt = 60f / bpm * currentClip.Beats;
                //Debug.Log (dt);
                var otherAudioSource = audioSources[1 - audioSourceToPlay];

                otherAudioSource.clip = nextClip.Clip;
                otherAudioSource.volume = volumeScale * volume;
                otherAudioSource.PlayScheduled(AudioSettings.dspTime + dt);

                yield return new WaitForSecondsRealtime((float)dt);
            }
        }

        #endregion
        #region Public Structures

        /// <summary>
        /// Class to store clip info.
        /// </summary>
        [Serializable]
        public class ClipInfo
        {
            /// <summary>
            /// AudioClip to use.
            /// </summary>
            [SerializeField] AudioClip clip;
            
            /// <summary>
            /// Number of bars in the clip.
            /// </summary>
            [SerializeField] int bars = 4;

            /// <summary>
            /// AudioClip constructor.
            /// </summary>
            public ClipInfo(AudioClip clip)
            {
                this.clip = clip;
            }

            /// <summary>
            /// Gets/sets the number of bars in this clip.
            /// </summary>
            public int Bars
            {
                get { return bars; }
                set { bars = value; }
            }

            /// <summary>
            /// Gets/sets the referenced AudioClip.
            /// </summary>
            public AudioClip Clip
            {
                get { return clip; }
                set { clip = value; }
            }

            /// <summary>
            /// Returns the number of beats in this clip.
            /// </summary>
            public int Beats { get { return bars * 4; } }
        }

        #endregion
    }
}
