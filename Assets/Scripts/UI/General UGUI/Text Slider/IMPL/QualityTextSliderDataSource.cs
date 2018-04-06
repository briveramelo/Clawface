using UnityEngine;

public class QualityTextSliderDataSource : TextSliderDataSource {

    #region Accessors (TextSliderDataSource)

    public override string Text
    {
        get
        {
            return QualitySettings.names[index].ToUpper();
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
            return QualitySettings.names.Length;
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
        index = SettingsManager.Instance.QualityLevel;
        base.ForceUpdate();
    }

    #endregion
}
