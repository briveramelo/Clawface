using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ModMan;

public class PSPlayAndDisableAfterSeconds : MonoBehaviour {

    [SerializeField]
    private float timeToDeactivate;

    ParticleSystem[] _ps;

    void Awake()
    {
        _ps = GetComponentsInChildren<ParticleSystem>();

    }

    private void OnEnable()
    {
        Play();
        gameObject.DeActivate(timeToDeactivate);
    }

    public void Play()
    {
        foreach (var ps in _ps) ps.Play();
    }
}
