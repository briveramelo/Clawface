using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerModAnimationManager : MonoBehaviour {

    private Dictionary<ModType, PlayerAnimationStates> modToAnimationMap = new Dictionary<ModType, PlayerAnimationStates>()
    {
        {ModType.StunBaton, PlayerAnimationStates.StunBaton}
    };
        
    [SerializeField] Animator animator;
    [SerializeField] AnimationClip[] animations;
    private PlayerStateManager.StateVariables stateVariables;
    private Mod currentMod;
    private PlayerAnimationStates currentAnimationState;

	// Use this for initialization
	void Start () {
        stateVariables = GetComponent<PlayerStateManager>().stateVariables;
        currentMod = null;
        currentAnimationState = PlayerAnimationStates.Idle;
    }

    public void PlayModAnimation(Mod mod, float frame)
    {
        if (mod != currentMod)
        {
            if (modToAnimationMap.ContainsKey(mod.getModType()))
            {
                if (modToAnimationMap.TryGetValue(mod.getModType(), out currentAnimationState))
                {
                    animator.Play(currentAnimationState.ToString(), -1, frame);
                    animator.speed = 0f;
                }
            }
        }else
        {
            animator.Play(currentAnimationState.ToString(), -1, frame);
            animator.speed = 0f;
        }
    }
}
