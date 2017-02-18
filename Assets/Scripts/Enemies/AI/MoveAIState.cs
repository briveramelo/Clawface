using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveAIState : IAIState
{
    public MoveAIState(){ }

    public override void Update()
    {
        m_CharacterAI.MoveTo(new Vector3(0, 0, 0));
    
        if(m_CharacterAI.GetPosition().x == 0)
        {
            m_CharacterAI.ChangeAIState(new RotateAIState());
        }
    }

}
