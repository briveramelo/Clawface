﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ICharacterAI
{
    protected ICharacter m_Character = null;
    protected IAIState m_AIState = null;

    protected float m_AttackRange = 10.0f;
    protected const float ATTACK_COOLDOWN = 2.0f;
    protected float m_AttackCoolDown = ATTACK_COOLDOWN;

    public ICharacterAI(ICharacter i_Character)
    {
        m_Character = i_Character;
    }

    public virtual void ChangeAIState(IAIState i_AIState)
    {
        m_AIState = i_AIState;
        m_AIState.SetCharacterAI(this);
    }

    public Vector3 GetPosition()
    {
        return m_Character.GetGameObject().transform.position;
    }

    public void MoveTo(Vector3 i_Position)
    {
        m_Character.MoveTo(i_Position);
    }

    public void StopMove()
    {
        m_Character.StopMove();
    }

    public void FaceToTarget()
    {
        m_Character.FaceToTarget();
    }

    public virtual void Attack()
    {
        FaceToTarget();
        StopMove();

        m_AttackCoolDown -= Time.deltaTime;

        if (m_AttackCoolDown >= 0.0f)
            return;

        m_AttackCoolDown = ATTACK_COOLDOWN;
        m_Character.Attack();
    }
    
    public void UpdateAnimationTo(AnimationStates i_State)
    {
        m_Character.UpdateAnimationTo(i_State);
    }
    
    public bool TargetInAttackRange()
    {
        float distance = Vector3.Distance(m_Character.GetPosition(), m_Character.GetTarget().transform.position);
        return (distance <= m_AttackRange);
    }

    public void Update()
    {
        Debug.Log(m_AIState);
        m_AIState.Update();
    }
}
