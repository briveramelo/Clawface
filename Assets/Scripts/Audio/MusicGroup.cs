// MusicGroup.cs
// Author: Aaron

using ModMan;

using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

namespace Turing.Audio
{
    /// <summary>
    /// Class to emulate FMOD-style music playback
    /// </summary>
    [ExecuteInEditMode]
    public sealed class MusicGroup : MonoBehaviour
    {
        #region Public Fields

        /// <summary>
        /// Invoked on a music beat.
        /// </summary>
        [HideInInspector]
        public UnityEvent onBeat = new UnityEvent();

        #endregion
        #region Serialized Unity Inspector Fields

        /// <summary>
        /// Beats per minute in this song.
        /// </summary>
        [Tooltip("Beats per minute in this song.")]
        [SerializeField] float bpm;

        /// <summary>
        /// Should this MusicGroup play on awake?
        /// </summary>
        [Tooltip("Should this MusicGroup play on awake?")]
        [SerializeField] bool playOnAwake = false;

        /// <summary>
        /// Transform to which to parent child MusicChannels.
        /// </summary>
        [Tooltip("Transform to which to parent child MusicChannels.")]
        [SerializeField] Transform instrumentParent;

        /// <summary>
        /// List of all child MusicChannels.
        /// </summary>
        [Tooltip("List of all child MusicChannels.")]
        [SerializeField] List<MusicChannel> instrumentChannels = 
            new List<MusicChannel>();

        #endregion
        #region Private Fields

        const string _INSTRUMENTS_PARENT_NAME = "INSTRUMENTS";
        const HideFlags _CHANNEL_HIDE_FLAGS = HideFlags.None;

        /// <summary>
        /// Time until next beat (seconds).
        /// </summary>
        float beatTimer;

        /// <summary>
        /// Current playback time (seconds).
        /// </summary>
        float playbackTime;

        /// <summary>
        /// Current instrument channel index (for naming).
        /// </summary>
        int instrumentChannelIndex = 0;

        /// <summary>
        /// Is this MusicGroup currently playing?
        /// </summary>
        bool playing = false;

        /// <summary>
        /// Current volume scale of this MusicGroup.
        /// </summary>
        float volumeScale = 1f;

        #endregion
        #region Unity Lifecycle

        private void OnEnable()
        {
            if (!Application.isPlaying)
            {
                var instruments = GetComponentsInChildren<MusicChannel>();
                if (instruments == null || InvalidMusicChannels())
                {
                    GenerateMusicChannels();
                }
            }
        }

        private void Awake()
        {
            if (Application.isPlaying && playOnAwake) Play();
        }

        private void FixedUpdate()
        {
            if (playing)
            {
                playbackTime += Time.fixedDeltaTime;
                if (beatTimer <= 0f)
                {
                    onBeat.Invoke();
                    beatTimer = 60f / bpm;
                } 
                
                else beatTimer -= Time.fixedDeltaTime;
            }
        }

        #endregion
        #region Public Methods

        /// <summary>
        /// Returns true if this MusicGroup is playing (read-only).
        /// </summary>
        public bool IsPlaying { get { return playing; } }

        /// <summary>
        /// Returns the current voluem scale (read-only).
        /// </summary>
        public float VolumeScale { get { return volumeScale; } }

        /// <summary>
        /// Plays this MusicGroup.
        /// </summary>
        public void Play()
        {
            playing = true;
            playbackTime = 0f;

            foreach (var instrument in instrumentChannels)
            {
                instrument.SetBPM (bpm);
                instrument.PlayTrack(false);
            }

            beatTimer = 60f / bpm - Time.fixedDeltaTime;
        }

        /// <summary>
        /// Stops playback.
        /// </summary>
        public void Stop()
        {
            playing = false;

            foreach (var instrument in instrumentChannels)
                instrument.Stop();
        }

        /// <summary>
        /// Sets the volume scale of this MusicGroup.
        /// </summary>
        public void SetVolumeScale(float volumeScale)
        {
            foreach (var instrument in instrumentChannels)
                instrument.VolumeScale = volumeScale;
        }

        /// <summary>
        /// Adds a new MusicChannel to this MusicGroup.
        /// </summary>
        public void AddInstrumentChannel ()
        {
            var channel = GenerateMusicChannel (_INSTRUMENTS_PARENT_NAME + 
                instrumentChannelIndex++, instrumentParent.transform);
            instrumentChannels.Add (channel);
        }

        /// <summary>
        /// Removes the MusicChannel at the given index.
        /// </summary>
        /// <param name="index"></param>
        public void RemoveInstrumentChannel (int index)
        {
            instrumentChannels.RemoveAt (index);
        }

        #endregion
        #region Private Methods

        /// <summary>
        /// Generates blank MusicChannels for this MusicGroup.
        /// </summary>
        void GenerateMusicChannels ()
        {
            if (instrumentParent == null)
            {
                instrumentParent = new GameObject (_INSTRUMENTS_PARENT_NAME).transform;
                instrumentParent.hideFlags = _CHANNEL_HIDE_FLAGS;
                instrumentParent.SetParent(transform);
                instrumentParent.Reset();
            }
        }

        /// <summary>
        /// Generates an empty MusicGhannel.
        /// </summary>
        MusicChannel GenerateMusicChannel (string channelName, Transform parent)
        {
            var channelObj = gameObject.FindInChildren (channelName);
            MusicChannel channel;
            if (channelObj == null)
            {
                channelObj = new GameObject (
                    channelName,
                    typeof(AudioSource),
                    typeof(MusicChannel)
                    );
                channelObj.hideFlags = _CHANNEL_HIDE_FLAGS;
                channelObj.transform.SetParent(parent);
                channelObj.transform.Reset();
                channel = channelObj.GetComponent<MusicChannel>();
            } 
            
            else
            {
                channel = channelObj.GetComponent<MusicChannel>();
                if (channel == null) channel = channelObj.AddComponent<MusicChannel>();
            }
            return channel;
        }

        /// <summary>
        /// Returns true if anything is wrong with the MusicChannels.
        /// </summary>
        /// <returns></returns>
        bool InvalidMusicChannels()
        {
            return instrumentParent == null;
        }

        #endregion
    }
}