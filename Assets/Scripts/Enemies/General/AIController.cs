using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AIController : MonoBehaviour {

    [HideInInspector] public float timeInLastState=0;
    [HideInInspector] public bool stateTimerIsRunning=false;
    [HideInInspector] public GameObject attackTarget;

    protected Stats stats;

    protected State currentState;
    public virtual State CurrentState {
        get { return currentState; }
        set {
            if (currentState!=null) {
                currentState.OnExit();
            }
            currentState = value;
            currentState.OnEnter();
            StartCoroutine(IERestartStateTimer());
        }
    }

    protected virtual void Update() {
        currentState.Update();
    }

    public virtual void Reset() {
        stateTimerIsRunning = false;
        timeInLastState = 0f;
    }

    public void RestartStateTimer() {
        StartCoroutine(IERestartStateTimer());
    }

    protected virtual IEnumerator IERestartStateTimer() {
        stateTimerIsRunning = false;
        yield return new WaitForEndOfFrame();
        timeInLastState = 0f;
        stateTimerIsRunning = true;
        while (stateTimerIsRunning) {
            timeInLastState += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    }
}
