// VFXOneOff.cs
// Author: Aaron

using ModMan;

using System.Collections;

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

        void OnEnable ()
        {            
            if (playOnAwake) Play();
        }

        #endregion
        #region Private Methods

        /// <summary>
        /// Timer for self-disabling.
        /// </summary>
        IEnumerator DisableTimer(float duration)
        {
            yield return new WaitForSeconds (duration);

            Stop();
            gameObject.SetActive(false);
        }

        #endregion
        #region Public Methods
        public void Play(float duration) {
            gameObject.SetActive(true);
            PlayInternal();
            StartCoroutine(DisableTimer(duration));
        }


        /// <summary>
        /// Plays this effect.
        /// </summary>
        public void Play()
        {
            PlayInternal();
            StartCoroutine(DisableTimer(EffectDuration));
        }

        void PlayInternal() {
            gameObject.SetActive(true);

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

            StopAllCoroutines();
        }

        /// <summary>
        /// Returns the duration of this effect (equal to the time length
        /// of the longest aspect of this effect).
        /// </summary>
        public float EffectDuration
        {
            get
            {
                float max = 0f;

                // Check particle systems
                foreach (var particleSystem in particleSystems)
                {
                    float duration = particleSystem.main.duration + 
                        particleSystem.main.startLifetime.Max();
                    duration /= particleSystem.main.simulationSpeed;
                    if (duration > max)
                        max = duration;
                }

                return max;
            }
        }

        #endregion
    }
}