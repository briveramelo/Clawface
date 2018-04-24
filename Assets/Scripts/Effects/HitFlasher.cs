// HitFlasher.cs
// Author: Aaron

using System.Collections;
using ModMan;
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
        Renderer[] meshRenderers;
        Renderer[] MeshRenderers {
            get {
                if (meshRenderers==null) {
                    meshRenderers = GetComponentsInChildren<Renderer>(true);
                }
                return meshRenderers;
            }
        }

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
        [SerializeField] Color initialOutlineColor = new Color(255f/255f, 142f/255f, 0f);
        float flashStrength;

        #endregion

        #region Private fields
        private float flashInterval;
        #endregion

        #region Unity Lifecycle
        
        #endregion
        #region Properties

        /// <summary>
        /// Returns the color of the hit flash (read-only).
        /// </summary>
        public Color HitFlashColor { get { return hitFlashColor; } }

        #endregion
        #region Public Methods
        void StopCoroutines() {
            StopCoroutine("FixFlashColor");
            StopCoroutine("DoHitFlash");
            StopCoroutine("MoveToColor");
        }

        public void ResetFlashColor() {
            StopCoroutines();
            SetFlashColorAndStrength(0f, hitFlashColor);            
        }
        public void ResetOutlineColor() {
            SetOutlineColor(initialOutlineColor);
        }
        public void GraduallyResetOutlineColor(float transitionDuration) {
            StartCoroutine(MoveToColor(transitionDuration, "_ASEOutlineColor", initialOutlineColor));
        }
        public void GraduallyResetFlashColor(float transitionDuration) {
            StartCoroutine(MoveToColor(transitionDuration, "_HitColor", hitFlashColor));
        }
        public void HitFlash () { HitFlash (hitFlashStrength, hitFlashTime); }

        /// <summary>
        /// Flashes the shader.
        /// </summary>
        public void HitFlash (float intensity, float duration)
        {
            StopCoroutines();
            StartCoroutine(DoHitFlash(intensity, duration, hitFlashColor));
        }


        public void FixColor(float intensity, float duration, Color fixedColor) {
            StopCoroutines();
            StartCoroutine(FixFlashColor(intensity, duration, fixedColor));
        }
        public void FixColor(float intensity, float duration) {
            StopCoroutines();
            StartCoroutine(FixFlashColor(intensity, duration, hitFlashColor));
        }

        #endregion
        #region Private Methods

        /// <summary>
        /// Sets the strength of the flash.
        /// </summary>
        void SetFlashColorAndStrength (float strength, Color color)
        {
            flashStrength = strength;
            foreach (SkinnedMeshRenderer meshRenderer in MeshRenderers) {
                MaterialPropertyBlock props = new MaterialPropertyBlock();
                meshRenderer.GetPropertyBlock(props);
                props.SetColor ("_HitColor", color);
                props.SetFloat ("_HitColorStrength", strength);
                meshRenderer.SetPropertyBlock (props);
            }
        }

        void LerpToColorProperty(Color targetColor, string colorProp, float progress) {
            foreach (SkinnedMeshRenderer meshRenderer in MeshRenderers) {
                MaterialPropertyBlock props = new MaterialPropertyBlock();
                meshRenderer.GetPropertyBlock(props);
                Color currentColor = props.GetVector(colorProp);
                currentColor = Color.Lerp(currentColor, targetColor, progress);
                props.SetColor(colorProp, currentColor);
                meshRenderer.SetPropertyBlock(props);
            }
        }

        IEnumerator MoveToColor(float duration, string outlineColorProp, Color targetOutlineColor) {
            float t = 0f;
            while (t <= duration) {
                float progress = t / duration;
                LerpToColorProperty(targetOutlineColor, outlineColorProp, progress);
                t += Time.deltaTime;
                yield return null;
            }

            LerpToColorProperty(targetOutlineColor, outlineColorProp, 1f);
        }


        IEnumerator DoHitFlash (float intensity, float duration, Color thisHitFlashColor)
        {
            float t = 0f;
            while (t <= duration)
            {
                SetFlashColorAndStrength (intensity * (1f - t / duration), thisHitFlashColor);

                t += Time.deltaTime;
                yield return null;
            }

            SetFlashColorAndStrength (0.0f, thisHitFlashColor);
        }

        IEnumerator FixFlashColor(float intensity, float duration, Color fixedColor) {
            float t = 0f;
            SetFlashColorAndStrength(intensity, fixedColor);
            while (t <= duration) {
                t += Time.deltaTime;
                yield return null;
            }

            SetFlashColorAndStrength(0.0f, hitFlashColor);
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
                SetFlashColorAndStrength(flashStrength, edibleColorPalette.GetColor(edibleColorPaletteType));

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
            foreach (SkinnedMeshRenderer meshRenderer in MeshRenderers) {
                MaterialPropertyBlock props = new MaterialPropertyBlock();
                meshRenderer.GetPropertyBlock(props);
                props.SetFloat("_HitColorStrength", newStrength);
                meshRenderer.SetPropertyBlock(props);
            }
        }

        public void SetOutlineColor(Color outlineColor) {
            foreach (SkinnedMeshRenderer meshRenderer in MeshRenderers) {
                MaterialPropertyBlock props = new MaterialPropertyBlock();
                meshRenderer.GetPropertyBlock(props);
                props.SetColor("_ASEOutlineColor", outlineColor);
                meshRenderer.SetPropertyBlock(props);
            }
        }

        #endregion
    }
}