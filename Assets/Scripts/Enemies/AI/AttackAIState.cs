using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackAIState : IAIState
{
    public AttackAIState()
    {
//        m_CharacterAI.UpdateAnimationTo(AnimationStates.Swing);
    }

    public override void Update()
    {
        if(m_CharacterAI.TargetInAttackRange() == false)
        {
            m_CharacterAI.ChangeAIState(new IdleAIState());
            return;
        }

        m_CharacterAI.UpdateAnimationTo(AnimationStates.Swing);
        m_CharacterAI.Attack();
    }
}
