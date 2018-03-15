using System;
using UnityEngine;

public class FireModeTextSliderDataSource : TextSliderDataSource {

    #region Accessors (TextSliderDataSource)

    public override string Text
    {
        get
        {
            switch(mode)
            {
                case FireMode.AIM_TO_SHOOT:
                    return "AIM TO SHOOT";
                case FireMode.SINGLE_TRIGGER:
                    return "SINGLE TRIGGER";
                case FireMode.AUTOFIRE:
                    return "AUTOMATIC";
                case FireMode.MANUAL:
                    return "MANUAL";
                default:
                    throw new Exception("Invalid FireMode Enum value.");
            }
        }
    }

    public override object Value
    {
        get
        {
            return mode;
        }
    }

    public override int Count
    {
        get
        {
            return (int)FireMode.COUNT;
        }
    }

    public override int Selected
    {
        get
        {
            return (int)mode;
        }
        set
        {
            mode = (FireMode)value;
        }
    }

    #endregion

    #region Fields (Private)

    private FireMode mode;

    #endregion

    #region Interface (TextSliderDataSource)

    public override void ForceUpdate()
    {
        mode = SettingsManager.Instance.FireMode;
        base.ForceUpdate();
    }

    #endregion
}
