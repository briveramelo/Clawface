// FloatRange.cs

using UnityEngine;

/// <summary>
/// Class to define a float range.
/// </summary>
[System.Serializable]
public class FloatRange {

    #region Vars

    /// <summary>
    /// Mininum boundary.
    /// </summary>
    [SerializeField] float _min = 1f;

    /// <summary>
    /// Maximum boundary.
    /// </summary>
    [SerializeField] float _max = 1f;

    #endregion
    #region Properties

    /// <summary>
    /// Gets/sets the minumum boundary of this FloatRange.
    /// </summary>
    public float Min {
        get { return _min; }
        set { _min = value; }
    }

    /// <summary>
    /// Gets/sets the maximum boundary of this FloatRange.
    /// </summary>
    public float Max {
        get { return _max; }
        set { _max = value; }
    }

    #endregion
    #region Methods

    /// <summary>
    /// Returns a random value from the range.
    /// </summary>
    public float GetRandomValue () {
        return Random.Range (_min, _max);
    }

    #endregion
}
