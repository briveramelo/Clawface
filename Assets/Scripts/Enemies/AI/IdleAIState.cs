using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleAIState : IAIState
{
    public IdleAIState() { }

    bool m_bOnMove = false;

    Vector3 m_Position = Vector3.zero;

    public override void Update()
    {
        if (m_Position == Vector3.zero)
            GetMovePosition();

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
        Vector3 rand = new Vector3(UnityEngine.Random.Range(-10.0f, 10.0f), 0, UnityEngine.Random.Range(-10.0f, 10.0f));
        m_Position = m_CharacterAI.GetPosition() + rand;
    }

}