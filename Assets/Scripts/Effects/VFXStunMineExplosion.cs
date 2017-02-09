using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXStunMineExplosion : MonoBehaviour {

	[SerializeField] ParticleSystem _shockEmitter;

    [SerializeField] ParticleSystem _sparkEmitter;

    [SerializeField] ParticleSystem _sparkTrailEmitter;

	public void Emit () {
        _shockEmitter.Emit (8);
        _sparkEmitter.Emit (15);
        _sparkTrailEmitter.Emit(10);
    }
}
