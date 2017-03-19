using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MallCopController : AIController {

    public string mystate;
    private MallCopProperties properties;
    private Mod mod;
    private States states;

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

    public void Initialize(
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

    protected override void Update() {

        if (CurrentState == states.chase &&
            timeInLastState > properties.maxChaseTime &&
            attackTarget!=null) {

            CurrentState = states.patrol;            
        }        
        base.Update();
    }


    private void OnTriggerStay(Collider other) {
        if ((other.gameObject.tag == Strings.Tags.PLAYER) &&
            CurrentState != states.chase && CurrentState!=states.swing) {

            attackTarget = other.gameObject;
            CurrentState = states.chase;
        }
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

    private class States {
        public MallCopPatrolState patrol = new MallCopPatrolState();
        public MallCopChaseState chase= new MallCopChaseState();
        public MallCopSwingState swing = new MallCopSwingState();
        public MallCopTwitchState twitch = new MallCopTwitchState();
        public MallCopFallState fall = new MallCopFallState();

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
    Twitch = 5
}
