using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ModMan;

namespace Turing.Audio {

    [System.Serializable]
    [ExecuteInEditMode]
    [AddComponentMenu("")]
    [RequireComponent(typeof(AudioSource))]
    public class MusicChannel : MonoBehaviour {

        [SerializeField]
        AudioSource _audioSource;

        [Range(0f, 1f)]
        [SerializeField] float _volume = 1f;

        float _volumeScale =1f;

        [SerializeField]
        MusicGroup _parent;

        [SerializeField]
        List<ClipInfo> _clips = new List<ClipInfo>();

        int _beatsLeft;

        bool _playing = false;

        private void Awake() {
            if (_parent == null) _parent = GetComponentInParent<MusicGroup>();
            _parent.onBeat.AddListener(CheckForLoop);

            if (_audioSource == null) _audioSource = GetComponent<AudioSource>();
        }

        public List<ClipInfo> Clips { get { return _clips; } }

        public float VolumeScale {
            get { return _volumeScale; }
            set { _volumeScale = value; }
        }

        public void PlayTrack () {
            if (_clips.Count <= 0) return;
            if (_audioSource == null)
                _audioSource = GetComponent<AudioSource>();

            _audioSource.volume = _volume;

            var clip = _clips.GetRandom();
            _beatsLeft = clip.Beats;
            _audioSource.PlayOneShot(clip.Clip);

            Debug.Log (_beatsLeft / 126f * 60f);
            Debug.Log (clip.Clip.length);

            _playing = true;
        }

        public void Stop () {
            _audioSource.Stop();
            _playing = false;
        }

        public void SetParent(MusicGroup parent) {
            _parent = parent;
            parent.onBeat.AddListener(CheckForLoop);
        }

        public void AddClip (AudioClip clip) {
            _clips.Add(new ClipInfo (clip));
        }

        public void CheckForLoop () {
            if (_beatsLeft-- == 0) PlayTrack();
        }

        [System.Serializable]
        public class ClipInfo {
            [SerializeField] AudioClip _clip;
            [SerializeField] int _bars = 4;

            public ClipInfo (AudioClip clip) {
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
