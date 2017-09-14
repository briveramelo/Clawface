// FlickerLight.cs
// Author: Aaron

using System.Collections;

using UnityEngine;

namespace Turing.VFX
{
    /// <summary>
    /// Behavior to flicker light intensity.
    /// </summary>
    public class FlickerLight : MonoBehaviour
    {
        #region Serialized Unity Inspector Fields

        /// <summary>
        /// Intensity of flickering.
        /// </summary>
        [Tooltip("Intensity of flickering.")]
        [SerializeField] float flickerIntensity = 0.5f;

        /// <summary>
        /// Frequency of flickering (seconds).
        /// </summary>
        [Tooltip("Frequency of flickering (seconds).")]
        [SerializeField] float flickerFrequency = 0.1f;

        #endregion
        #region Private Fields

        /// <summary>
        /// Light attached to this behavior.
        /// </summary>
        new Light light;

        /// <summary>
        /// Min/max light intensity.
        /// </summary>
        float min, max;

        #endregion
        #region Unity Lifecycle

        private void Awake()
        {
            light = GetComponent<Light>();
            max = light.intensity;
            min = max * flickerIntensity;
            StartCoroutine(Flicker());
        }

        #endregion
        #region Public Methods

        /// <summary>
        /// Starts flickering.
        /// </summary>
        public void Play()
        {
            StartCoroutine(Flicker());
        }

        /// <summary>
        /// Pauses flickering.
        /// </summary>
        public void Stop()
        {
            StopCoroutine(Flicker());
        }

        #endregion
        #region Private Methods

        IEnumerator Flicker()
        {
            while (true)
            {
                while (light == null) yield return null;

                light.intensity = Random.Range(min, max);

                yield return new WaitForSeconds(flickerFrequency);
            }
        }

        #endregion
    }
}