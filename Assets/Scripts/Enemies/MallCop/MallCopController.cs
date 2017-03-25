using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MallCopController : AIController {
    
    protected MallCopProperties properties;
    protected Mod mod;
    protected States states;
    protected Animator animator;

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
        Stats stats) {

        this.properties = properties;
        this.mod = mod;
        this.stats = stats;
        this.animator = animator;

        states = new States();
        states.Initialize(properties, this, velBody, animator, stats);
        CurrentState = states.patrol;
    }    

    public void UpdateState(EMallCopState state) {

        switch (state) {
            case EMallCopState.Swing:
                CurrentState = states.swing;
                break;
            case EMallCopState.Fall:
                CurrentState = states.fall;
                break;
            case EMallCopState.Patrol:
                CurrentState = states.patrol;
                break;
            case EMallCopState.Chase:
                CurrentState = states.chase;
                break;
            case EMallCopState.Twitch:
                CurrentState = states.twitch;
                break;
            case EMallCopState.Fire:
                CurrentState = states.twitch;
                break;
        }

    }

    public override void Reset() {
        CurrentState = states.patrol;
        base.Reset();
    }

    #region Animation Events
    public void Swing() {
        mod.Activate();
    }

    public void AttackAnimationDone() {
        CurrentState = states.chase;
    }
    #endregion


    protected class States {
        public MallCopPatrolState patrol = new MallCopPatrolState();
        public MallCopChaseState chase= new MallCopChaseState();
        public MallCopSwingState swing = new MallCopSwingState();
        public MallCopTwitchState twitch = new MallCopTwitchState();
        public MallCopFallState fall = new MallCopFallState();

        public MallCopFireState fire = new MallCopFireState();
        public MallCopFleeState flee = new MallCopFleeState();

        public void Initialize(
            MallCopProperties properties,
            MallCopController controller,
            VelocityBody velBody,
            Animator animator, 
            Stats stats) {

            chase.Initialize(properties, controller, velBody, animator, stats);
            patrol.Initialize(properties, controller, velBody, animator, stats);
            swing.Initialize(properties, controller, velBody, animator, stats);
            twitch.Initialize(properties, controller, velBody, animator, stats);
            fall.Initialize(properties, controller, velBody, animator, stats);
            fire.Initialize(properties, controller, velBody, animator, stats);
            flee.Initialize(properties, controller, velBody, animator, stats);
        }
    }
}

public enum MallCopAnimationStates {
    Idle = 0,
    Walk = 1,
    Swing = 2,
    HitReaction = 3,
    Stunned = 4,
    GettingUp = 5,
    DrawWeapon = 6,
    Run = 7,
    Shoot = 8
}
public enum EMallCopState {
    Patrol = 0,
    Swing = 1,
    Fall = 3,
    Chase = 4,
    Twitch = 5,
    Fire=6,
    Flee=7
}
