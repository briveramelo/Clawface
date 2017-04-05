//Brandon Rivera-Melo

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MovementEffects;

public class MallCopTwitchState : MallCopState {

    private Vector3 startStunPosition;

    public override void OnEnter() {
        animator.SetInteger(Strings.ANIMATIONSTATE, (int)MallCopAnimationStates.HitReaction);        
        Timing.RunCoroutine(Twitch());
    }
    public override void Update() {
        
    }
    public override void OnExit() {
        
    }

    bool isMidTwitch;
    public bool IsMidTwitch() {
        return isMidTwitch;
    }

    private IEnumerator<float> Twitch() {
        isMidTwitch = true;
        velBody.velocity = Vector3.zero;
        startStunPosition = velBody.transform.position;
        float twitchRadius = properties.twitchRange;
        float twitchTime = properties.twitchTime;
        while (twitchTime > 0) {
            velBody.transform.position = startStunPosition + Random.onUnitSphere * twitchRadius;
            twitchTime -= Time.deltaTime;
            yield return Timing.WaitForOneFrame;
        }
        velBody.transform.position = startStunPosition;
        isMidTwitch = false;                
    }
}
