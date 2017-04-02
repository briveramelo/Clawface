﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public abstract class MallCopController : AIController {

    public EMallCopState ECurrentState {
        get {            
            return states.GetEState((MallCopState)CurrentState);
        }
    }

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
        CurrentState = states.GetState(state);
    }

    public override void ResetForRebirth() {
        CurrentState = states.patrol;
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
        public MallCopPatrolState patrol = new MallCopPatrolState();
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
            Stats stats) {

            chase.Initialize(properties, controller, velBody, animator, stats);
            patrol.Initialize(properties, controller, velBody, animator, stats);
            swing.Initialize(properties, controller, velBody, animator, stats);
            twitch.Initialize(properties, controller, velBody, animator, stats);
            fall.Initialize(properties, controller, velBody, animator, stats);
            fire.Initialize(properties, controller, velBody, animator, stats);
            flee.Initialize(properties, controller, velBody, animator, stats);

            mallCopStates = new Dictionary<EMallCopState, MallCopState>() {
                {EMallCopState.Swing, swing },
                {EMallCopState.Fall, fall },
                {EMallCopState.Patrol, patrol },
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

