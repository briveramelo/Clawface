using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GrapplingBotController : AIController {

    public EGrapplingBotState ECurrentState {
        get {
            return states.GetEState((GrapplingBotState)CurrentState);
        }
    }

    protected GrapplingBotProperties properties;
    protected Mod mod;
    protected States states;
    protected Animator animator;    

    public virtual void Initialize(
        GrapplingBotProperties properties,
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

    public void UpdateState(EGrapplingBotState state) {
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
        CurrentState = states.approach;
    }
    #endregion

    protected class States {
        public GrapplingBotPatrolState patrol = new GrapplingBotPatrolState();
        public GrapplingBotApproachState approach = new GrapplingBotApproachState();
        public GrapplingBotExplodeState explode = new GrapplingBotExplodeState();
        public GrapplingBotTwitchState twitch = new GrapplingBotTwitchState();
        public GrapplingBotGrappleState grapple = new GrapplingBotGrappleState();


        private Dictionary<EGrapplingBotState, GrapplingBotState> grapplingBotStates;

        public void Initialize(
            GrapplingBotProperties properties,
            GrapplingBotController controller,
            VelocityBody velBody,
            Animator animator,
            Stats stats) {

            patrol.Initialize(properties, controller, velBody, animator, stats);
            approach.Initialize(properties, controller, velBody, animator, stats);
            explode.Initialize(properties, controller, velBody, animator, stats);
            twitch.Initialize(properties, controller, velBody, animator, stats);
            grapple.Initialize(properties, controller, velBody, animator, stats);            

            grapplingBotStates = new Dictionary<EGrapplingBotState, GrapplingBotState>() {
                {EGrapplingBotState.Patrol, patrol },
                {EGrapplingBotState.Approach, approach },
                {EGrapplingBotState.Explode, explode },
                {EGrapplingBotState.Twitch, twitch },
                {EGrapplingBotState.Grapple, grapple },
            };
        }

        public GrapplingBotState GetState(EGrapplingBotState state) {
            return grapplingBotStates[state];
        }
        public EGrapplingBotState GetEState(GrapplingBotState state) {
            EGrapplingBotState key = grapplingBotStates.FirstOrDefault(x => x.Value == state).Key;
            return key;
        }
    }
}
