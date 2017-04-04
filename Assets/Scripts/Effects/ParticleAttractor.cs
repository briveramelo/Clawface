using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class ParticleAttractor : MonoBehaviour {

	[SerializeField] Transform _attractorOrigin;

    [SerializeField] float _attractorStrength;

    [SerializeField] AttractionType _attractionType = AttractionType.Linear;

    [SerializeField] bool _reduceLifetimeNearOrigin = true;

    [SerializeField] float _lifetimeReductionFactor = 0.5f;

    ParticleSystem.Particle[] _particles;

    ParticleSystem _ps;

    private void Awake() {
        _ps = GetComponent<ParticleSystem>();
        _particles = new ParticleSystem.Particle[_ps.main.maxParticles];
    }

    private void LateUpdate() {
        var alive = _ps.GetParticles (_particles);
        var dp = Time.deltaTime * _attractorStrength;
        for (int i = 0; i < alive; i++) {
            var d = (_attractorOrigin.position - _particles[i].position);
            float dv = 0f;
            switch (_attractionType) {
                case AttractionType.Linear:
                    dv = dp;
                    break;
                case AttractionType.Inverse:
                    dv = dp / d.magnitude;
                    break;
                case AttractionType.InverseSquared:
                    dv = dp * dp / d.magnitude / d.magnitude;
                    break;
            }
            _particles[i].velocity = _particles[i].velocity + d.normalized * dv;
            if (_reduceLifetimeNearOrigin) _particles[i].remainingLifetime -= Time.deltaTime * _lifetimeReductionFactor;
        }
        _ps.SetParticles (_particles, alive);
    }

    public enum AttractionType {
        Linear,
        Inverse,
        InverseSquared
    }
}
