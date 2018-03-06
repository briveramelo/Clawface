using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class GoreDetailTextSliderDataSource : TextSliderDataSource {

    #region Accessors (TextSliderDataSource)

    public override string Text
    {
        get
        {
            if (index == 0)
            {
                return "OFF";
            } else
            {
                int value = (int)Mathf.Pow(2, index - 1);
                return string.Format("x{0}", value);
            }
        }
    }

    public override object Value
    {
        get
        {
            return index;
        }
    }
    
    public override int Count
    {
        get
        {
            return 5; // HACK - This should be a constant somewhere...
        }
    }

    public override int Selected
    {
        get
        {
            return index;
        }
        set
        {
            index = value;
        }
    }

    #endregion

    #region Fields (Private)

    private int index;

    #endregion

    #region Interface (TextSliderDataSource)

    public override void ForceUpdate()
    {
        index = SettingsManager.Instance.GoreDetail;
    }

    #endregion
}
