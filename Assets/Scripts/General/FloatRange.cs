// FloatRange.cs
// Author: Aaron

using System;

using UnityEngine;

/// <summary>
/// Class to define a float range.
/// </summary>
[Serializable]
public class FloatRange
{
    #region Vars

    /// <summary>
    /// Mininum boundary.
    /// </summary>
    [SerializeField]
    float min = 1f;

    /// <summary>
    /// Maximum boundary.
    /// </summary>
    [SerializeField]
    float max = 1f;

    #endregion
    #region Properties

    /// <summary>
    /// Gets/sets the minumum boundary of this FloatRange.
    /// </summary>
    public float Min
    {
        get { return min; }
        set { min = value; }
    }

    /// <summary>
    /// Gets/sets the maximum boundary of this FloatRange.
    /// </summary>
    public float Max
    {
        get { return max; }
        set { max = value; }
    }

    #endregion
    #region Methods

    /// <summary>
    /// Returns a random value from the range.
    /// </summary>
    public float GetRandomValue()
    {
        return UnityEngine.Random.Range(min, max);
    }

    #endregion
}