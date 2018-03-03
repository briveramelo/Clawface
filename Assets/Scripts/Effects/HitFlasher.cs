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
        /// Number of seconds between flashes while stunned.
        /// </summary>
        const float STUNNED_STATE_FLASH_INTERVAL = 0.5f;

        /// <summary>
        /// All SkinnedMeshRenderers attached to this object.
        /// </summary>
        SkinnedMeshRenderer[] meshRenderers;

        #endregion
        #region Serialized Unity Inspector Fields

        [Header("Hit Flash Settings")]

        [SerializeField] Color hitFlashColor = Color.white;

        [Range(0.0f, 1.0f)]
        [SerializeField] float hitFlashStrength = 1.0f;

        [Range(0.01f, 2.0f)]
        [SerializeField] float hitFlashTime = 0.25f;

        [Header("Stunned Flash Settings")]

        //[SerializeField] Color stunnedFlashColor = Color.white;
        [SerializeField] ColorPalette edibleColorPalette;
        [SerializeField] ColorPalette.ColorType edibleColorPaletteType;

        [Range(0.01f, 10.0f)]
        [SerializeField] float stunnedFlashInterval = 0.5f;

        [Range(0.01f, 10.0f)]
        [SerializeField]float proximityFlashInterval = 0.2f;

        [Range(0.0f, 1.0f)]
        [SerializeField] float stunnedFlashStrength = 1.0f;

        [Range(1.0f, 25.0f)]
        [SerializeField] float stunnedFlashPower = 1.0f;

        float flashStrength;

        #endregion

        #region Private fields
        private float flashInterval;
        #endregion

        #region Unity Lifecycle

        void Awake()
        {
            meshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();
        }

        void OnEnable()
        {
            SetFlashStrength(0.0f, Color.white);
        }

        #endregion
        #region Properties

        /// <summary>
        /// Returns the color of the hit flash (read-only).
        /// </summary>
        public Color HitFlashColor { get { return hitFlashColor; } }

        #endregion
        #region Public Methods

        public void HitFlash () { HitFlash (hitFlashStrength, hitFlashTime); }

        /// <summary>
        /// Flashes the shader.
        /// </summary>
        public void HitFlash (float intensity, float duration)
        {
            StopCoroutine("DoHitFlash");
            StartCoroutine(DoHitFlash(intensity, duration));
        }

        #endregion
        #region Private Methods

        /// <summary>
        /// Sets the strength of the flash.
        /// </summary>
        void SetFlashStrength (float strength, Color color)
        {
            flashStrength = strength;
            MaterialPropertyBlock props = new MaterialPropertyBlock();
            props.SetColor ("_HitColor", color);
            props.SetFloat ("_HitColorStrength", strength);
            foreach (SkinnedMeshRenderer meshRenderer in meshRenderers)
                meshRenderer.SetPropertyBlock (props);
        }

        IEnumerator DoHitFlash (float intensity, float duration)
        {
            float t = 0f;
            while (t <= duration)
            {
                SetFlashStrength (intensity * (1f - t / duration), hitFlashColor);

                t += Time.deltaTime;
                yield return null;
            }

            SetFlashStrength (0.0f, hitFlashColor);
        }

        /// <summary>
        /// Sets the flasher to show the stunned state.
        /// </summary>
        public void SetStunnedState ()
        {
            flashInterval = stunnedFlashInterval;
            StopAllCoroutines();
            if (gameObject.activeInHierarchy)
                StartCoroutine(DoStunnedState());
            else Debug.LogWarning("Tried to start stun state on inactive object!", gameObject);
        }

        IEnumerator DoStunnedState ()
        {
            float t = 0f;

            while (true)
            {
                float percent = Mathf.Clamp01(t / flashInterval);


                float flashStrength = Mathf.Clamp01(Mathf.Pow(stunnedFlashStrength * (0.5f + Mathf.Sin(2f * Mathf.PI * percent) + 0.5f), stunnedFlashPower));
                SetFlashStrength(flashStrength, edibleColorPalette.GetColor(edibleColorPaletteType));

                t = (t + Time.unscaledDeltaTime) % (flashInterval);

                yield return null;
            }
        }

        public void SetCloseToEatState()
        {
            flashInterval = proximityFlashInterval;
            StopAllCoroutines();
            if (gameObject.activeInHierarchy)
                StartCoroutine(DoStunnedState());
            else Debug.LogWarning("Tried to start edible state on inactive object!", gameObject);
        }

        public void SetStrength(float newStrength)
        {
            MaterialPropertyBlock props = new MaterialPropertyBlock();
            props.SetFloat("_HitColorStrength", newStrength);
            foreach (SkinnedMeshRenderer meshRenderer in meshRenderers)
                meshRenderer.SetPropertyBlock(props);
        }

        #endregion
    }
}