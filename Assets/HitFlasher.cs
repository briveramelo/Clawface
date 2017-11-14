// HitFlasher.cs
// Author: Aaron

using System.Collections;

using UnityEngine;

namespace Turing.VFX
{
    /// <summary>
    /// Behavior to control the hit flash shader.
    /// </summary>
    public class HitFlasher : MonoBehaviour
    {
        #region Private Fields

        SkinnedMeshRenderer[] meshRenderers;

        #endregion
        #region Unity Lifecycle

        void Awake()
        {
            meshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();
        }

        #endregion
        #region Public Methods

        /// <summary>
        /// Flashes the shader.
        /// </summary>
        public void Flash (float intensity=1.0f, float duration=0.25f)
        {
            StopAllCoroutines();
            StartCoroutine(DoFlash(intensity, duration));
        }

        #endregion
        #region Private Methods

        IEnumerator DoFlash (float intensity, float duration)
        {
            float t = 0f;
            while (t <= duration)
            {
                MaterialPropertyBlock props = new MaterialPropertyBlock();
                props.SetFloat ("_HitColorStrength", intensity * (1f - t / duration));
                foreach (SkinnedMeshRenderer meshRenderer in meshRenderers)
                    meshRenderer.SetPropertyBlock (props);

                t += Time.deltaTime;
                yield return null;
            }
        }

        #endregion
    }
}