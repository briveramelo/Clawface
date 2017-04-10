using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using ModMan;

namespace Turing.Audio {

    [ExecuteInEditMode]
    public class MusicGroup : MonoBehaviour {

        const string _INSTRUMENTS_PARENT_NAME = "INSTRUMENTS";
        const HideFlags _CHANNEL_HIDE_FLAGS = HideFlags.None;//HideFlags.HideInHierarchy;

        [SerializeField] float _bpm;
        float _beatTimer;

        [SerializeField] bool _playOnAwake = false;

        float _playbackTime;

        [SerializeField] Transform _instrumentParent;

        [SerializeField] List<MusicChannel> _instrumentChannels = new List<MusicChannel>();

        [SerializeField]
        int _instrumentChannelIndex = 0;

        [SerializeField]
        public UnityEvent onBeat = new UnityEvent();

        bool _playing = false;

        float _volumeScale = 1f;

        public bool IsPlaying { get { return _playing; } }

        public float VolumeScale { get { return _volumeScale; } }

        private void OnEnable() {
            if (!Application.isPlaying) {
                var instruments = GetComponentsInChildren<MusicChannel>();
                if (instruments == null || InvalidAudioChannels()) {
                    GenerateMusicChannels();
                }
            } else {
                
            }
        }

        private void Awake() {
            if (Application.isPlaying && _playOnAwake) Play();
        }

        private void FixedUpdate() {
            if (_playing) {
                _playbackTime += Time.fixedDeltaTime;
                if (_beatTimer <= 0f) {
                    onBeat.Invoke();
                    _beatTimer = 60f / _bpm;
                } else _beatTimer -= Time.fixedDeltaTime;
            }
        }

        public void Play() {
            _playing = true;
            _playbackTime = 0f;

            foreach (var instrument in _instrumentChannels) {
                instrument.SetBPM (_bpm);
                instrument.PlayTrack(false);
            }

            _beatTimer = 60f / _bpm - Time.fixedDeltaTime;
        }

        public void Stop() {
            _playing = false;

            foreach (var instrument in _instrumentChannels)
                instrument.Stop();
        }

        protected void SetVolumeScale(float volumeScale) {
            foreach (var instrument in _instrumentChannels)
                instrument.VolumeScale = volumeScale;
        }

        void GenerateMusicChannels () {
            if (_instrumentParent == null) {
                _instrumentParent = new GameObject (_INSTRUMENTS_PARENT_NAME).transform;
                _instrumentParent.hideFlags = _CHANNEL_HIDE_FLAGS;
                _instrumentParent.SetParent(transform);
                _instrumentParent.Reset();
            }
        }

        MusicChannel GenerateMusicChannel (string channelName, Transform parent) {
            var channelObj = gameObject.FindInChildren (channelName);
            MusicChannel channel;
            if (channelObj == null) {
                channelObj = new GameObject (
                    channelName,
                    typeof(AudioSource),
                    typeof(MusicChannel)
                    );
                channelObj.hideFlags = _CHANNEL_HIDE_FLAGS;
                channelObj.transform.SetParent(parent);
                channelObj.transform.Reset();
                channel = channelObj.GetComponent<MusicChannel>();
            } else {
                channel = channelObj.GetComponent<MusicChannel>();
                if (channel == null) channel = channelObj.AddComponent<MusicChannel>();
            }
            channel.SetParent (this);
            return channel;
        }

        public void AddInstrumentChannel () {
            var channel = GenerateMusicChannel (_INSTRUMENTS_PARENT_NAME + _instrumentChannelIndex++, _instrumentParent.transform);
            channel.SetParent (this);
            _instrumentChannels.Add (channel);
        }

        public void RemoveInstrumentChannel (int index) {
            _instrumentChannels.RemoveAt (index);
        }

        protected bool InvalidAudioChannels() {
            return _instrumentParent == null;
        }

    }
}