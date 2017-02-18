//MallCop AI created by Lai, Brandon, Bharat

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MallCopAI : MonoBehaviour, ICollectable, IStunnable, IMovable, IDamageable, ISkinnable
{
    #region 1. Public fields

    #endregion

    #region 2. Serialized Unity Inspector Fields
    [SerializeField]
    private GlowObject skinGlowScript;
    [SerializeField]
    private Stats myStats;
    [SerializeField]
    private GameObject mySkin;
    [SerializeField]
    private Rigidbody rigbod;
    [SerializeField]
    private Transform foot;
    [SerializeField]
    private Animator animator;
    [SerializeField]
    private Mod mod;

    [SerializeField]
    private float runMultiplier;
    [SerializeField, Range(5f, 15f)]
    private float attackTime;
    [SerializeField, Range(1, 6)]
    private int numShocksToStun;
    [SerializeField, Range(.1f, 1)]
    private float twitchRange;
    [SerializeField, Range(.1f, 1f)]
    private float twitchTime;
    [SerializeField, Range(30, 50)]
    private float rotationSpeed;
    #endregion

    #region 3. Private fields
    private MallCopState currentState = MallCopState.WALK;
    private GameObject attackTarget;
    private float rotationMultiplier;
    private Vector3 startStunPosition;
    private Vector3 velocity;
    private List<Vector3> externalForces;
    private int stunCount;
    private bool isGlowing = false;
    private bool isGrounded;
    private bool isFalling = false;
    private float sphereRadius = 0.1f;
    private OnDeath onDeath;
    private bool willHasBeenWritten;
    private bool inRange;
    private bool canAttack;
    private bool canFall;
    #endregion

    #region 4. Unity Lifecycle

    private void OnEnable()
    {
        if (willHasBeenWritten)
        {
            Revive();
        }
    }

    void Start()
    {
        externalForces = new List<Vector3>();

        for (int i = 0; i < 100; i++)
        {
            externalForces.Add(Vector3.zero);
        }
        Revive();

        PlayerMovement dummy = null;
        mod.setModSpot(ModSpot.ArmR);
        mod.AttachAffect(ref myStats, ref dummy);

    }

    // Update is called once per frame
    void Update()
    {
        isGrounded = IsGrounded();
        if (myStats.GetStat(StatType.Health) > 0)
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
            //velocity = rigbod.velocity;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == Strings.PLAYER && currentState != MallCopState.ATTACK && canAttack)
        {
            attackTarget = other.gameObject;
            currentState = MallCopState.ATTACK;
            inRange = true;
        }
        else if (other.gameObject.tag == Strings.PLAYER && canAttack)
        {
            inRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == Strings.PLAYER)
        {
            inRange = false;
        }
    }
    #endregion

    #region 5. Public Methods    
    public void DoDamage()
    {
        mod.Activate();
    }

    public void GetUp()
    {
        if (myStats.GetStat(StatType.Health) > 0)
        {
            animator.SetInteger(Strings.ANIMATIONSTATE, (int)MallCopAnimationStates.GettingUp);
        }
    }

    public void DoneGettingUp()
    {
        canFall = true;
        canAttack = true;
        currentState = MallCopState.ATTACK;
    }

    public void AttackAnimationDone()
    {
        canAttack = true;
    }

    void IDamageable.TakeDamage(float damage)
    {
        if (myStats.GetStat(StatType.Health) > 0)
        {
            myStats.TakeDamage(damage);
            if (myStats.GetStat(StatType.Health) <= 5 && !isGlowing)
            {
                isGlowing = true;
                skinGlowScript.SetToGlow();
            }
            if (myStats.GetStat(StatType.Health) <= 0)
            {
                if (animator.GetInteger(Strings.ANIMATIONSTATE) != (int)MallCopAnimationStates.Stunned)
                {
                    animator.SetInteger(Strings.ANIMATIONSTATE, (int)MallCopAnimationStates.Stunned);
                }
                GetComponent<Rigidbody>().isKinematic = true;

                mod.DetachAffect();
                Invoke("Die", 5f);
            }
        }
    }

    GameObject ICollectable.Collect()
    {
        GameObject droppedSkin = Instantiate(mySkin, null, true) as GameObject;
        return droppedSkin;
    }

    bool ISkinnable.IsSkinnable()
    {
        return myStats.GetStat(StatType.Health) <= 5;
    }

    GameObject ISkinnable.DeSkin()
    {
        Invoke("Die", 0.1f);
        return Instantiate(mySkin, null, false);
    }

    void IStunnable.Stun()
    {
        stunCount++;
        if (stunCount >= numShocksToStun)
        {
            stunCount = 0;
            startStunPosition = transform.position;
            currentState = MallCopState.STUNNED;
            canAttack = false;
        }
    }

    void IMovable.AddExternalForce(Vector3 forceVector, float decay)
    {
        StartCoroutine(AddPsuedoForce(forceVector, decay));
    }

    public bool HasWillBeenWritten() { return willHasBeenWritten; }

    public void RegisterDeathEvent(OnDeath onDeath)
    {
        willHasBeenWritten = true;
        this.onDeath = onDeath;
    }
    #endregion

    #region 6. Private Methods

    private void Die()
    {
        if (willHasBeenWritten)
        {
            onDeath();
        }
        skinGlowScript.ResetToNormal();
        gameObject.SetActive(false);
    }

    private void Revive()
    {
        myStats.ResetStats();
        isFalling = false;
        rigbod.isKinematic = false;
        rotationMultiplier = 1;// (Random.value > 0.5 ? 1 : -1 ) * Random.Range(.5f, 1.0f);
        inRange = false;
        canAttack = true;
        canFall = true;
        isGlowing = false;
        animator.SetInteger(Strings.ANIMATIONSTATE, 0);
        currentState = MallCopState.WALK;
        StopAllCoroutines();
        rigbod.velocity = Vector3.zero;
        //TODO check for missing mod and create a new one and attach it
        //TODO check for missing skin too! 
    }

    private bool IsGrounded()
    {
        Collider[] cols = Physics.OverlapSphere(foot.transform.position, sphereRadius);
        for (int i = 0; i < cols.Length; i++)
        {
            if (cols[i].gameObject.layer == (int)Layers.Ground)
            {
                return true;
            }
        }
        if (!isFalling)
        {
            StartCoroutine(ApplyGravity());
        }
        return false;
    }

    private void Walk()
    {
        if (animator.GetInteger(Strings.ANIMATIONSTATE) != (int)MallCopAnimationStates.Walk)
        {
            animator.SetInteger(Strings.ANIMATIONSTATE, (int)MallCopAnimationStates.Walk);
        }
        transform.Rotate(rotationMultiplier * rotationSpeed * Vector3.up * Time.deltaTime);

        Vector3 movementDirection = transform.forward;
        rigbod.velocity = movementDirection * myStats.GetStat(StatType.MoveSpeed) * Time.fixedDeltaTime + GetExternalForceSum();
    }

    private void Attack()
    {
        rigbod.velocity = GetExternalForceSum();
        if (canAttack && attackTarget != null)
        {

            Vector3 lookAtPosition = new Vector3(attackTarget.transform.position.x, 0, attackTarget.transform.position.z);
            transform.LookAt(lookAtPosition);
            transform.rotation = Quaternion.Euler(0f, transform.rotation.eulerAngles.y, 0f);

            if (inRange)
            {
                canAttack = false;
                if (animator.GetInteger(Strings.ANIMATIONSTATE) != (int)MallCopAnimationStates.Swing)
                {
                    animator.SetInteger(Strings.ANIMATIONSTATE, (int)MallCopAnimationStates.Swing);
                }
                else
                {
                    animator.Play(MallCopAnimationStates.Swing.ToString(), -1, 0f);
                }
            }
            else
            {
                if (animator.GetInteger(Strings.ANIMATIONSTATE) != (int)MallCopAnimationStates.Run)
                {
                    animator.SetInteger(Strings.ANIMATIONSTATE, (int)MallCopAnimationStates.Run);
                }
                Vector3 movementDirection = attackTarget.transform.position - transform.position;
                Vector3 movementDirectionXZ = new Vector3(movementDirection.x, 0, movementDirection.z);
                rigbod.velocity = movementDirectionXZ.normalized * myStats.GetStat(StatType.MoveSpeed) * runMultiplier * Time.fixedDeltaTime + GetExternalForceSum();
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

    private IEnumerator ApplyGravity()
    {
        isFalling = true;
        int currentIndex = externalForces.FindIndex(vec => vec == Vector3.zero);
        float timeElapsed = 0f;
        float gravity = 9.81f;
        while (!isGrounded && isFalling)
        {
            externalForces[currentIndex] = Vector3.down * (gravity * timeElapsed);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        isFalling = false;
        externalForces[currentIndex] = Vector3.zero;
    }

    private void Twitch()
    {
        if (canFall)
        {
            if (animator.GetInteger(Strings.ANIMATIONSTATE) != (int)MallCopAnimationStates.Stunned)
            {
                canFall = false;
                canAttack = false;
                animator.SetInteger(Strings.ANIMATIONSTATE, (int)MallCopAnimationStates.Stunned);
                //StartCoroutine(WaitForFallAnimation());
            }
        }
    }
    #endregion

    #region 7. Internal Structures
    public delegate void OnDeath();
    private enum MallCopAnimationStates
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

    private enum MallCopState
    {
        WALK = 0,
        ATTACK = 1,
        STUNNED = 2
    }
    #endregion

}
