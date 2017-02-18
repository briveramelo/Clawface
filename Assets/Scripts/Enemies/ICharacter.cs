using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ICharacter
{
    protected GameObject m_GameObject = null;
    protected Rigidbody m_Rigidbody = null;
    protected ICharacterAI m_AI = null;

    public ICharacter() {}

    #region GameObject:
    public void SetGameObject(GameObject i_GameObject)
    {
        m_GameObject = i_GameObject;
        m_Rigidbody = m_GameObject.GetComponent<Rigidbody>();
    }

    public GameObject GetGameObject()
    {
        return m_GameObject;
    }
    #endregion

    #region Movement:
    public void MoveTo(Vector3 i_Position)
    {
        float step = 5.0f * Time.deltaTime;

        //Rotate and Face to target
        Vector3 targetDirection = i_Position - m_GameObject.transform.position;
        Vector3 newDirection = Vector3.RotateTowards(m_GameObject.transform.forward, targetDirection, step, 0.0F);
        m_GameObject.transform.rotation = Quaternion.LookRotation(newDirection);

        //Move to target
        Vector3 movementDirection = m_GameObject.transform.forward;
        m_Rigidbody.velocity = movementDirection * 200.0f * Time.fixedDeltaTime;
    }

    public Vector3 GetPosition()
    {
        return m_GameObject.transform.position;
    }
    #endregion

    #region AI:
    public void SetAI(ICharacterAI i_CharacterAI)
    {
        m_AI = i_CharacterAI;
    }

    public void UpdateAI()
    {
        m_AI.Update();
    }
    #endregion
}


