// TrailerLightManager.cs
// Author: Aaron

using System.Collections.Generic;

using UnityEngine;

namespace Turing.VFX
{
    /// <summary>
    /// Manager for lighting for the trailer.
    /// </summary>
    public sealed class TrailerLightManager : MonoBehaviour
    {
        #region Serialized Unity Inspector Fields

        /// <summary>
        /// Lighting scale curve.
        /// </summary>
        [Tooltip("Lighting scale curve.")]
        [SerializeField] AnimationCurve lightScaler;

        /// <summary>
        /// Lights to ignore.
        /// </summary>
        [Tooltip("Lights to ignore.")]
        [SerializeField] List<Light> ignoreList = new List<Light>();

        #endregion
        #region Private Fields

        Dictionary<Light, float> intensities = 
            new Dictionary<Light, float>();

        #endregion
        #region Unity Lifecycle

        private void Awake()
        {
            var lights = FindObjectsOfType<Light>();
            foreach (var light in lights)
            {
                if (ignoreList.Contains(light)) continue;

                intensities.Add(light, light.intensity);
            }

            Application.targetFrameRate = 60;
        }

        private void Update()
        {
            ScaleLights(lightScaler.Evaluate(Time.realtimeSinceStartup));
        }

        #endregion
        #region Private Methods

        void ScaleLights(float value)
        {
            foreach (var light in intensities.Keys)
            {
                light.intensity = intensities[light] * value;
            }
        }

        #endregion
    }
}