using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerModAnimationManager : MonoBehaviour {

    [SerializeField] Animator animator;
    private Mod currentMod;
    private PlayerAnimationStates currentAnimationState;

	// Use this for initialization
	void Start () {
        currentMod = null;
        currentAnimationState = PlayerAnimationStates.Idle;
    }

    public void PlayModAnimation(Mod mod, int frame, int totalFrames)
    {
        animator.Play(currentAnimationState.ToString(), -1, frame);
        animator.speed = 0f;
    }
}
