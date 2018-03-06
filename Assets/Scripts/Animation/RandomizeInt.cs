using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomizeInt : StateMachineBehaviour 
{
    public string parameterName;
    public int min = 0;
    public int max = 0;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //base.OnStateEnter(animator, stateInfo, layerIndex);
        //animator.SetInteger (parameterName, Random.Range (min, max + 1));
    }
}
