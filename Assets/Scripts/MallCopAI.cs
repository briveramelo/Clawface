//MallCop AI created by Lai
//OnTriggerEnter has a magic string "Player". Fixed by using tag???

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MallCopAI : MonoBehaviour
{
    enum Action
    {
        WALK = 0,
        ATTACK = 1
    }

    [SerializeField] Stats m_state;

    public float attackTime = 10.0f;
    public float attackRange = 1.0f;

    private Action m_action = Action.WALK;
    private GameObject attackTarget;

    private float m_speed = 0.0f;
    private float m_rotation_parameter = 0.0f;

	// Use this for initialization
	void Start ()
    {
        m_speed = m_state.GetStat(StatType.MoveSpeed);
        m_rotation_parameter = Random.Range(-1.0f, 1.0f);
    }
	
	// Update is called once per frame
	void Update ()
    {
        switch (m_action)
        {
            case Action.WALK:
                Walk();
                break;
            case Action.ATTACK:
                Attack();
                break;
            default:
                break;
        }
    }

    void Walk()
    {
        float rotation_speed = 50.0f;
        transform.Rotate(m_rotation_parameter * rotation_speed * Vector3.up * Time.deltaTime);
        transform.Translate(m_speed * Vector3.forward * Time.deltaTime);
    }

    void Attack()
    {
        float distance = Vector3.Distance(attackTarget.transform.position, transform.position);

        if(distance > attackRange)
        {
            float chaseSpeed = m_speed * 2.0f;
            transform.position = Vector3.MoveTowards(transform.position, attackTarget.transform.position, chaseSpeed * Time.deltaTime);
            transform.LookAt(attackTarget.transform);
        }

        attackTime -= Time.deltaTime;

        if (attackTime <= 0.0f)
        {
            initState();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.name == "Player")
        {
            m_action = Action.ATTACK;
            attackTarget = other.gameObject;
        }
    }

    private void initState()
    {
        attackTime = 10.0f;
        m_action = Action.WALK;
        attackTarget = null;
        m_rotation_parameter = Random.Range(-1.0f, 1.0f);
    }
}
