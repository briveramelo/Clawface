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

    #region 2. Serialized Unity Inspector Fields
    [SerializeField] protected string DEBUG_CURRENTSTATE;
    [SerializeField] protected string DEBUG_ATTACKTARGET;
    [SerializeField] protected GameObject modPrefab;
    #endregion

    #region 3. Hidden Unity Inspector Fields
    [HideInInspector] public float timeInLastState = 0;
    [HideInInspector] public bool stateTimerIsRunning = false;
    #endregion

    #region 4. Private fields
    private Transform attackTarget;
    #endregion

    #region 5. Protected fields
    protected AIProperties properties;
    protected Stats stats;
    protected Mod mod;
    protected States states;
    protected Animator animator;
    protected TransformMemento modMemento = new TransformMemento();
    protected NavMeshAgent navAgent;
    protected NavMeshObstacle navObstacle;
    protected BulletHellPatternController bulletPatternController;
    protected State currentState;
    #endregion


    public Transform AttackTarget {
        get {
            if (attackTarget==null) {
                attackTarget = FindPlayer();
            }
            return attackTarget;
        }
        set {
            attackTarget = value;
            DEBUG_ATTACKTARGET = attackTarget.name;
        }
    }
    public Vector3 AttackTargetPosition { get { return AttackTarget.position - transform.forward * .1f; } }



    private bool deActivateAI = false;
    public float distanceFromTarget;
    public float DistanceFromTarget {get{ distanceFromTarget = Vector3.Distance(transform.position, AttackTarget.position); return distanceFromTarget; }}
    public Vector3 DirectionToTarget {
        get {
            return (AttackTargetPosition - transform.position).NormalizedNoY();            
        }
    }
    
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
    public List<Func<bool>> checksToUpdateState = new List<Func<bool>>();

    private void OnDisable()
    {
        foreach (KeyValuePair<EAIState, AIState> state in states.aiStates)
        {
                Timing.KillCoroutines(state.Value.coroutineName);
        }
    }

   public void DeActivateAI()
    {
        deActivateAI = true;
    }


    public void Update() {

        if (!deActivateAI)
        {
            bool hasUpdated = false;
            checksToUpdateState.ForEach(check => {
                if (!hasUpdated && check())
                {
                    hasUpdated = true;
                }
            });
            if (currentState != null)
            {
                currentState.Update();
            }
        }
    }


    public virtual void ResetForRebirth() {
        stateTimerIsRunning = false;
        timeInLastState = 0f;
        deActivateAI = false;
        CurrentState = states.chase;

        if (mod != null)
        {
            mod.transform.Reset(modMemento);
            mod.DeactivateModCanvas();

        }
    }

    public void RestartStateTimer() {
        Timing.RunCoroutine(IERestartStateTimer());
    }

    public Transform FindPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag(Strings.Tags.PLAYER);
        if (player)
        {
            return player.transform;
        }
        else
        {
            return null;
        }
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
        VelocityBody velBody,
        Animator animator,
        Stats stats,
        NavMeshAgent navAgent,
        NavMeshObstacle navObstacle,
        List<AIState> aiStates)
    {

        this.properties = properties;
        this.stats = stats;
        this.animator = animator;
        this.navAgent = navAgent;
        this.navObstacle = navObstacle;

        states = new States();
        states.Initialize(properties, this, velBody, animator, stats, navAgent, navObstacle, aiStates);

        if (mod != null)
            modMemento.Initialize(mod.transform);

        CurrentState = states.chase;
    }

    public void Initialize(
        AIProperties properties,
        Mod mod,
        VelocityBody velBody,
        Animator animator,
        Stats stats,
        NavMeshAgent navAgent,
        NavMeshObstacle navObstacle,
        List<AIState> aiStates )
    {

        this.properties = properties;
        this.mod = mod;
        this.stats = stats;
        this.animator = animator;
        this.navAgent = navAgent;
        this.navObstacle = navObstacle;

        states = new States();
        states.Initialize(properties, this, velBody, animator, stats, navAgent, navObstacle,aiStates);

        if(mod != null)
        modMemento.Initialize(mod.transform);

        CurrentState = states.chase;
    }

    public void Initialize(
        AIProperties properties,
        VelocityBody velBody,
        Animator animator,
        Stats stats,
        NavMeshAgent navAgent,
        NavMeshObstacle navObstacle,
        BulletHellPatternController bulletPatternController,
        List<AIState> aiStates)
    {

        this.properties = properties;
        this.stats = stats;
        this.animator = animator;
        this.navAgent = navAgent;
        this.navObstacle = navObstacle;

        states = new States();
        states.Initialize(properties, this, velBody, animator, stats, navAgent, navObstacle, bulletPatternController,aiStates);

        if (mod != null)
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
        if(mod != null)
        mod.Activate();
    }

    public void AttackAnimationDone()
    {
        CurrentState = states.chase;
    }
    #endregion



    protected class States
    {
        public AIState chase;
        public AIState attack;
        public AIState fire;
        public AIState death;
        public AIState stun;

        public Dictionary<EAIState, AIState> aiStates;

        public void Initialize(
            AIProperties properties,
            AIController controller,
            VelocityBody velBody,
            Animator animator,
            Stats stats,
            NavMeshAgent navAgent,
            NavMeshObstacle navObstacle,
            List<AIState> aiStatesElements)
        {

            aiStates = new Dictionary<EAIState, AIState>();

            foreach (AIState state in aiStatesElements)
            {

                state.Initialize(properties, controller, velBody, animator, stats, navAgent, navObstacle);

                if (state.stateName.Equals("chase"))
                {
                    chase = state;
                    aiStates.Add(EAIState.Chase, chase);
                }
                else if (state.stateName.Equals("attack"))
                {
                    attack = state;
                    aiStates.Add(EAIState.Attack, attack);
                }

                else if (state.stateName.Equals("fire"))
                {
                    fire = state;
                    aiStates.Add(EAIState.Fire, fire);
                }

                else if (state.stateName.Equals("death"))
                {
                    death = state;
                    aiStates.Add(EAIState.Death, death);
                }
                else if (state.stateName.Equals("stun"))
                {
                    stun = state;
                    aiStates.Add(EAIState.Stun, stun);
                }
            }

        }

        public void Initialize(
            AIProperties properties,
            AIController controller,
            VelocityBody velBody,
            Animator animator,
            Stats stats,
            NavMeshAgent navAgent,
            NavMeshObstacle navObstacle,
            BulletHellPatternController bulletPatternController,
            List<AIState> aiStatesElements)
        {

            aiStates = new Dictionary<EAIState, AIState>();

            foreach (AIState state in aiStatesElements)
            {

                state.Initialize(properties, controller, velBody, animator, stats, navAgent, navObstacle, bulletPatternController);

                if (state.stateName.Equals("chase"))
                {
                    chase = state;
                    aiStates.Add(EAIState.Chase, chase);
                }
                else if (state.stateName.Equals("attack"))
                {
                    attack = state;
                    aiStates.Add(EAIState.Attack, attack);
                }

                else if (state.stateName.Equals("fire"))
                {
                    fire = state;
                    aiStates.Add(EAIState.Fire, fire);
                }

                else if (state.stateName.Equals("death"))
                {
                    death = state;
                    aiStates.Add(EAIState.Death, death);
                }
                else if (state.stateName.Equals("stun"))
                {
                    stun = state;
                    aiStates.Add(EAIState.Stun, stun);
                }

            }

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
