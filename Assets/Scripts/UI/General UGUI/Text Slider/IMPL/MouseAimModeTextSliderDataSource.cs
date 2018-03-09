using System;
using UnityEngine;

public class MouseAimModeTextSliderDataSource : TextSliderDataSource {

    #region Accessors (TextSliderDataSource)

    public override string Text
    {
        get
        {
            switch(mode)
            {
                case MouseAimMode.AUTOMATIC:
                    return "AUTOMATIC";
                case MouseAimMode.ALWAYS_ON:
                    return "ALWAYS";
                case MouseAimMode.ALWAYS_OFF:
                    return "NEVER";
                default:
                    throw new Exception("Invalid MouseAimMode Enum value.");
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
            return (int)MouseAimMode.COUNT;
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
            mode = (MouseAimMode)value;
        }
    }

    #endregion

    #region Fields (Private)

    private MouseAimMode mode;

    #endregion

    #region Interface (TextSliderDataSource)

    public override void ForceUpdate()
    {
        mode = SettingsManager.Instance.MouseAimMode;
    }

    #endregion
}
