using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaiMallCopAI : EnemyAI
{
    public LaiMallCopAI(ICharacter i_Character) : base(i_Character) {}

    public override void Update()
    {
        if (TargetInAttackRange())
        {
            if(!IsSameAIState(typeof(AttackAIState)))
                ChangeAIState(new AttackAIState());
        }
        else if (TargetInChaseRange())
        {
            if (!IsSameAIState(typeof(ChaseAIState)))
                ChangeAIState(new ChaseAIState());
        }
        else
        {
            if (!IsSameAIState(typeof(WalkAIState)))
                ChangeAIState(new WalkAIState());
        }

        m_AIState.Update();
    }
}



