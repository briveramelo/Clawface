// SerializableStringPair.cs
// Author: Aaron

using System;

using UnityEngine;

/// <summary>
/// Serializable string pair class.
/// </summary>
[Serializable]
public class SerializableStringPair
{
    #region Vars

    /// <summary>
    /// Key of this pair.
    /// </summary>
    [SerializeField] string key;

    /// <summary>
    /// Value of this pair.
    /// </summary>
    [SerializeField] string value;

    #endregion
    #region Properties

    /// <summary>
    /// Returns the key of this pair (read-only).
    /// </summary>
    public string Key { get { return key; } }

    /// <summary>
    /// Returns the value of this pair (read-only).
    /// </summary>
    public string Value { get { return value; } }

    #endregion
    #region Constructors

    /// <summary>
    /// Key/value constructor.
    /// </summary>
    public SerializableStringPair (string key, string value) 
    {
        this.key = key;
        this.value = value;
    }

    #endregion
    #region Methods

    /// <summary>
    /// Sets the value of this pair.
    /// </summary>
    public void SetValue (string value) 
    {
        this.value = value;
    }

    public override string ToString() 
    {
        return string.Format ("{0}: {1}", key, value);
    }

    #endregion
}
