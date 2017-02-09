using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXBlasterShoot : MonoBehaviour {

	[SerializeField] ParticleSystem _puffEmitter;

    [SerializeField] ParticleSystem _sparkEmitter;

    public void Emit () {
        _puffEmitter.Emit (5);
        _sparkEmitter.Emit (15);
    }
}
