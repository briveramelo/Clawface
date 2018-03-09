using System;
using UnityEngine;

public class DifficultyTextSliderDataSource : TextSliderDataSource {

    #region Accessors (TextSliderDataSource)

    public override string Text
    {
        get
        {
            switch(difficulty)
            {
                case Difficulty.EASY:
                    return "EASY";
                case Difficulty.NORMAL:
                    return "NORMAL";
                case Difficulty.HARD:
                    return "HARD";
                default:
                    throw new Exception("Invalid Difficulty Enum value.");
            }
        }
    }

    public override object Value
    {
        get
        {
            return difficulty;
        }
    }

    public override int Count
    {
        get
        {
            return (int)Difficulty.COUNT;
        }
    }

    public override int Selected
    {
        get
        {
            return (int)difficulty;
        }
        set
        {
            difficulty = (Difficulty)value;
        }
    }

    #endregion

    #region Fields (Private)

    private Difficulty difficulty;

    #endregion

    #region Interface (TextSliderDataSource)

    public override void ForceUpdate()
    {
        difficulty = SettingsManager.Instance.Difficulty;
    }

    #endregion
}
