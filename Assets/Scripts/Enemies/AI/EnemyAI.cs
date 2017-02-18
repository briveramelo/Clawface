using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : ICharacterAI
{
    public EnemyAI(ICharacter i_Character) : base(i_Character)
    {
        ChangeAIState(new IdleAIState());
    }

    public override void ChangeAIState(IAIState i_AIState)
    {
        base.ChangeAIState(i_AIState);
    }
}