using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerModAnimationManager : MonoBehaviour {

    private Dictionary<ModType, PlayerAnimationStates> modToAnimationMap = new Dictionary<ModType, PlayerAnimationStates>()
    {
        {ModType.StunBaton, PlayerAnimationStates.MeleeRight}
    };
        
    [SerializeField] Animator animator;
    bool isPlaying;
    Mod currentMod;
    bool modActive;
    PlayerStateManager.StateVariables stateVariables;

	// Use this for initialization
	void Start () {
        modActive = false;
        isPlaying = false;
        stateVariables = GetComponent<PlayerStateManager>().stateVariables;
    }

    public void PlayModAnimation(Mod mod, bool isMoving)
    {
        if (modToAnimationMap.ContainsKey(mod.getModType()))
        {
            if (!isPlaying)
            {
                currentMod = mod;
                int animationState = (int)modToAnimationMap[currentMod.getModType()];
                if (mod.getModSpot() == ModSpot.ArmL)
                {
                    animationState += 2;
                }
                if (isMoving)
                {
                    animationState++;
                }
                if (animator.GetInteger(Strings.ANIMATIONSTATE) != animationState)
                {
                    isPlaying = true;
                    animator.SetInteger(Strings.ANIMATIONSTATE, animationState);
                    StopAllCoroutines();
                    StartCoroutine(WaitForAnimation());
                }
                else
                {
                    isPlaying = true;
                    PlayerAnimationStates state = (PlayerAnimationStates)animationState;
                    animator.Play(state.ToString(), -1, 0f);
                    StopAllCoroutines();
                    StartCoroutine(WaitForAnimation());
                }
            }
        }
        else
        {
            mod.Activate();
        }
    }

    IEnumerator WaitForAnimation()
    {
        //print("Waiting for animation "+ animator.GetInteger(Strings.ANIMATIONSTATE) + " to finish");
        while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.95)
        {
            yield return null;
        }
        //print("Animation " + animator.GetInteger(Strings.ANIMATIONSTATE) + " finished");
        AnimationDone();
    }

    public void ActivateMod()
    {
        currentMod.Activate();
    }

    public void DeActivateMod()
    {
        modActive = false;
    }

    public void AnimationDone()
    {
        isPlaying = false;
        stateVariables.stateFinished = true;
    }

    public bool GetIsPlaying()
    {
        return isPlaying;
    }
}
