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

        /// <summary>
        /// All SkinnedMeshRenderers attached to this object.
        /// </summary>
        SkinnedMeshRenderer[] meshRenderers;

        #endregion
        #region Unity Lifecycle

        void Awake()
        {
            meshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();
        }

        void OnEnable()
        {
            SetFlashStrength(0.0f);
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

        /// <summary>
        /// Sets the strength of the flash.
        /// </summary>
        void SetFlashStrength (float strength)
        {
            MaterialPropertyBlock props = new MaterialPropertyBlock();
            props.SetFloat ("_HitColorStrength", strength);
            foreach (SkinnedMeshRenderer meshRenderer in meshRenderers)
                meshRenderer.SetPropertyBlock (props);
        }

        IEnumerator DoFlash (float intensity, float duration)
        {
            float t = 0f;
            while (t <= duration)
            {
                SetFlashStrength (intensity * (1f - t / duration));

                t += Time.deltaTime;
                yield return null;
            }

            SetFlashStrength (0.0f);
        }

        #endregion
    }
}