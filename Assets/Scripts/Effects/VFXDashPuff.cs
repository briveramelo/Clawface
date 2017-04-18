using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXDashPuff : MonoBehaviour {

	ParticleSystem[] _ps;

    private void Awake() {
        _ps = GetComponentsInChildren<ParticleSystem>();
    }

    public void Play () {
        foreach (var ps in _ps) ps.Play();
    }

    public void Stop () {
        foreach (var ps in _ps) ps.Stop();
    }
}
