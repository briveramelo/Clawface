using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXStunBatonImpact : MonoBehaviour {

    [SerializeField] ParticleSystem _shockEmitter;

    [SerializeField] ParticleSystem _sparkEmitter;

	public void Emit () {
        _shockEmitter.Emit (8);
        _sparkEmitter.Emit (15);
    }
}
