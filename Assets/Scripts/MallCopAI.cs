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
        ATTACK = 1,
        STUNNED =2
    }

    [SerializeField] Stats myStats;
    [SerializeField] GameObject skin;
    [SerializeField] Rigidbody rigbod;

    [SerializeField, Range(5f, 15f)] private float attackTime;
    [SerializeField, Range(1,2)] private float maxRunSpeedMultiplier;
    [SerializeField, Range(2,10)] private float redirectionAccelerationMultipler;
    [SerializeField, Range(350,600)] private float acceleration;
    [SerializeField, Range(1, 6)] private int numShocksToStun;
    [SerializeField, Range(.1f, 1)] private float twitchRange;
    [SerializeField, Range(.1f, 1f)] private float twitchTime;
    [SerializeField, Range(30,50)] private float rotationSpeed;

    private MallCopState currentState = MallCopState.WALK;
    private GameObject attackTarget;
    float rotationMultiplier;
    Vector3 startStunPosition;

    int stunCount;


	// Use this for initialization
	void Start ()
    {
        rotationMultiplier = 1;// (Random.value > 0.5 ? 1 : -1 ) * Random.Range(.5f, 1.0f);
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
        stunCount++;
        if (stunCount >= numShocksToStun) {
            stunCount = 0;
            startStunPosition = transform.position;
            currentState = MallCopState.STUNNED;
            Invoke("SetMallCopToWalk", twitchTime);
        }
    }

    void IMovable.AddExternalForce(Vector3 force)
    {
        rigbod.AddForce(force);
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
            case MallCopState.STUNNED:
                Twitch();
                break;
        }
    }

    private void SetMallCopToWalk() {
        currentState = MallCopState.WALK;
    }

    private void Walk()
    {
        transform.Rotate(rotationMultiplier * rotationSpeed * Vector3.up * Time.deltaTime);
        if (ShouldAddMoreForce(myStats.GetStat(StatType.MoveSpeed))) {
            Vector3 movementDirection = transform.forward;
            float movementForce = acceleration * Time.deltaTime;
            rigbod.AddForce(movementDirection * movementForce);
        }
    }

    private bool ShouldAddMoreForce(float maxSpeed) {
        float speedProjection = Vector3.Dot(rigbod.velocity, transform.forward);
        return speedProjection < maxSpeed;
    }

    private bool ShouldRunQuickly() {
        float speedProjection = Vector3.Dot(rigbod.velocity, transform.forward);
        return speedProjection < 0.1f;
    }

    private void Attack()
    {        
        transform.LookAt(attackTarget.transform);
        Vector3 movementDirection = (attackTarget.transform.position - transform.position).normalized;
        float movementForce = acceleration * Time.deltaTime;
        if (ShouldAddMoreForce(myStats.GetStat(StatType.MoveSpeed) * maxRunSpeedMultiplier)) {
            if (ShouldRunQuickly()){
                movementForce *= redirectionAccelerationMultipler;
            }
            rigbod.AddForce(movementDirection * movementForce);
        }            

        attackTime -= Time.deltaTime;

        if (attackTime <= 0.0f)
        {
            ResetToWalking();
        }
    }

    private void Twitch() {
        //TODO: Make this allow getting pushed
        Vector3 newPosition = Random.insideUnitSphere * twitchRange;
        newPosition.z = Mathf.Abs(newPosition.z);
        transform.position = startStunPosition + newPosition;
    }

    private void OnTriggerStay(Collider other)
    {
        if (currentState==MallCopState.WALK) {
            if (other.name == Strings.PLAYER)
            {
                attackTarget = other.gameObject;
                currentState = MallCopState.ATTACK;
            }
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
