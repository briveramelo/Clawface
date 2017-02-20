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

	// Use this for initialization
	void Start () {
        modActive = false;
        isPlaying = false;
    }
	
	// Update is called once per frame
	void Update () {
        if (!HitstopManager.Instance.IsInHitstop())
        {
            animator.enabled = true;
        }
        else
        {
            animator.enabled = false;
        }
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
                    StartCoroutine(WaitForAnimation());
                }
                else
                {
                    isPlaying = true;
                    PlayerAnimationStates state = (PlayerAnimationStates)animationState;
                    animator.Play(state.ToString(), -1, 0f);
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
        while(animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.95)
        {
            yield return null;
        }
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
    }

    public bool GetIsPlaying()
    {
        return isPlaying;
    }
}
