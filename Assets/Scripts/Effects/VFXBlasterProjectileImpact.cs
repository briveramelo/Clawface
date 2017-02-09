using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXBlasterProjectileImpact : MonoBehaviour {

    [SerializeField] ParticleSystem _puffEmitter;

    [SerializeField] ParticleSystem _sparkEmitter;
	
	public void Emit () {
        _puffEmitter.Emit (15);
        _sparkEmitter.Emit (30);
    }
}
