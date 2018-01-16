#region Using Statements

using System;
using UnityEngine;
using UnityEngine.Assertions;

#endregion

/// <summary>
/// A Scriptable Object that represents a splat that can be applied to the world.
/// </summary>
[CreateAssetMenu (fileName = "Splat 1", menuName = "Turing/Gore/Splat SO")]
public class SplatSO : ScriptableObject {

    #region Accessors (Public)

    /// <summary>
    /// Gets the Frames to Render
    /// </summary>
    public Frame[] Frames
    {
        get
        {
            return frames;
        }
    }

    /// <summary>
    /// Where, in UV coords, to anchor the rotation and position
    /// of the frames.
    /// </summary>
    public Vector2 Anchor
    {
        get
        {
            return anchor;
        }
    }

    /// <summary>
    /// Initial rotation of the frame.
    /// </summary>
    public Vector2 Rotation
    {
        get
        {
            return rotation;
        }
    }

    #endregion

    #region Fields (Unity Serialization)

    /// <summary>
    /// Serialized frames.  See Accessors.
    /// </summary>
    [SerializeField]
    private Frame[] frames = null;

    /// <summary>
    /// Serialized anchor.  See Accessors.
    /// </summary>
    [SerializeField]
    private Vector2 anchor = new Vector2(0F, 0.5F);

    /// <summary>
    /// Serialized rotation value.  See Accessors.
    /// </summary>
    [SerializeField]
    private Vector2 rotation = new Vector2(0F, 0F);

    #endregion

    #region Inteface (Unity Lifecycle)

    /// <summary>
    /// Awakes this instance.
    /// </summary>
    private void Awake()
    {
        // Asserts
        Assert.IsNotNull(frames);
        Assert.IsTrue(frames.Length >= 1);
        Assert.IsTrue(rotation.sqrMagnitude > 0);

        // Quick Fixes
        rotation.Normalize();
    }

    #endregion

    #region Interface (Public)

    /// <summary>
    /// Checks if the Splat has more frames.
    /// </summary>
    /// <param name="frame">The index of the to-be-displayed frame.</param>
    /// <returns>true if there is more frames, false otherwise.</returns>
    public bool HasMoreFrames(int frame)
    {
        return frame < frames.Length;
    }

    /// <summary>
    /// Calculates the rotation for this splat based off of a provided rotation
    /// and the splat's implicit rotation.
    /// </summary>
    /// <param name="rotation">The projectile rotation / direction in the X/Z plane.</param>
    /// <returns>The "rotation" of the splat to render frames with.</returns>
    public Vector2 CalculateRotation(Vector2 rotation)
    {
        return (this.rotation + rotation).normalized;
    }

    #endregion

    #region Types (Public)

    /// <summary>
    /// A Frame of the Spalt Animation.
    /// </summary>
    [Serializable]
    public struct Frame
    {
        #region Fields (Public)

        /// <summary>
        /// The splat's mask indicating where blood will be splat and where it won't.
        /// </summary>
        public Texture2D mask;

        /// <summary>
        /// A normal used for adding additional data to the painted splat.
        /// YAY LUMPY BLOOD!
        /// </summary>
        public Texture2D normal;

        #endregion
    }

    #endregion
}
