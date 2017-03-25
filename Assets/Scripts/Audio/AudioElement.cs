using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ModMan;

[RequireComponent(typeof(AudioSource))]
public class AudioElement : MonoBehaviour {

    AudioSource _audioSource;

    [SerializeField] bool _loop = false;
    [SerializeField] bool _changeVolumeEachLoop = false;
    [SerializeField] bool _changePitchEachLoop = false;
    bool _playing = false;
    float _bassClipTimer = 0f;
    float _midClipTimer = 0f;
    float _trebleClipTimer = 0f;
    float _clipTimer = 0f;

	[SerializeField] bool _layered = false;
    
    [SerializeField] List<AudioClip> _bassAudioClips = new List<AudioClip>();
    [SerializeField] List<AudioClip> _midAudioClips = new List<AudioClip>();
    [SerializeField] List<AudioClip> _trebleAudioClips = new List<AudioClip>();

    [SerializeField] List<AudioClip> _audioClips = new List<AudioClip>();

    [SerializeField] bool _randomVolume = false;
    [SerializeField] float _volume = 1f;
    [SerializeField] [FloatRange (0f, 1f)] FloatRange _volumeRange;

    [SerializeField] bool _randomPitch = false;
    [SerializeField] float _pitch = 1f;
    [SerializeField] [FloatRange(-3f, 3f)] FloatRange _pitchRange;

    private void Update() {
        if (_loop && _playing) {
            if (_layered) {
                _bassClipTimer -= Time.deltaTime;
                if (_bassClipTimer <= 0f) LoopSound (_bassAudioClips, ref _bassClipTimer);

                _midClipTimer -= Time.deltaTime;
                if (_midClipTimer <= 0f) LoopSound (_midAudioClips, ref _midClipTimer);

                _trebleClipTimer -= Time.deltaTime;
                if (_trebleClipTimer <= 0f) LoopSound (_trebleAudioClips, ref _trebleClipTimer);
            } else {
                _clipTimer -= Time.deltaTime;
                if (_clipTimer <= 0f) LoopSound (_audioClips, ref _clipTimer);
            }
        }
    }

    public void Play () {
        Play (GetComponent<AudioSource>());
    }

    public void Play (AudioSource source) {
        _audioSource = source;
        source.volume = _randomVolume ? _volumeRange.GetRandomValue() : _volume;
        source.pitch = _randomPitch ? _pitchRange.GetRandomValue() : _pitch;
        if (_loop) _playing = true;

        // Play layered sound
        if (_layered) {
            var bassClip = _bassAudioClips.GetRandom();
            source.PlayOneShot (bassClip);

            var midClip = _midAudioClips.GetRandom();
            source.PlayOneShot (midClip);

            var trebleClip = _trebleAudioClips.GetRandom();
            source.PlayOneShot (trebleClip);

            if (_loop) {
                _bassClipTimer = bassClip.length;
                _midClipTimer = midClip.length;
                _trebleClipTimer = trebleClip.length;
            }

        // Play normal (unlayered) sound
        } else {
            var clip = _audioClips.GetRandom();
            source.PlayOneShot (clip);

            if (_loop) {
                _clipTimer = clip.length;
            }
        }
    } 

    public void Stop () {
        _playing = true;
    }

    void LoopSound (List<AudioClip> clips, ref float timer) {
        var clip = clips.GetRandom();
        _audioSource.PlayOneShot (clip);
        timer += clip.length;
    }
}
