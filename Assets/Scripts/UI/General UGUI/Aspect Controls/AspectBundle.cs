using System;
using UnityEngine;

[Serializable]
public class AspectBundle<TBundleData>
{
    #region Fields (Public)

    public Aspect aspect;
    public TBundleData data;

    #endregion

    #region Interface (Public)

    public float GetAspectRatio()
    {
        switch (aspect)
        {
            case Aspect.Aspect5By4:
                return 5F / 4F;
            case Aspect.Aspect4By3:
                return 4F / 3F;
            case Aspect.Aspect3By2:
                return 3F / 2F;
            case Aspect.Aspect16By10:
                return 16F / 10F;
            case Aspect.Aspect16By9:
                return 16F / 9F;
            default:
                throw new Exception("Provided aspect not listed!");
        }
    }

    #endregion

    #region Types (Public)

    public enum Aspect : int
    {
        Aspect5By4,
        Aspect4By3,
        Aspect3By2,
        Aspect16By10,
        Aspect16By9,
    }

    #endregion
}