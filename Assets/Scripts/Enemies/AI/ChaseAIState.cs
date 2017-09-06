using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class ChaseAIState : IAIState
{
    public ChaseAIState() { }

    public override void Update()
    {
        m_CharacterAI.UpdateAnimationTo(AnimationStates.Run);
        m_CharacterAI.RunTo(m_CharacterAI.GetTarget().gameObject.transform.position);
    }
}
