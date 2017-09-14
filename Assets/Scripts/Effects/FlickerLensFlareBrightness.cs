// FlickerLensFlareBrightness.cs
// Author: Aaron

using System.Collections;

using UnityEngine;

namespace Turing.VFX
{
    /// <summary>
    /// Behavior to flicker the brightness of a LensFlare.
    /// </summary>
    public sealed class FlickerLensFlareBrightness : MonoBehaviour
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
        /// LensFlare attached to this object.
        /// </summary>
        LensFlare flare;

        /// <summary>
        /// Min/max brightness.
        /// </summary>
        float min, max;

        #endregion
        #region Unity Lifecycle

        private void Awake()
        {
            flare = GetComponent<LensFlare>();
            max = flare.brightness;
            min = max * flickerIntensity;
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
                while (flare == null) yield return null;

                flare.brightness = Random.Range(min, max);

                yield return new WaitForSeconds(flickerFrequency);
            }
        }

        #endregion
    }
}