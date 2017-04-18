using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXFireEffect : MonoBehaviour {

    [SerializeField] bool _startActive = false;

	ParticleSystem[] _ps;
    FlickerLensFlareBrightness[] _flickers;
    LensFlare _lensFlare;
    Light _light;
    FlickerLight[] _lightFlickers;



    private void Awake() {
        _ps = GetComponentsInChildren<ParticleSystem>();
        _light = GetComponentInChildren<Light>();
        _lensFlare = GetComponentInChildren<LensFlare>();
        _flickers = GetComponentsInChildren<FlickerLensFlareBrightness>();
        _lightFlickers = GetComponentsInChildren<FlickerLight>();
        if (_startActive) Play();
    }

    public void Play() {
        foreach (var ps in _ps) ps.Play();
        if (_light != null) _light.enabled = true;
        if (_lensFlare != null) _lensFlare.enabled = true;
        foreach (var flicker in _flickers) flicker.Play();
        foreach (var flicker in _lightFlickers) flicker.Play();
    }

    public void Stop() {
        foreach (var ps in _ps) ps.Stop();
        if (_light != null) _light.enabled = false;
        if (_lensFlare != null) _lensFlare.enabled = false;
        foreach (var flicker in _flickers) flicker.Stop();
        foreach (var flicker in _lightFlickers) flicker.Stop();
    }
}
