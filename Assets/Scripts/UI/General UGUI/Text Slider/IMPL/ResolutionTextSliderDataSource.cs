using System.Collections.Generic;
using UnityEngine;

public class ResolutionTextSliderDataSource : TextSliderDataSource {

    #region Accessors (TextSliderDataSource)

    public override string Text
    {
        get
        {
            return string.Format("{0} x {1}", resolution.width, resolution.height);
        }
    }

    public override object Value
    {
        get
        {
            return resolution;
        }
    }

    public override int Count
    {
        get
        {
            return validResolutions.Length;
        }
    }

    public override int Selected
    {
        get
        {
            return LocateIndex(resolution);
        }
        set
        {
            resolution = validResolutions[value];
        }
    }

    #endregion

    #region Fields (Private)

    private Resolution[] validResolutions;
    private Resolution resolution;

    #endregion

    #region Interface (TextSliderDataSource)

    public override void ForceUpdate()
    {
        resolution = SettingsManager.Instance.Resolution;
        validResolutions = DetermineValidResolutions();
        base.ForceUpdate();
    }

    #endregion

    #region Interface (Private)

    private Resolution[] DetermineValidResolutions()
    {
        Resolution[] all = Screen.resolutions;
        List<Resolution> valid = new List<Resolution>();
        foreach (Resolution resolution in all)
        {
            if (IsSupported(resolution))
            {
                Resolution cmp = valid.Find((other) => { return other.width == resolution.width && other.height == resolution.height; });
                if (cmp.width != resolution.width || cmp.height != resolution.height) {
                    valid.Add(resolution);
                } else if (cmp.refreshRate < resolution.refreshRate)
                {
                    valid.Remove(cmp);
                    valid.Add(cmp);
                }
            }
        }
        return valid.ToArray();
    }

    private bool IsSupported(Resolution resolution)
    {
        float aspect = 1.0F * resolution.width / resolution.height;
        return (
            Mathf.Approximately(aspect, 5F / 4F) ||
            Mathf.Approximately(aspect, 4F / 3F) ||
            Mathf.Approximately(aspect, 3F / 2F) ||
            Mathf.Approximately(aspect, 16F / 10F) ||
            Mathf.Approximately(aspect, 16F / 9F)
            );
    }

    private int LocateIndex(Resolution resolution)
    {
        Resolution[] valid = validResolutions;
        for (int index = 0; index < valid.Length; index++)
        {
            Resolution cmp = valid[index];
            if (cmp.width == resolution.width && cmp.height == resolution.height)
            {
                return index;
            }
        }

        return -1;
    }

    #endregion
}
