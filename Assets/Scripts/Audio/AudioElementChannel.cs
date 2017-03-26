using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[ExecuteInEditMode]
[RequireComponent(typeof(AudioSource))]
public class AudioElementChannel : MonoBehaviour {

    AudioSource _audioSource;

    [SerializeField] bool _loop = false;
    bool _playing = false;

    [SerializeField] List<AudioClip> _clips;

    [SerializeField] float _volume = 1f;
    [SerializeField] bool _randomVolume = false;
    [SerializeField] [FloatRange (0f, 1f)]
    FloatRange _volumeRange = new FloatRange();

    float _loopTimer;

    private void OnEnable() {
        _audioSource = GetComponent<AudioSource>();
    }

    private void Update() {
        if (_loop && _playing) {

        }
    }

    public void PlaySound () {
        _audioSource.volume = _randomVolume ? _volumeRange.GetRandomValue() : _volume;
        _audioSource.pitch = _randomPitch ? _pitchRange.GetRandomValue() : _pitch;
    }
}
