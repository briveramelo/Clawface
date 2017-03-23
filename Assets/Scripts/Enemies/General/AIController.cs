//Brandon Rivera-Melo

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
