using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXModCharge : MonoBehaviour {

    [Tooltip("Mod charge time (in seconds).")]
    [SerializeField] float _chargeTime;
    float _chargeTimer;
    bool _playing = false;

    [SerializeField] List<ParticleSystem> _chargingParticleSystems;
    [SerializeField] List<ParticleSystem> _onChargedParticleSystems;
    [SerializeField] List<ParticleSystem> _chargedParticleSystems;

    private void Awake() {
        
    }

    private void Update() {
        if (_chargeTimer > 0f && _playing) {
            _chargeTimer -= Time.deltaTime;
            if (_chargeTimer <= 0f) {
                foreach (var ps in _chargingParticleSystems) ps.Stop();
                foreach (var ps in _onChargedParticleSystems) ps.Play();
                foreach (var ps in _chargedParticleSystems) ps.Play();
            }
        }
    }

    public void StartCharging (float time) {
        _chargeTimer = time;
        _playing = true;
        foreach (var ps in _chargedParticleSystems) ps.Stop();
        foreach (var ps in _chargingParticleSystems) {
            var main = ps.main;
            main.duration = time;
            ps.Play();
        }
    }

    public void StartCharging () {
       StartCharging (_chargeTime);
    }

    public void StopCharging () {
        foreach (var ps in _chargingParticleSystems) ps.Stop();
        foreach (var ps in _chargedParticleSystems) ps.Stop();
        _playing = false;
    }
}
