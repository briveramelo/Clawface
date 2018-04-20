using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;
using System;
public class RoutineRunner : MonoBehaviour {

    private string coroutineName;
    protected string CoroutineName {
        get {
            if (string.IsNullOrEmpty(coroutineName)) {
                coroutineName = Guid.NewGuid().ToString();
            }
            return coroutineName;
        }
    }
    protected virtual void OnDisable() {
        KillCoroutines();
    }

    protected void RunRoutine(IEnumerator<float> routine, Segment segment=Segment.Update) {
        Timing.RunCoroutine(routine, segment, CoroutineName);
    }

    protected IEnumerator<float> DelayAction(Action action, float seconds = 0f) {
        yield return 0f;
        yield return Timing.WaitForSeconds(seconds);
        action();
    }
    protected IEnumerator<float> DelayAction(Action action, int frames)
    {
        for (int i = 0; i < frames; i++) {
            yield return 0f;
        }
        action();
    }
    protected virtual void KillCoroutines() {
        Timing.KillCoroutines(CoroutineName);
    }
}

public class RoutineRunnerNonMono {

    private string coroutineName;
    public string CoroutineName {
        get {
            if (string.IsNullOrEmpty(coroutineName)) {
                coroutineName = Guid.NewGuid().ToString();
            }
            return coroutineName;
        }
    }
    public virtual void Kill() {
        Timing.KillCoroutines(CoroutineName);
    }

    protected IEnumerator<float> DelayAction(Action action, float seconds = 0f) {
        yield return 0f;
        yield return Timing.WaitForSeconds(seconds);
        action();
    }
}