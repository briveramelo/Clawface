using System;
using UnityEngine;

public abstract class TextSliderDataSource : MonoBehaviour
{

    #region Accessors (Public)

    /// <summary>
    /// Returns the textual representation of the currently selected value
    /// </summary>
    public abstract string Text
    {
        get;
    }

    /// <summary>
    /// Returns the untyped current value (to facilitate working with Unity nonsense),.
    /// This will have to be cast by whatever is using this..  Don't do stupid things. :P
    /// </summary>
    public abstract object Value
    {
        get;
    }

    /// <summary>
    /// The number of entries to display;
    /// </summary>
    public abstract int Count
    {
        get;
    }

    public abstract int Selected
    {
        get;
        set;
    }

    #endregion

    #region Interface (Public)

    public virtual void ForceUpdate()
    {
        if (OnDataSourceForcedUpdate != null)
        {
            OnDataSourceForcedUpdate.Invoke(this);
        }
    }

    #endregion

    #region Types (Public)

    public event Action<TextSliderDataSource> OnDataSourceForcedUpdate;

    #endregion
}
