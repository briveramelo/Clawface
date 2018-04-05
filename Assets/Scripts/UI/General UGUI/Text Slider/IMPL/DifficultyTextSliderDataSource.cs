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
                case Difficulty.VERY_EASY:
                    return "VERY EASY";
                case Difficulty.EASY:
                    return "EASY";
                case Difficulty.NORMAL:
                    return "NORMAL";
                case Difficulty.HARD:
                    return "HARD";
                case Difficulty.INSANE:
                    return "INSANE";
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
            return DifficultyToIndex(difficulty);
        }
        set
        {
            difficulty = IndexToDifficulty(value);
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
        base.ForceUpdate();
    }

    public int DifficultyToIndex(Difficulty difficulty)
    {
        switch (difficulty)
        {
            case Difficulty.VERY_EASY:
                return 0;
            case Difficulty.EASY:
                return 1;
            case Difficulty.NORMAL:
                return 2;
            case Difficulty.HARD:
                return 3;
            case Difficulty.INSANE:
                return 4;
            default:
                throw new Exception("Invalid Difficulty Enum value.");
        }
    }

    public Difficulty IndexToDifficulty(int index)
    {
        switch (index)
        {
            case 0:
                return Difficulty.VERY_EASY;
            case 1:
                return Difficulty.EASY;
            case 2:
                return Difficulty.NORMAL;
            case 3:
                return Difficulty.HARD;
            case 4:
                return Difficulty.INSANE;
            default:
                throw new Exception("Difficulty index out of range.");
        }
    }

    #endregion
}
