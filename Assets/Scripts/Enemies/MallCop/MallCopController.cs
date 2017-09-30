using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using ModMan;
using UnityEngine.AI;

public abstract class MallCopController : AIController {

    public EMallCopState ECurrentState {
        get {            
            return states.GetEState((MallCopState)CurrentState);
        }
    }

    [SerializeField] protected GameObject modPrefab;

    protected MallCopProperties properties;
    protected Mod mod;
    protected States states;
    protected Animator animator;
    protected TransformMemento modMemento = new TransformMemento();

    public override State CurrentState {
        set {
            if (stats.health > 0 || value==states.fall) {
                if ((currentState == states.twitch && !states.twitch.IsMidTwitch()) ||
                    currentState != states.twitch ||
                    value==states.fall) {

                    base.CurrentState = value;                                      
                }
            }
        }
    }

    public virtual void Initialize(
        MallCopProperties properties,
        Mod mod,
        VelocityBody velBody,
        Animator animator,
        Stats stats,
        NavMeshAgent navAgent) {

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

    public void UpdateState(EMallCopState state) {
        CurrentState = states.GetState(state);
    }

    public override void ResetForRebirth() {
        CurrentState = states.chase;
        //if (mod==null || mod.GetWielderInstanceID()!=gameObject.GetInstanceID()) {
        //    Debug.Log(mod.GetWielderInstanceID() + " " + gameObject.GetInstanceID());
        //    GameObject newMod = Instantiate(modPrefab);
        //    mod = newMod.GetComponent<Mod>();
        //}
        mod.transform.Reset(modMemento);        
        mod.DeactivateModCanvas();            
        base.ResetForRebirth();
    }

    #region Animation Events
    public void ActivateMod() {
        mod.Activate();
    }

    public void AttackAnimationDone() {
        CurrentState = states.chase;
    }
    #endregion


    protected class States {
        public MallCopChaseState chase= new MallCopChaseState();
        public MallCopSwingState swing = new MallCopSwingState();
        public MallCopTwitchState twitch = new MallCopTwitchState();
        public MallCopFallState fall = new MallCopFallState();

        public MallCopFireState fire = new MallCopFireState();
        public MallCopFleeState flee = new MallCopFleeState();

        private Dictionary<EMallCopState, MallCopState> mallCopStates;

        public void Initialize(
            MallCopProperties properties,
            MallCopController controller,
            VelocityBody velBody,
            Animator animator, 
            Stats stats,
            NavMeshAgent navAgent) {

            chase.Initialize(properties, controller, velBody, animator, stats, navAgent);
            swing.Initialize(properties, controller, velBody, animator, stats, navAgent);
            twitch.Initialize(properties, controller, velBody, animator, stats, navAgent);
            fall.Initialize(properties, controller, velBody, animator, stats, navAgent);
            fire.Initialize(properties, controller, velBody, animator, stats, navAgent);
            flee.Initialize(properties, controller, velBody, animator, stats, navAgent);

            mallCopStates = new Dictionary<EMallCopState, MallCopState>() {
                {EMallCopState.Swing, swing },
                {EMallCopState.Fall, fall },
                {EMallCopState.Chase, chase },
                {EMallCopState.Twitch, twitch },
                {EMallCopState.Fire, fire },
                {EMallCopState.Flee, flee },
            };
        }

        public MallCopState GetState(EMallCopState state) {
            return mallCopStates[state];
        }
        public EMallCopState GetEState(MallCopState state) {
            EMallCopState key = mallCopStates.FirstOrDefault(x => x.Value == state).Key;
            return key;
        }
    }
}


