using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MallCopController : AIController {

    public string mystate;
    protected MallCopProperties properties;
    protected Mod mod;
    protected States states;

    public override State CurrentState {
        set {
            if (stats.health > 0 || value==states.fall) {
                if ((currentState == states.twitch && !states.twitch.IsMidTwitch()) || currentState != states.twitch) {
                    if (currentState != null) {
                        currentState.OnExit();
                    }
                    currentState = value;
                    mystate = currentState.ToString();
                    currentState.OnEnter();
                    StartCoroutine(IERestartStateTimer());
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

    #region Animation Events
    public void Swing() {
        mod.Activate();
    }

    public void AttackAnimationDone() {
        CurrentState = states.chase;
    }
    #endregion

    public override void Reset() {
        CurrentState = states.patrol;
        base.Reset();
    }

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
            Stats myStats) {

            chase.Initialize(properties, controller, velBody, animator, myStats);
            patrol.Initialize(properties, controller, velBody, animator, myStats);
            swing.Initialize(properties, controller, velBody, animator, myStats);
            twitch.Initialize(properties, controller, velBody, animator, myStats);
            fall.Initialize(properties, controller, velBody, animator, myStats);
            fire.Initialize(properties, controller, velBody, animator, myStats);
            flee.Initialize(properties, controller, velBody, animator, myStats);
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
