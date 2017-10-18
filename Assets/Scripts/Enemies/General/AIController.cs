//Brandon Rivera-Melo

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using MovementEffects;
using System;
using System.Linq;
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
    public Vector3 AttackTargetPosition { get { return AttackTarget.position - transform.forward * .1f; } }
    private Transform attackTarget;

    [SerializeField] protected GameObject modPrefab;
    protected AIProperties properties;
    protected Stats stats;
    protected Mod mod;
    protected States states;
    protected Animator animator;
    protected TransformMemento modMemento = new TransformMemento();
    protected NavMeshAgent navAgent;

    protected float distanceFromTarget {get{ return Vector3.Distance(transform.position, AttackTarget.position); }}
    public Vector3 directionToTarget {
        get {
            return (AttackTargetPosition - transform.position).NormalizedNoY();            
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

        CurrentState = states.chase;
        mod.transform.Reset(modMemento);
        mod.DeactivateModCanvas();
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

    protected EAIState ECurrentState
    {
        get
        {
            return states.GetEState((AIState)CurrentState);
        }
    }

    public void Initialize(
        AIProperties properties,
        Mod mod,
        VelocityBody velBody,
        Animator animator,
        Stats stats,
        NavMeshAgent navAgent)
    {

        this.properties = properties;
        this.mod = mod;
        this.stats = stats;
        this.animator = animator;
        this.navAgent = navAgent;

        states = new States();
        states.Initialize(properties, this, velBody, animator, stats, navAgent);
        modMemento.Initialize(mod.transform);
        CurrentState = states.chase;
    }

    public void UpdateState(EAIState state)
    {
        CurrentState = states.GetState(state);
    }

    #region Animation Events
    public void ActivateMod()
    {
        mod.Activate();
    }

    public void AttackAnimationDone()
    {
        CurrentState = states.chase;
    }
    #endregion



    protected class States
    {
        public AIChaseState chase;
        public AIAttackState attack;
        public AIFireState fire;
        public AIDeathState death;

        private Dictionary<EAIState, AIState> aiStates;

        public void Initialize(
            AIProperties properties,
            AIController controller,
            VelocityBody velBody,
            Animator animator,
            Stats stats,
            NavMeshAgent navAgent)
        {

            chase.Initialize(properties, controller, velBody, animator, stats, navAgent);
            attack.Initialize(properties, controller, velBody, animator, stats, navAgent);
            fire.Initialize(properties, controller, velBody, animator, stats, navAgent);
            death.Initialize(properties, controller, velBody, animator, stats, navAgent);

            aiStates = new Dictionary<EAIState, AIState>() {
                {EAIState.Chase, chase },
                {EAIState.Attack, attack },
                {EAIState.Fire, fire },
                {EAIState.Death, death },
            };
        }

        public AIState GetState(EAIState state)
        {
            return aiStates[state];
        }
        public EAIState GetEState(AIState state)
        {
            EAIState key = aiStates.FirstOrDefault(x => x.Value == state).Key;
            return key;
        }
    }

}
