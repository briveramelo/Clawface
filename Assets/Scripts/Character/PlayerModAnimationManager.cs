﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerModAnimationManager : MonoBehaviour {

    private Dictionary<ModType, PlayerAnimationStates> modToAnimationMap = new Dictionary<ModType, PlayerAnimationStates>()
    {
        {ModType.StunBaton, PlayerAnimationStates.StunBatonR},
        {ModType.Boomerang, PlayerAnimationStates.BoomerangR}
    };
        
    [SerializeField] Animator animator;
    private Mod currentMod;
    private PlayerAnimationStates currentAnimationState;

	// Use this for initialization
	void Start () {
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
                    if(mod.getModSpot() == ModSpot.ArmL)
                    {
                        currentAnimationState++;
                    }
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
