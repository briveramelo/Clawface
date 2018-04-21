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

    protected virtual void KillCoroutines() {
        Timing.KillCoroutines(CoroutineName);
    }
    protected void KillRoutine(string routineName) {
        Timing.KillCoroutines(routineName);
    }

    protected virtual CoroutineHandle RunRoutine(IEnumerator<float> routine, Segment segment = Segment.Update) {
        return Timing.RunCoroutine(routine, segment, CoroutineName);
    }
    protected virtual float WaitFor(IEnumerator<float> routine, Segment segment = Segment.Update) {
        return Timing.WaitUntilDone(RunRoutine(routine, segment));
    }
    protected virtual float WaitForSeconds(float secondsToWait) {
        return Timing.WaitForSeconds(secondsToWait);
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