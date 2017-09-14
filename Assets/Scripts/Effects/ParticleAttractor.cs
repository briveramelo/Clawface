// ParticleAttractor.cs
// Author: Aaron

using UnityEngine;

namespace Turing.VFX
{
    /// <summary>
    /// Behavior to attract particles towards a Transform.
    /// </summary>
    [RequireComponent(typeof(ParticleSystem))]
    public sealed class ParticleAttractor : MonoBehaviour
    {
        #region Serialized Unity Inspector Vars

        /// <summary>
        /// Transform towards which to attract particles.
        /// </summary>
        [Tooltip("Transform towards which to attract particles.")]
        [SerializeField] Transform attractorOrigin;

        /// <summary>
        /// Strength of attraction.
        /// </summary>
        [Tooltip("Strength of attraction.")]
        [SerializeField] float attractorStrength;

        /// <summary>
        /// Type of attraction.
        /// </summary>
        [Tooltip("Type of attraction.")]
        [SerializeField] AttractionType attractionType = 
            AttractionType.Linear;

        /// <summary>
        /// Reduce lifetime as particles get close to the origin?
        /// </summary>
        [Tooltip("Reduce lifetime as particles get close to the origin?")]
        [SerializeField] bool reduceLifetimeNearOrigin = true;

        /// <summary>
        /// How quickly are particle lifetimes reduced near the origin?
        /// </summary>
        [Tooltip("How quickly are particle lifetimes reduced near the origin?")]
        [SerializeField] float lifetimeReductionFactor = 0.5f;

        /// <summary>
        /// Use curve to dictate attraction over time?
        /// </summary>
        [Tooltip("Use curve to dictate attraction over time?")]
        [SerializeField] bool useAttractionOverTime = false;
        
        /// <summary>
        /// Attraction over time curve.
        /// </summary>
        [Tooltip("Attraction over time curve.")]
        [SerializeField] AnimationCurve attractionOverTime;

        #endregion
        #region Private Fields

        /// <summary>
        /// All particles in this ParticleSystem.
        /// </summary>
        ParticleSystem.Particle[] particles;

        /// <summary>
        /// ParticleSystem attached to this object.
        /// </summary>
        ParticleSystem ps;

        #endregion
        #region Unity Lifecycle

        private void Awake()
        {
            ps = GetComponent<ParticleSystem>();
            particles = new ParticleSystem.Particle[ps.main.maxParticles];
        }

        private void LateUpdate()
        {
            var alive = ps.GetParticles(particles);
            var dp = Time.deltaTime * (useAttractionOverTime ? attractionOverTime.Evaluate(ps.time) : attractorStrength);
            for (int i = 0; i < alive; i++)
            {
                var d = (attractorOrigin.position - particles[i].position);
                float dv = 0f;
                switch (attractionType)
                {
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

                particles[i].velocity = particles[i].velocity + d.normalized * dv;
                if (reduceLifetimeNearOrigin) particles[i].remainingLifetime -= Time.deltaTime * lifetimeReductionFactor;
            }
            ps.SetParticles(particles, alive);
        }

        #endregion
        #region public Structures

        /// <summary>
        /// Type of particle attraction.
        /// </summary>
        public enum AttractionType
        {
            Linear,
            Inverse,
            InverseSquared
        }

        #endregion
    }
}