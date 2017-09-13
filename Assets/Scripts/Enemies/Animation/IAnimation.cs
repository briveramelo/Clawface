using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AnimationStates
{
    Idle = 0,
    Walk = 1,
    Swing = 2,
    HitReaction = 3,
    Stunned = 4,
    GettingUp = 5,
    DrawWeapon = 6,
    Run = 7,
    Shoot = 8
}

public class IAnimation
{
    protected ICharacter m_Character = null;
    protected Animator m_Animator = null;

    public IAnimation(ICharacter i_Character)
    {
        m_Character = i_Character;
        m_Animator = m_Character.GetGameObject().GetComponent<Animator>();
    }

    public Animator GetAnimator()
    {
        return m_Animator;
    }
}
