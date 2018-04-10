using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GoreDetailTextSliderDataSource : TextSliderDataSource {

    #region Serialized Unity Fields
    [SerializeField] private List<IntName> valueNames; 
    #endregion

    #region Accessors (TextSliderDataSource)

    public override string Text
    {
        get
        {
            return valueNames.Find(valueName => valueName.value == goreDetail).name;
        }
    }

    public override object Value
    {
        get
        {
            return goreDetail;
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
            return goreDetail;
        }
        set
        {
            goreDetail = value;
        }
    }

    #endregion

    #region Fields (Private)

    private int goreDetail;

    #endregion

    #region Interface (TextSliderDataSource)

    public override void ForceUpdate()
    {
        goreDetail = SettingsManager.Instance.GoreDetail;
        base.ForceUpdate();
    }

    #endregion
}

[System.Serializable]
public class IntName {
    public string name;
    public int value;
}