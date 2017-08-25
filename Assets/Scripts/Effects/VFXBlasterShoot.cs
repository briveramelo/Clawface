using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ModMan;

public class VFXBlasterShoot : MonoBehaviour {

	[SerializeField] ParticleSystem _puffEmitter;

    [SerializeField] ParticleSystem _sparkEmitter;

    private void OnEnable() {
        gameObject.DeActivate(1.5f);
    }

    public void Emit () {
        gameObject.SetActive(true);
        _puffEmitter.Emit (5);
        _sparkEmitter.Emit (15);
    } 
}
