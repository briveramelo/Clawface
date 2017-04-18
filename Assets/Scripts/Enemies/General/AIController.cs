//Brandon Rivera-Melo

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MovementEffects;
using System;
using ModMan;

public abstract class AIController : MonoBehaviour {

    [SerializeField] protected string DEBUG_CURRENTSTATE;

    [HideInInspector] public float timeInLastState = 0;
    [HideInInspector] public bool stateTimerIsRunning = false;    
    public Transform AttackTarget {
        get {
            if (attackTarget==null) {
                attackTarget = FindPlayer();
            }
            return attackTarget;
        }
        set { attackTarget = value; }
    }
    private Transform attackTarget;

    protected Stats stats;
    protected float distanceFromTarget {get{ return Vector3.Distance(transform.position, AttackTarget.position); }}
    public Vector3 directionToTarget {
        get {
            return (AttackTarget.position - transform.position).NormalizedNoY();            
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

    public Transform FindPlayer()
    {
        return GameObject.FindGameObjectWithTag(Strings.Tags.PLAYER).transform;
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
