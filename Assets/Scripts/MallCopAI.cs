//MallCop AI created by Lai
//OnTriggerEnter has a magic string "Player". Fixed by using tag???

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MallCopAI : MonoBehaviour, ICollectable, IStunnable, IMovable, IDamageable
{
    enum MallCopState
    {
        WALK = 0,
        ATTACK = 1
    }

    [SerializeField] Stats myStats;
    [SerializeField] GameObject skin;

    public float attackTime = 10.0f;
    public float attackRange = 1.0f;

    private MallCopState currentState = MallCopState.WALK;
    private GameObject attackTarget;

    private float rotationMultiplier = 0.0f;

	// Use this for initialization
	void Start ()
    {
        rotationMultiplier = Random.Range(-1.0f, 1.0f);
    }

    void IDamageable.TakeDamage(float damage)
    {
        myStats.TakeDamage(damage);
        if (myStats.GetStat(StatType.Health) <= 0) {
            Destroy(gameObject);
        }
    }

    GameObject ICollectable.Collect()
    {
        GameObject droppedSkin = Instantiate(skin, null, true) as GameObject;
        return droppedSkin;
    }

    bool ICollectable.IsCollectable()
    {
        return myStats.GetStat(StatType.Health)<2;
    }

    void IStunnable.Stun()
    {
        
    }

    void IMovable.AddExternalForce(Vector3 force)
    {
        
    }

    // Update is called once per frame
    void Update ()
    {
        switch (currentState)
        {
            case MallCopState.WALK:
                Walk();
                break;
            case MallCopState.ATTACK:
                Attack();
                break;
            default:
                break;
        }
    }

    void Walk()
    {
        float rotationSpeed = 50.0f;
        transform.Rotate(rotationMultiplier * rotationSpeed * Vector3.up * Time.deltaTime);
        transform.Translate(myStats.GetStat(StatType.MoveSpeed) * Vector3.forward * Time.deltaTime);
    }

    void Attack()
    {
        float distance = Vector3.Distance(attackTarget.transform.position, transform.position);

        if(distance > attackRange)
        {
            float chaseSpeed = myStats.GetStat(StatType.MoveSpeed) * 2.0f;
            transform.position = Vector3.MoveTowards(transform.position, attackTarget.transform.position, chaseSpeed * Time.deltaTime);
            transform.LookAt(attackTarget.transform);
        }

        attackTime -= Time.deltaTime;

        if (attackTime <= 0.0f)
        {
            ResetToWalking();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.name == Strings.PLAYER)
        {
            currentState = MallCopState.ATTACK;
            attackTarget = other.gameObject;
        }
    }

    private void ResetToWalking()
    {
        attackTime = 10.0f;
        currentState = MallCopState.WALK;
        attackTarget = null;
        rotationMultiplier = Random.Range(-1.0f, 1.0f);
    }

    
}
