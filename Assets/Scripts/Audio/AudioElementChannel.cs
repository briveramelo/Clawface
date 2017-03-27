// AudioElementChannel.cs

using System.Collections.Generic;
using UnityEngine;
using ModMan;

/// <summary>
/// Audio class for an individual channel attached to an AudioElement.
/// </summary>
[System.Serializable]
[ExecuteInEditMode]
[RequireComponent(typeof(AudioSource))]
public class AudioElementChannel : MonoBehaviour {

    #region Vars

    /// <summary>
    /// AudioSource to play from (should be attached to gameObject).
    /// </summary>
    AudioSource _audioSource;

    /// <summary>
    /// AudioElement that uses this AudioElementChannel.
    /// </summary>
    AudioElement _parent;

    
    bool _loop = false;
    bool _playing = false;

    
    [SerializeField] List<AudioClip> _clips = new List<AudioClip>();

    /// <summary>
    /// Change volume each loop (when using randomized volume)?
    /// </summary>
    [SerializeField] bool _changeVolumeEachLoop = false;

    [SerializeField][Range(0f, 1f)]
    float _volume = 1f;
    [SerializeField] bool _randomVolume = false;
    [SerializeField] [FloatRange (0f, 1f)]
    FloatRange _volumeRange = new FloatRange();

    float _loopTimer;

    #endregion
    #region Unity Callbacks

    private void OnEnable() {
        _audioSource = GetComponent<AudioSource>();
        if (_parent == null)
            _parent = GetComponentInParent<AudioElement>();
    }

    private void Update() {
        if (_loop && _playing) {
            _loopTimer -= Time.deltaTime;
            if (_loopTimer <= 0f) {
                LoopSound();
            }
        }
    }

    #endregion
    #region Properties

    public List<AudioClip> Clips { get { return _clips; } }

    public float Volume {
        get { return _volume; }
        set { _volume = value; }
    }

    public FloatRange VolumeRange {
        get { return _volumeRange; }
        set { _volumeRange = value; }
    }

    public bool UseRandomVolume {
        get { return _randomVolume; }
        set { _randomVolume = value; }
    }

    public bool ChangeVolumeEachLoop {
        get { return _changeVolumeEachLoop; }
        set { _changeVolumeEachLoop = value; }
    }

    #endregion
    #region Methods

    public void SetParent (AudioElement parent) {
        _parent = parent;
    }

    public void PlaySound (float pitch, bool loop=false) {
        if (_clips.Count <= 0) {
            //Debug.LogError (string.Format("No audio clips given! {0}.{1}", _parent.name, name));
            return;
        }

        _loop = loop;
        _audioSource.volume = _randomVolume ? _volumeRange.GetRandomValue() : _volume;
        _audioSource.pitch = pitch;

        var clip = _clips.GetRandom();
        _audioSource.PlayOneShot (clip);

        if (_loop) {
            _loopTimer = clip.length;
            _playing = true;
        }
    }

    void LoopSound () {
        var clip = _clips.GetRandom();
        _audioSource.PlayOneShot (clip);
        _loopTimer = clip.length;
    }

    public void Stop () {
        _audioSource.Stop();
        _playing = false;
    }

    #endregion
}
