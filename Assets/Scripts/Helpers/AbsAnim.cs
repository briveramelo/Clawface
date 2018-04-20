using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;
using System;

[Serializable]
public abstract class BaseAnim
{
    public AnimationCurve curve;
    public DeltaTime deltaTime;
    public float duration;
    public bool looping, playOnAwake;
    protected Action<float> onUpdate;
    public virtual Action<float> OnUpdate { get { return onUpdate; } set { onUpdate = value; } }
    public Action onComplete;
    public bool IsAnimating { get; private set; }

    protected abstract CoroutineHandle RunCoroutine(string coroutineName);

    public void Animate(string coroutineName)
    {
        IsAnimating = true;
        if (looping)
        {
            Timing.RunCoroutine(Loop(coroutineName), coroutineName);
        }
        else
        {
            Timing.RunCoroutine(Run(coroutineName), coroutineName);
        }
    }

    public void Stop(string coroutineName) {
        Timing.KillCoroutines(coroutineName);
        IsAnimating = false;
    }

    IEnumerator<float> Run(string coroutineName)
    {
        yield return Timing.WaitUntilDone(RunCoroutine(coroutineName));
        if (onComplete != null)
        {
            onComplete();
        }
    }

    IEnumerator<float> Loop(string coroutineName)
    {
        while (true)
        {
            yield return Timing.WaitUntilDone(RunCoroutine(coroutineName));
        }
    }

}

[Serializable]
public class DiffAnim : BaseAnim
{
    public float startValue, diff;

    protected override CoroutineHandle RunCoroutine(string coroutineName)
    {
        return Timing.RunCoroutine(Support.TransitionFloatByDiff(OnUpdate, curve, duration, startValue, diff, deltaTime), coroutineName);
    }
}

[Serializable]
public class AbsAnim : BaseAnim
{
    public float maxValue;

    protected override CoroutineHandle RunCoroutine(string coroutineName)
    {
        return Timing.RunCoroutine(Support.TransitionFloatByAbs(OnUpdate, curve, duration, maxValue, deltaTime), coroutineName);
    }
}

public enum DeltaTime
{
    Standard,
    UnScaled,
    Fixed
}

public static class Support
{

    public static string GetWaveName(int i) {
        string waveNumber = "";
        if (i < 10) {
            waveNumber = string.Format("{0}{1}", 0, i);
        }
        else {
            waveNumber = i.ToString();
        }
        return string.Format("{0}{1}", Strings.Editor.Wave, waveNumber);
    }

    public static float GetDeltaTime(DeltaTime deltaTimeType)
    {
        switch (deltaTimeType)
        {
            case DeltaTime.Standard: return Time.deltaTime;
            case DeltaTime.UnScaled: return Time.unscaledDeltaTime;
            case DeltaTime.Fixed: return Time.fixedDeltaTime;
        }
        return Time.deltaTime;
    }

    public static IEnumerator<float> TransitionFloatByDiff(Action<float> action, AnimationCurve transitionCurve, float transitionTime, float startValue, float totalDiff, DeltaTime deltaTimeType = DeltaTime.Standard)
    {
        float timeRemaining = transitionTime;
        float normalizedTime = 0f;
        while (timeRemaining > 0f)
        {
            float deltaTime = GetDeltaTime(deltaTimeType);            
            timeRemaining -= deltaTime;
            normalizedTime = 1f - timeRemaining / transitionTime;
            float currentDiff = transitionCurve.Evaluate(normalizedTime) * totalDiff;
            float currentValue = startValue + currentDiff;
            action(currentValue);
            yield return 0f;
        }
        float finalDiff = transitionCurve.Evaluate(1f) * totalDiff;
        float endValue = startValue + finalDiff;
        action(endValue);
    }


    public static IEnumerator<float> TransitionFloatByAbs(Action<float> action, AnimationCurve transitionCurve, float transitionTime, float peakValue, DeltaTime deltaTimeType = DeltaTime.Standard)
    {
        float timeRemaining = transitionTime;
        float normalizedTime = 0f;
        while (timeRemaining > 0f)
        {
            float deltaTime = GetDeltaTime(deltaTimeType);
            timeRemaining -= deltaTime;
            normalizedTime = 1f - timeRemaining / transitionTime;
            float currentValue = transitionCurve.Evaluate(normalizedTime) * peakValue;
            action(currentValue);
            yield return 0f;
        }
        float finalValue = transitionCurve.Evaluate(1f) * peakValue;
        action(finalValue);
    }
}

