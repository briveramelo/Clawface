using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXFireStream : MonoBehaviour {

    [SerializeField]
	List<ParticleSystem> _emitters;

    public void Play () {
        foreach (var emitter in _emitters) emitter.Play();
    }

    public void Stop () {
        foreach (var emitter in _emitters) emitter.Stop();
    }
}
