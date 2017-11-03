using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomizeFloat : StateMachineBehaviour 
{
    public string floatName;
    public float min = 0f;
    public float max = 0f;

	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	    base.OnStateEnter(animator, stateInfo, layerIndex);
        animator.SetFloat (floatName, Random.Range(min, max));
	}
}
