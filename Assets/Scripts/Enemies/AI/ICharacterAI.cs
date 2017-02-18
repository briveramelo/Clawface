using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ICharacterAI
{
    protected ICharacter m_Character = null;
    protected IAIState m_AIState = null;

    public ICharacterAI(ICharacter i_Character)
    {
        m_Character = i_Character;
    }

    public Vector3 GetPosition()
    {
        return m_Character.GetGameObject().transform.position;
    }

    public virtual void ChangeAIState(IAIState i_AIState)
    {
        m_AIState = i_AIState;
        m_AIState.setCharacterAI(this);
    }

    public void MoveTo(Vector3 i_Position)
    {
        m_Character.MoveTo(i_Position);
    }

    public void Update()
    {
        m_AIState.Update();
    }
}
