using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackAIState : IAIState
{
    public AttackAIState()
    {
    }

    public override void Update()
    {
        m_CharacterAI.UpdateAnimationTo(AnimationStates.Swing);
        m_CharacterAI.Attack();
    }
}
