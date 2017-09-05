// VFXOneOff.cs
// Author: Aaron

using UnityEngine;

namespace Turing.VFX
{
    /// <summary>
    /// Standard class for one-off VFX.
    /// </summary>
    public class VFXOneOff : MonoBehaviour
    {
        #region Serialized Unity Inspector Fields

        /// <summary>
        /// Play this effect on awake?
        /// </summary>
        [Tooltip("Play this effect on awake?")]
        [SerializeField] bool playOnAwake = true;

        #endregion
        #region Private Fields

        ParticleSystem[] particleSystems;
        Light[] lights;
        LensFlare[] lensFlares;
        FlickerLight[] lightFlickers;
        FlickerLensFlareBrightness[] lensFlareFlickers;

        #endregion
        #region Unity Lifecycle

        void Awake()
        {
            particleSystems = GetComponentsInChildren<ParticleSystem>();
            lights = GetComponentsInChildren<Light>();
            lensFlares = GetComponentsInChildren<LensFlare>();
            lightFlickers = GetComponentsInChildren<FlickerLight>();
            lensFlareFlickers = GetComponentsInChildren<FlickerLensFlareBrightness>();

            if (playOnAwake) Play();
        }

        #endregion
        #region Public Methods

        /// <summary>
        /// Plays this effect.
        /// </summary>
        public void Play()
        {
            foreach (var particleSystem in particleSystems)
                particleSystem.Play();
            foreach (var light in lights) light.enabled = true;
            foreach (var lensFlare in lensFlares) lensFlare.enabled = true;
            foreach (var lightFlicker in lightFlickers) lightFlicker.Play();
            foreach (var lensFlareFlicker in lensFlareFlickers) lensFlareFlicker.Play();
        }

        /// <summary>
        /// Stops this effect.
        /// </summary>
        public void Stop()
        {
            foreach (var particleSystem in particleSystems)
                particleSystem.Stop();
            foreach (var light in lights) light.enabled = false;
            foreach (var lensFlare in lensFlares) lensFlare.enabled = false;
            foreach (var lightFlicker in lightFlickers) lightFlicker.Stop();
            foreach (var lensFlareFlicker in lensFlareFlickers) lensFlareFlicker.Stop();
        }

        #endregion
    }
}