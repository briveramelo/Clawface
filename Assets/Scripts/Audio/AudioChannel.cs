﻿// AudioChannel.cs

using System.Collections.Generic;
using UnityEngine;
using ModMan;

namespace Turing.Audio {

    /// <summary>
    /// Audio class for an individual channel attached to an AudioGroup.
    /// </summary>
    [System.Serializable]
    [ExecuteInEditMode]
    public class AudioChannel : MonoBehaviour {

        #region Vars

        /// <summary>
        /// AudioSource to play from (should be attached to gameObject).
        /// </summary>
        AudioSource _audioSource;

        /// <summary>
        /// AudioElement that uses this AudioElementChannel.
        /// </summary>
        AudioGroup _parent;

        /// <summary>
        /// Will this AudioChannel loop?
        /// </summary>
        bool _loop = false;

        /// <summary>
        /// Is this AudioChannel playing?
        /// </summary>
        bool _playing = false;

        /// <summary>
        /// List of AudioClips to play in this channel.
        /// </summary>
        [SerializeField]
        [Tooltip("List of AudioClips to play in this channel.")]
        List<AudioClip> _clips = new List<AudioClip>();

        float _clipLength;

        /// <summary>
        /// Change volume each loop (when using randomized volume)?
        /// </summary>
        [SerializeField]
        [Tooltip("Randomize volume on each loop?")]
        bool _changeVolumeEachLoop = false;

        /// <summary>
        /// Volume scale (from parent AudioGroup).
        /// </summary>
        float _volumeScale = 1f;

        /// <summary>
        /// Uniform volume to use with this channel.
        /// </summary>
        [SerializeField]
        [Range(0f, 1f)]
        [Tooltip("Uniform volume to use with this channel.")]
        float _uniformVolume = 1f;

        float _randomizedVolume = 1f;

        /// <summary>
        /// Randomize volume?
        /// </summary>
        [SerializeField]
        [Tooltip("Randomize volume?")]
        bool _useRandomVolume = false;

        /// <summary>
        /// Range of values to use for volume.
        /// </summary>
        [SerializeField]
        [FloatRange(0f, 1f)]
        [Tooltip("Range of values to use for volume.")]
        FloatRange _randomVolumeRange = new FloatRange();

        /// <summary>
        /// Timer for playback looping.
        /// </summary>
        float _loopTimer;

        #endregion
        #region Unity Callbacks

        private void OnEnable() {
            _audioSource = GetComponentInParent<AudioSource>();
            if (_parent == null)
                _parent = GetComponentInParent<AudioGroup>();
        }

        private void Update() {
            if (_playing) {
                if (_loop) {
                    _loopTimer -= Time.deltaTime;
                    if (_loopTimer <= 0f)
                        LoopSound();
                 }

                _audioSource.volume = (_useRandomVolume ? _randomizedVolume : _uniformVolume) * _volumeScale;
            }
            
        }

        #endregion
        #region Properties

        /// <summary>
        /// Returns this AudioChannel's parent AudioGroup (read-only).
        /// </summary>
        public AudioGroup Parent { get { return _parent; } }

        /// <summary>
        /// Returns all AudioClips used in this AudioChannel (read-only).
        /// </summary>
        public List<AudioClip> Clips { get { return _clips; } }

        public float ClipLength { get { return _clipLength; } }

        public float VolumeScale {
            get { return _volumeScale; }
            set { _volumeScale = value; }
        }

        /// <summary>
        /// Gets/sets the uniform volume to use.
        /// </summary>
        public float UniformVolume {
            get { return _uniformVolume; }
            set { _uniformVolume = value; }
        }

        /// <summary>
        /// Gets/sets the range of volumes to use.
        /// </summary>
        public FloatRange RandomVolumeRange {
            get { return _randomVolumeRange; }
            set { _randomVolumeRange = value; }
        }

        /// <summary>
        /// Gets/sets whether or not to randomize volume.
        /// </summary>
        public bool UseRandomVolume {
            get { return _useRandomVolume; }
            set { _useRandomVolume = value; }
        }

        /// <summary>
        /// Gets/sets whether or not to randomize volume on each loop.
        /// </summary>
        public bool ChangeVolumeEachLoop {
            get { return _changeVolumeEachLoop; }
            set { _changeVolumeEachLoop = value; }
        }

        #endregion
        #region Methods

        /// <summary>
        /// Sets the parent AudioGroup of this AudioChannel.
        /// </summary>
        /// <param name="parent"></param>
        public void SetParent(AudioGroup parent) {
            _parent = parent;
        }

        /// <summary>
        /// Plays a sound from this channel.
        /// </summary>
        public void PlaySound(float pitch, bool loop = false) {
            if (_clips.Count <= 0) return;

            _loop = loop;
            if (_useRandomVolume) _randomizedVolume = _randomVolumeRange.GetRandomValue();
            _audioSource.volume = (_useRandomVolume ? _randomizedVolume : _uniformVolume) * _volumeScale;
            _audioSource.pitch = pitch;

            var clip = _clips.GetRandom();
            _clipLength = clip.length;
            _audioSource.PlayOneShot(clip);

            _playing = true;

            if (_loop) _loopTimer = clip.length;
        }

        /// <summary>
        /// Loops the sound.
        /// </summary>
        void LoopSound() {
            var clip = _clips.GetRandom();
            _audioSource.PlayOneShot(clip);
            _loopTimer = clip.length;
        }

        /// <summary>
        /// Stops this AudioChannel's playback.
        /// </summary>
        public void Stop() {
            _audioSource.Stop();
            _playing = false;
        }

        #endregion
    }
}