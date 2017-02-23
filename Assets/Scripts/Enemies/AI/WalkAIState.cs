using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkAIState : IAIState
{
    bool m_bOnMove = false;
    Vector3 m_Position = Vector3.zero;

    public WalkAIState() { }

    public override void Update()
    {
        if (m_Position == Vector3.zero)
        {
            m_CharacterAI.UpdateAnimationTo(AnimationStates.Walk);
            GetMovePosition();
        }

        m_CharacterAI.MoveTo(m_Position);

        if (m_bOnMove)
        {
            float dis = Vector3.Distance(m_Position, m_CharacterAI.GetPosition());

            if (dis > 0.5f)
                return;

            GetMovePosition();
        }
        m_bOnMove = true;
    }

    private void GetMovePosition()
    {
        m_bOnMove = false;
        float range = 10.0f;
        Vector3 RandPosition = new Vector3(UnityEngine.Random.Range(-range, range), 0, UnityEngine.Random.Range(-range, range));
        m_Position = m_CharacterAI.GetPosition() + RandPosition;
    }
}