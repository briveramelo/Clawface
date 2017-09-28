using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.AI;

public abstract class KamikazeController : AIController {

    public EKamikazeState ECurrentState
    {
        get
        {
            return states.GetEState((KamikazeState)CurrentState);
        }
    }


    protected KamikazeProperties properties;
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
       KamikazeProperties properties,
       VelocityBody velBody,
       Animator animator,
       Stats stats,
       NavMeshAgent navAgent
        )
    {

        this.properties = properties;
        this.stats = stats;
        this.animator = animator;
        this.navAgent = navAgent;

        states = new States();
        states.Initialize(properties, this, velBody, animator, stats, navAgent);
        CurrentState = states.chase;
    }

    public void UpdateState(EKamikazeState state)
    {
        CurrentState = states.GetState(state);
    }

    public override void ResetForRebirth()
    {
        CurrentState = states.chase;
        base.ResetForRebirth();
    }

    public void ActivateMod()
    {

    }

    public void AttackAnimationDone()
    {
        CurrentState = states.chase;
    }


    protected class States
    {
        public KamikazeChaseState chase = new KamikazeChaseState();
        public KamikazeSelfDestructState selfDestruct = new KamikazeSelfDestructState();

        private Dictionary<EKamikazeState, KamikazeState> kamikazeStates;

        public void Initialize(
            KamikazeProperties properties,
            KamikazeController controller,
            VelocityBody velBody,
            Animator animator,
            Stats stats,
            NavMeshAgent navAgent
           )
        {

            chase.Initialize(properties, controller, velBody, animator, stats, navAgent);
            selfDestruct.Initialize(properties, controller, velBody, animator, stats, navAgent);

            kamikazeStates = new Dictionary<EKamikazeState, KamikazeState>() {
                {EKamikazeState.Chase, chase },
                {EKamikazeState.SelfDestruct, selfDestruct },
            };
        }

        public KamikazeState GetState(EKamikazeState state)
        {
            return kamikazeStates[state];
        }
        public EKamikazeState GetEState(KamikazeState state)
        {
            EKamikazeState key = kamikazeStates.FirstOrDefault(x => x.Value == state).Key;
            return key;
        }
    }

}
