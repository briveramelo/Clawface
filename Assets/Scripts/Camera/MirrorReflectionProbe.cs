// MirrorReflectionProbe.cs
// Author: Aaron

using UnityEngine;

namespace Turing.Effects
{
    /// <summary>
    /// Behavior to make a ReflectionProbe follow a camera under the floor.
    /// </summary>
    public sealed class MirrorReflectionProbe : MonoBehaviour
    {
        #region Serialized Unity Inspector Fields

        /// <summary>
        /// Camera transform to follow.
        /// </summary>
        [SerializeField]
        [Tooltip("Camera transform to follow.")]
        new Transform camera;

        #endregion
        #region Unity Lifecycle

        private void Update()
        {
            if (camera == null) camera = Camera.main.transform;

            else
            {
                transform.position = new Vector3(
                    camera.position.x,
                    -camera.position.y,
                    camera.position.z);
            }
        }

        #endregion
    }
}