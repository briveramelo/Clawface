//Brandon Rivera-Melo

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MovementEffects;
using System;

public abstract class AIController : MonoBehaviour {

    [SerializeField] protected string DEBUG_CURRENTSTATE;

    [HideInInspector] public float timeInLastState = 0;
    [HideInInspector] public bool stateTimerIsRunning = false;
    [HideInInspector] public Transform attackTarget;

    protected Stats stats;
    protected float distanceFromTarget {get{ return Vector3.Distance(transform.position, attackTarget.position); }}
    public Vector3 directionToTarget {
        get {
            Vector3 dir = attackTarget.position - transform.position;
            dir.y = 0;
            return dir.normalized;
        }
    }
    protected State currentState;
    public virtual State CurrentState {
        get { return currentState; }
        set {
            if (currentState!=null) {
                currentState.OnExit();
            }
            currentState = value;
            DEBUG_CURRENTSTATE = currentState.ToString();
            currentState.OnEnter();
            Timing.RunCoroutine(IERestartStateTimer());
        }
    }
    protected List<Func<bool>> checksToUpdateState = new List<Func<bool>>();

    protected virtual void Update() {
        bool hasUpdated = false;
        checksToUpdateState.ForEach(check => {
            if (!hasUpdated && check()) {
                hasUpdated = true;
            }
        });
        if (currentState!=null) {
            currentState.Update();
        }
    }

    public virtual void ResetForRebirth() {
        stateTimerIsRunning = false;
        timeInLastState = 0f;
    }

    public void RestartStateTimer() {
        Timing.RunCoroutine(IERestartStateTimer());
    }

    protected IEnumerator<float> IERestartStateTimer() {
        stateTimerIsRunning = false;
        yield return Timing.WaitForOneFrame;
        timeInLastState = 0f;
        stateTimerIsRunning = true;
        while (stateTimerIsRunning) {
            timeInLastState += Time.deltaTime;
            yield return Timing.WaitForOneFrame;
        }
    }
}
