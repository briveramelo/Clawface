//MallCop AI created by Lai

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MallCopAI : MonoBehaviour, ICollectable, IStunnable, IMovable, IDamageable, ISkinnable
{
    enum MallCopState
    {
        WALK = 0,
        ATTACK = 1,
        STUNNED =2
    }

    [SerializeField] GlowObject skinGlowScript;
    [SerializeField] Stats myStats;
    [SerializeField] GameObject mySkin;
    [SerializeField] Rigidbody rigbod;

    [SerializeField, Range(5f, 15f)] private float attackTime;
    [SerializeField, Range(1, 6)] private int numShocksToStun;
    [SerializeField, Range(.1f, 1)] private float twitchRange;
    [SerializeField, Range(.1f, 1f)] private float twitchTime;
    [SerializeField, Range(30,50)] private float rotationSpeed;

    private MallCopState currentState = MallCopState.WALK;
    private GameObject attackTarget;
    private float rotationMultiplier;
    private Vector3 startStunPosition;
    private List<Vector3> externalForces;
    private Rigidbody rigid;
    private int stunCount;
    private bool isGlowing = false;

    public delegate void OnDeath();
    private OnDeath onDeath;
    private bool willHasBeenWritten;

    public void RegisterDeathEvent(OnDeath onDeath) {
        this.onDeath += onDeath;
        willHasBeenWritten = true;
    }

    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
    }

    // Use this for initialization
    void Start ()
    {
        externalForces = new List<Vector3>();

        for (int i = 0; i < 100; i++)
        {
            externalForces.Add(Vector3.zero);
        }

        rotationMultiplier = 1;// (Random.value > 0.5 ? 1 : -1 ) * Random.Range(.5f, 1.0f);
    }

    void IDamageable.TakeDamage(float damage)
    {
        myStats.TakeDamage(damage);
        if (myStats.GetStat(StatType.Health) <= 5 && !isGlowing) {
            isGlowing = true;
            skinGlowScript.SetToGlow();
        }
        if (myStats.GetStat(StatType.Health) <= 0) {
            Destroy(gameObject);
        }
    }

    GameObject ICollectable.Collect()
    {
        GameObject droppedSkin = Instantiate(mySkin, null, true) as GameObject;
        return droppedSkin;
    }

    bool ISkinnable.IsSkinnable()
    {
        return myStats.GetStat(StatType.Health)<=5;
    }

    GameObject ISkinnable.DeSkin()
    {
        Destroy(gameObject, 0.1f);
        return Instantiate(mySkin, null, false);
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

    void IMovable.AddExternalForce(Vector3 forceVector, float decay)
    {
        StartCoroutine(AddPsuedoForce(forceVector, decay));
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

        Vector3 movementDirection = transform.forward;
        rigid.velocity = movementDirection * myStats.GetStat(StatType.MoveSpeed) * Time.fixedDeltaTime + GetExternalForceSum();
    }

    private void Attack()
    {        
        transform.LookAt(attackTarget.transform);
        Vector3 movementDirection = (attackTarget.transform.position - transform.position).normalized;

        rigid.velocity = movementDirection * myStats.GetStat(StatType.MoveSpeed) * Time.fixedDeltaTime + GetExternalForceSum();
        
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

    private Vector3 GetExternalForceSum()
    {
        Vector3 totalExternalForce = Vector3.zero;
        externalForces.ForEach(force => totalExternalForce += force);
        return totalExternalForce;
    }

    private IEnumerator AddPsuedoForce(Vector3 forceVector, float decay)
    {
        int currentIndex = externalForces.FindIndex(vec => vec == Vector3.zero);

        externalForces[currentIndex] = forceVector;

        while (externalForces[currentIndex].magnitude > .2f)
        {
            externalForces[currentIndex] = Vector3.Lerp(externalForces[currentIndex], Vector3.zero, decay);
            yield return null;
        }
        externalForces[currentIndex] = Vector3.zero;
    }

    private void ResetToWalking()
    {
        attackTime = 10.0f;
        currentState = MallCopState.WALK;
        attackTarget = null;
        rotationMultiplier = Random.Range(-1.0f, 1.0f);
    }

    private void OnDestroy()
    {
        if (willHasBeenWritten) {
            onDeath();
        }
    }

}
