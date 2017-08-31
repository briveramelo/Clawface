using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ModMan;

public class VFXOneOff : MonoBehaviour {

	ParticleSystem[] _ps;

    void Awake () {
        _ps = GetComponentsInChildren<ParticleSystem>();

    }

    private void OnEnable()
    {
        Play();
        gameObject.DeActivate(2f);
    }

    public void Play () {
        foreach (var ps in _ps) ps.Play();
    }
}
