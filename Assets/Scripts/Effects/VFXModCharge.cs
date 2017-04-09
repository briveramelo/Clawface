using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXModCharge : MonoBehaviour {

    ParticleSystem _particleSystem;

    private void Awake() {
        _particleSystem = GetComponentInChildren<ParticleSystem>();
    }

    public void Enable() {
        _particleSystem.Play();
    }

    public void OnDisable() {
        _particleSystem.Stop();
    }
}
