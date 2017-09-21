using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.AI;

public abstract class BouncerController : AIController {

    public EBouncerState ECurrentState
    {
        get
        {
            return states.GetEState((BouncerState)CurrentState);
        }
    }


    protected BouncerProperties properties;
    protected States states;
    protected Animator animator;
    protected TransformMemento modMemento = new TransformMemento();


    public override State CurrentState
    {
        set
        {
            base.CurrentState = value;
        }
    }

    public virtual void Initialize(
       BouncerProperties properties,
       VelocityBody velBody,
       Animator animator,
       Stats stats,
       NavMeshAgent navAgent)
    {

        this.properties = properties;
        this.stats = stats;
        this.animator = animator;
        this.navAgent = navAgent;

        states = new States();
        states.Initialize(properties, this, velBody, animator, stats, navAgent);
        CurrentState = states.patrol;
    }

    public void UpdateState(EBouncerState state)
    {
        CurrentState = states.GetState(state);
    }

    public override void ResetForRebirth()
    {
        CurrentState = states.patrol;
        base.ResetForRebirth();
    }

    public void AttackAnimationDone()
    {
        CurrentState = states.chase;
    }


    protected class States
    {
        public BouncerPatrolState patrol = new BouncerPatrolState();
        public BouncerChaseState chase = new BouncerChaseState();
        public BouncerFireState fire = new BouncerFireState();
        public BouncerTwitchState twitch = new BouncerTwitchState();
        public BouncerFallState fall = new BouncerFallState();

        private Dictionary<EBouncerState, BouncerState> bouncerStates;

        public void Initialize(
            BouncerProperties properties,
            BouncerController controller,
            VelocityBody velBody,
            Animator animator,
            Stats stats,
            NavMeshAgent navAgent)
        {

            patrol.Initialize(properties, controller, velBody, animator, stats, navAgent);
            chase.Initialize(properties, controller, velBody, animator, stats, navAgent);
            fire.Initialize(properties, controller, velBody, animator, stats, navAgent);
            twitch.Initialize(properties, controller, velBody, animator, stats, navAgent);
            fall.Initialize(properties, controller, velBody, animator, stats, navAgent);

            bouncerStates = new Dictionary<EBouncerState, BouncerState>() {
                {EBouncerState.Patrol, patrol },
                {EBouncerState.Chase, chase },
                {EBouncerState.Fire, fire },
                {EBouncerState.Twitch, twitch },
                {EBouncerState.Fall, fall },
            };
        }

        public BouncerState GetState(EBouncerState state)
        {
            return bouncerStates[state];
        }
        public EBouncerState GetEState(BouncerState state)
        {
            EBouncerState key = bouncerStates.FirstOrDefault(x => x.Value == state).Key;
            return key;
        }
    }


}
