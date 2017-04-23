using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ModMan;

namespace Turing.Audio {

    [System.Serializable]
    [ExecuteInEditMode]
    [AddComponentMenu("")]
    public class MusicChannel : MonoBehaviour {

        [SerializeField]
        AudioSource[] _audioSources = new AudioSource[2];

        [Range(0f, 1f)]
        [SerializeField]
        float _volume = 1f;

        float _volumeScale = 1f;

        //[SerializeField]
        //MusicGroup _parent;

        [SerializeField]
        List<ClipInfo> _clips = new List<ClipInfo>();

        int _beatsLeft;

        bool _playing = false;

        ClipInfo _currentClip;
        ClipInfo _nextClip;

        int _audioSourceToPlay = 0;

        float _bpm;

        private void Awake() {
            _audioSources = GetComponentsInChildren<AudioSource>();
            if (_audioSources.Length < 2) _audioSources = new AudioSource[2];
            if (_audioSources[0] == null) _audioSources[0] = gameObject.AddComponent<AudioSource>();
            if (_audioSources[1] == null) _audioSources[1] = gameObject.AddComponent<AudioSource>();

            foreach (var a in _audioSources) {
                a.loop = false;
                a.playOnAwake = false;
                a.volume = _volume;
            }
        }

        public List<ClipInfo> Clips { get { return _clips; } }

        public float VolumeScale {
            get { return _volumeScale; }
            set { _volumeScale = value; }
        }

        public void SetBPM(float bpm) { _bpm = bpm; }

        IEnumerator MusicLoop() {

            while (true) {
                //Debug.Log ("loop");

                _audioSourceToPlay = 1 - _audioSourceToPlay;
                var audioSource = _audioSources[_audioSourceToPlay];
                audioSource.volume = _volume * _volumeScale;

                if (!_playing) {
                    _currentClip = _clips.GetRandom();
                    audioSource.clip = _currentClip.Clip;
                    audioSource.volume = _volume * _volumeScale;
                    audioSource.Play();
                    _playing = true;
                } else {
                    _currentClip = _nextClip;
                }

                _nextClip = _clips.GetRandom();
                double dt = 60f / _bpm * _currentClip.Beats;
                //Debug.Log (dt);
                var otherAudioSource = _audioSources[1-_audioSourceToPlay];

                otherAudioSource.clip = _nextClip.Clip;
                otherAudioSource.volume = _volumeScale * _volume;
                otherAudioSource.PlayScheduled(AudioSettings.dspTime + dt);

                yield return new WaitForSecondsRealtime((float)dt);
            }
        }

        public void PlayTrack(bool isLoop = true) {
            if (_clips.Count <= 0) return;

            StartCoroutine(MusicLoop());
        }

        public void Stop() {
            StopAllCoroutines();
            _audioSources[0].Stop();
            _audioSources[1].Stop();
            _playing = false;
        }

        //public void SetParent(MusicGroup parent) {
            //_parent = parent;
        //}

        public void AddClip(AudioClip clip) {
            _clips.Add(new ClipInfo(clip));
        }

        [System.Serializable]
        public class ClipInfo {
            [SerializeField]
            AudioClip _clip;
            [SerializeField]
            int _bars = 4;

            public ClipInfo(AudioClip clip) {
                _clip = clip;
            }

            public int Bars {
                get { return _bars; }
                set { _bars = value; }
            }

            public AudioClip Clip {
                get { return _clip; }
                set { _clip = value; }
            }

            public int Beats { get { return _bars * 4; } }
        }
    }
}
