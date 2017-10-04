using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.AI;

public abstract class ZombieController : AIController {

    public EZombieState ECurrentState
    {
        get
        {
            return states.GetEState((ZombieState)CurrentState);
        }
    }


    //[SerializeField] protected GameObject modPrefab;

    protected ZombieProperties properties;
    //protected Mod mod;
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
       ZombieProperties properties,
       //Mod mod,
       VelocityBody velBody,
       Animator animator,
       Stats stats,
       NavMeshAgent navAgent)
    {

        this.properties = properties;
        //this.mod = mod;
        this.stats = stats;
        this.animator = animator;
        this.navAgent = navAgent;

        states = new States();
        states.Initialize(properties, this, velBody, animator, stats, navAgent);
        //modMemento.Initialize(mod.transform);
        CurrentState = states.chase;
    }

    public void UpdateState(EZombieState state)
    {
        CurrentState = states.GetState(state);
    }

    public override void ResetForRebirth()
    {
        CurrentState = states.chase;
        base.ResetForRebirth();
    }

    #region Animation Events
    //public void ActivateMod()
    //{
    //    mod.Activate();
    //}

    public void AttackAnimationDone()
    {
        CurrentState = states.chase;
    }
    #endregion

    protected class States
    {
        public ZombieChaseState chase = new ZombieChaseState();
        public ZombieAttackState attack = new ZombieAttackState();
        public ZombieFallState fall = new ZombieFallState();

        private Dictionary<EZombieState, ZombieState> zombieStates;

        public void Initialize(
            ZombieProperties properties,
            ZombieController controller,
            VelocityBody velBody,
            Animator animator,
            Stats stats,
            NavMeshAgent navAgent)
        {
            chase.Initialize(properties, controller, velBody, animator, stats, navAgent);
            attack.Initialize(properties, controller, velBody, animator, stats, navAgent);
            fall.Initialize(properties, controller, velBody, animator, stats, navAgent);

            zombieStates = new Dictionary<EZombieState, ZombieState>() {
                {EZombieState.Chase, chase },
                {EZombieState.Attack, attack },
                {EZombieState.Fall, fall },
            };
        }

        public ZombieState GetState(EZombieState state)
        {
            return zombieStates[state];
        }
        public EZombieState GetEState(ZombieState state)
        {
            EZombieState key = zombieStates.FirstOrDefault(x => x.Value == state).Key;
            return key;
        }
    }



}
