using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ICharacter
{
    protected GameObject m_GameObject = null;
    protected GameObject m_Target = null;
    protected ICharacterAI m_AI = null;
    protected IAnimation m_Animation = null;

    protected Rigidbody m_Rigidbody = null;

    public ICharacter(GameObject i_Target)
    {
        m_Target = i_Target;
    }

    #region GameObject:
    public void SetGameObject(GameObject i_GameObject)
    {
        m_GameObject = i_GameObject;
        m_Rigidbody = m_GameObject.GetComponent<Rigidbody>();
        m_Animation = new IAnimation(this);
    }

    public GameObject GetGameObject()
    {
        return m_GameObject;
    }
    #endregion

    #region Target (Player): 
    public GameObject GetTarget()
    {
        return m_Target;
    }
    #endregion

    #region Action:
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

    public void StopMove()
    {
        m_Rigidbody.velocity = new Vector3(0, 0, 0);
    }

    public void FaceToTarget()
    {
        float step = 5.0f * Time.deltaTime;
        Vector3 targetDirection = m_Target.transform.position - m_GameObject.transform.position;
        targetDirection.y = 0;
        Vector3 newDirection = Vector3.RotateTowards(m_GameObject.transform.forward, targetDirection, step, 0.0F);
        m_GameObject.transform.rotation = Quaternion.LookRotation(newDirection);
    }

    public void Attack()
    {
        Debug.Log("I am Attacking you");
    }

    public Vector3 GetPosition()
    {
        return m_GameObject.transform.position;
    }
    #endregion

    #region Animation:
    
    public void UpdateAnimationTo(AnimationStates i_State)
    {
        if(m_Animation != null)
            m_Animation.GetAnimator().SetInteger(Strings.ANIMATIONSTATE, (int)i_State);
    }
    #endregion


    #region AI:
    public void SetAI(ICharacterAI i_CharacterAI)
    {
        m_AI = i_CharacterAI;
    }

    public void UpdateAI()
    {
        Debug.Log(m_Animation);
        m_AI.Update();
    }
    #endregion
}


