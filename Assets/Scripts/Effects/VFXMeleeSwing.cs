// VFXMeleeSwing.cs
// Author: Aaron

using UnityEngine;

namespace Turing.VFX
{
    /// <summary>
    /// Behavior for melee swing meshes.
    /// </summary>
    public class VFXMeleeSwing : MonoBehaviour
    {
        #region Serialized Unity Inspector Fields

        /// <summary>
        /// UV value to start effect at.
        /// </summary>
        [Tooltip("UV value to start effect at.")]
        [SerializeField] float startUV = 1f;

        /// <summary>
        /// UV value to end effect at.
        /// </summary>
        [Tooltip("UV value to end effect at.")]
        [SerializeField] float endUV = 1f;

        /// <summary>
        /// Speed of effect.
        /// </summary>
        [Tooltip("Speed of effect.")]
        [SerializeField] float effectSpeed = 1f;

        #endregion
        #region Private Fields

        /// <summary>
        /// Is this effect currently playing?
        /// </summary>
        bool playing = false;

        /// <summary>
        /// Material used in this effect.
        /// </summary>
        Material mat;

        #endregion
        #region Unity Lifecycle

        private void Awake()
        {
            mat = GetComponent<MeshRenderer>().material;
            PlayAnimation();
        }

        private void Update()
        {
            if (playing)
            {
                var dUV = (endUV - startUV) * Time.deltaTime * effectSpeed;
                SetUV(Mathf.Clamp(mat.mainTextureOffset.x + dUV, startUV, endUV));
                if (mat.mainTextureOffset.x == endUV)
                {
                    playing = false;
                    GetComponent<MeshRenderer>().enabled = false;
                }
            }
        }

        #endregion
        #region Public Methods

        /// <summary>
        /// Plays this effect.
        /// </summary>
        public void PlayAnimation()
        {
            GetComponent<MeshRenderer>().enabled = true;
            SetUV(startUV);
            playing = true;
        }

        #endregion
        #region Private Methods

        /// <summary>
        /// Sets the UV of the effect mesh.
        /// </summary>
        void SetUV(float newUV)
        {
            var offset = mat.mainTextureOffset;
            offset.x = newUV;
            mat.mainTextureOffset = offset;
        }

        #endregion
    }
}