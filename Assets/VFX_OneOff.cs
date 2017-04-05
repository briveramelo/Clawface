using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFX_OneOff : MonoBehaviour {

	ParticleSystem[] _ps;

    void Awake () {
        _ps = GetComponentsInChildren<ParticleSystem>();

    }

    public void Play () {
        foreach (var ps in _ps) ps.Play();
    }
}
