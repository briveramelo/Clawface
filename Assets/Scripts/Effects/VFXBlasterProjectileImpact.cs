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

    private void OnParticleCollision(GameObject other)
    {
        //print("Bullet hit " + other.transform.root.gameObject.name + " with layer " + other.transform.root.gameObject.layer);

    }
}
