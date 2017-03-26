using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ModMan;

/// <summary>
/// Audio class to emulate FMOD-style functionality.
/// </summary>
[ExecuteInEditMode]
[RequireComponent(typeof(AudioSource), typeof(AudioElementChannel))]
public class AudioElement : MonoBehaviour {

    #region Consts

    const string _BASS_CHANNEL_NAME   = "BASS";
    const string _MID_CHANNEL_NAME    = "MID";
    const string _TREBLE_CHANNEL_NAME = "TREBLE";

    #endregion
    #region Vars

    /// <summary>
    /// Audio channel object for standard (non-layered) playback.
    /// </summary>
    {SerializeField] AudioElementChannel _standardChannel;

    /// <summary>
    /// Audio channel object for layered playback.
    /// </summary>
    [SerializeField] AudioElementChannel _bassChannel, _midChannel, _trebleChannel;

    /// <summary>
    /// Will playback loop?
    /// </summary>
    [SerializeField] bool _loop = false;

    /// <summary>
    /// Change volume each loop (when using randomized volume)?
    /// </summary>
    [SerializeField] bool _changeVolumeEachLoop = false;

    /// <summary>
    /// Change pitch each loop (when using randomized pitch)?
    /// </summary>
    [SerializeField] bool _changePitchEachLoop = false;

    /// <summary>
    /// If looped, is this AudioElement currently playing?
    /// </summary>
    bool _playing = false;

    /// <summary>
    /// Does this AudioElement use layered playback?
    /// </summary>
	[SerializeField] bool _layered = false;
    
    //[SerializeField] List<AudioClip> _bassAudioClips = new List<AudioClip>();
    //[SerializeField] List<AudioClip> _midAudioClips = new List<AudioClip>();
    //[SerializeField] List<AudioClip> _trebleAudioClips = new List<AudioClip>();
    //[SerializeField] List<AudioClip> _audioClips = new List<AudioClip>();

    /// <summary>
    /// Randomize volume on each playback?
    /// </summary>
    [SerializeField] bool _randomVolume = false;

    /// <summary>
    /// Uniform volume to use.
    /// </summary>
    [SerializeField] float _volume = 1f;

    /// <summary>
    /// Range of volumes to randomize.
    /// </summary>
    [SerializeField] [FloatRange (0f, 1f)]
    FloatRange _volumeRange = new FloatRange();

    /// <summary>
    /// Randomize pitch on each playback?
    /// </summary>
    [SerializeField] bool _randomPitch = false;

    /// <summary>
    /// Uniform pitch to use.
    /// </summary>
    [SerializeField] float _pitch = 1f;

    /// <summary>
    /// Range of pitches to use.
    /// </summary>
    [SerializeField] [FloatRange(-3f, 3f)]
    FloatRange _pitchRange = new FloatRange();

    #endregion
    #region Unity Callbacks

    private void OnEnable() {
        var channels = GetComponentsInChildren<AudioElementChannel>();
        if (channels == null) {
            Debug.LogWarning ("No AudioElementChannels found on this AudioElement! Generating now.", gameObject);
            GenerateAudioChannels();
        } else if (InvalidAudioChannels()) {
            Debug.LogWarning ("Fixing AudioElementChannels...", gameObject);
            GenerateAudioChannels();
        }
    }

    private void Update() {
        if (_loop && _playing) {
            if (_layered) {
                _bassClipTimer -= Time.deltaTime;
                if (_bassClipTimer <= 0f) LoopChannel (_bassChannel, ref _bassClipTimer);

                _midClipTimer -= Time.deltaTime;
                if (_midClipTimer <= 0f) LoopChannel (_midChannel, ref _midClipTimer);

                _trebleClipTimer -= Time.deltaTime;
                if (_trebleClipTimer <= 0f) LoopChannel (_trebleChannel, ref _trebleClipTimer);
            } else {
                _standardClipTimer -= Time.deltaTime;
                if (_standardClipTimer <= 0f) LoopChannel (_standardChannel, ref _standardClipTimer);
            }
        }
    }

    #endregion
    #region Methods

    public void Play () {

    } 

    public void PlayChannel (AudioElementChannel channel) {
        if (_loop) _playing = true;

        channel.PlaySound ();
        
        

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
        _playing = false;
        if (_layered) {
            _bassAudioSource.Stop();
            _midAudioSource.Stop();
            _trebleAudioSource.Stop();
        } else _audioSource.Stop();
    }

    void LoopSound (AudioSource source, List<AudioClip> clips, ref float timer) {
        var clip = clips.GetRandom();
        if (_changeVolumeEachLoop) source.volume = _volumeRange.GetRandomValue();
        if (_changePitchEachLoop) source.pitch = _pitchRange.GetRandomValue();

        source.PlayOneShot (clip);
        timer += clip.length;
    }

    void GenerateAudioSources () {
        // Generate/find standard AudioSource
        var audioSource = GetComponent<AudioSource>();
        _audioSource = audioSource == null ? gameObject.AddComponent<AudioSource>() : audioSource;

        // Generate/find bass AudioSource
        var bassObj = gameObject.FindInChildren (_BASS_AUDIOSOURCE_NAME);
        if (bassObj == null) {
            bassObj = new GameObject (_BASS_AUDIOSOURCE_NAME, typeof (AudioSource));
            bassObj.transform.SetParent (transform);
            bassObj.transform.Reset();
        }
        _bassAudioSource = bassObj.GetComponent<AudioSource>();

        // Generate/find mid AudioSource
        var midObj = gameObject.FindInChildren (_MID_AUDIOSOURCE_NAME);
        if (midObj == null) {
            midObj = new GameObject (_MID_AUDIOSOURCE_NAME, typeof(AudioSource));
            midObj.transform.SetParent (transform);
            midObj.transform.Reset();
        }
        _midAudioSource = midObj.GetComponent<AudioSource>();

        // Generate/find treble AudioSource
        var trebleObj = gameObject.FindInChildren (_TREBLE_AUDIOSOURCE_NAME);
        if (trebleObj == null) {
            trebleObj = new GameObject (_TREBLE_AUDIOSOURCE_NAME, typeof(AudioSource));
            trebleObj.transform.SetParent (transform);
            trebleObj.transform.Reset();
        }
        _trebleAudioSource = trebleObj.GetComponent<AudioSource>();
    }

    bool InvalidAudioSources () {
        return _audioSource == null || _bassAudioSource == null || 
            _midAudioSource == null || _trebleAudioSource == null;
    }
}
