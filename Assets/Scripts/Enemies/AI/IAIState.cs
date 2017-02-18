using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IAIState
{
    protected ICharacterAI m_CharacterAI = null;

    public IAIState()
    {

    }

    public void setCharacterAI(ICharacterAI i_CharacterAI)
    {
        m_CharacterAI = i_CharacterAI;
    }

    public abstract void Update();
}
