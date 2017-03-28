using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hook : MonoBehaviour {

    #region Public fields
    #endregion

    #region Serialized Unity Inspector fields
    [SerializeField]
    private float growRate;
    [SerializeField]
    private float shrinkRate;
    [SerializeField]
    private float maxLength;
    [SerializeField]
    private float rotateSpeed;
    [SerializeField]
    private float damage;
    [SerializeField]
    private float damageMultiplier;
    [SerializeField]
    private float jumpForce;
    [SerializeField]
    private float jumpForceMultiplier;
    [SerializeField]
    private float pullForce;
    #endregion

    #region Private Fields
    private float initSize;
    private Vector3 initPos;
    GrapplerMod.SharedVariables sharedVariables;
    private bool charging;
    private bool isPullingWielder;
    private bool justStartedThrowing;
    #endregion

    #region Unity Lifecycle
    // Use this for initialization
    void Start () {
        initSize = transform.localScale.z;
        initPos = transform.localPosition;
        justStartedThrowing = true;        
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider other)
    {   
        if (other.gameObject.tag != Strings.Tags.MOD && other.gameObject.tag != Strings.Tags.PLAYERDETECTOR && sharedVariables.modSpot != ModSpot.Legs)
        {            
            if (sharedVariables.throwHook)
            {
                HitTarget();
            }
            if ((other.gameObject.tag == Strings.Tags.ENEMY || other.gameObject.tag == Strings.Tags.PLAYER) && (sharedVariables.throwHook || sharedVariables.retractHook))
            {
                IDamageable damageable = other.gameObject.GetComponent<IDamageable>();
                if (damageable != null)
                {
                    damageable.TakeDamage(damage);
                }
            }
        }
    }

    void HitTarget() {
        sharedVariables.throwHook = false;
        sharedVariables.retractHook = true;
        if (sharedVariables.specialAttack)
        {
            sharedVariables.specialAttack = false;
            isPullingWielder = true;
            sharedVariables.hitTargetThisShot = true;
        }
    }
    #endregion

    #region Public Methods
    public void Init(ref GrapplerMod.SharedVariables sharedVariables)
    {
        this.sharedVariables = sharedVariables;
    }
    public void Throw()
    {
        transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, transform.localScale.z + growRate * Time.deltaTime);
        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, transform.localPosition.z + growRate * Time.deltaTime / 2.0f);
        if (sharedVariables.modSpot == ModSpot.Legs && sharedVariables.wielderMovable.IsGrounded())
        {
            Jump();
        }
        if (transform.localScale.z > maxLength)
        {
            SwitchFromThrowToRetract();
        }
        if (justStartedThrowing) {
            justStartedThrowing = false;
            sharedVariables.hitTargetThisShot = false;
        }
    }

    void Jump() {
        float force = charging ? jumpForce * jumpForceMultiplier : jumpForce;
        sharedVariables.wielderMovable.AddDecayingForce(Vector3.up * force);
        charging = false;
    }

    void SwitchFromThrowToRetract() {
        transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, maxLength);
        sharedVariables.throwHook = false;
        sharedVariables.retractHook = true;
    }

    public void Retract()
    {
        transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, transform.localScale.z - shrinkRate * Time.deltaTime);
        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, transform.localPosition.z - shrinkRate * Time.deltaTime / 2.0f);
        if (isPullingWielder)
        {
            sharedVariables.wielderMovable.AddDecayingForce(sharedVariables.wielderMovable.GetForward() * pullForce);
        }        
        if (transform.localScale.z < initSize)
        {
            FinishRetracting();
        }
    }

    void FinishRetracting() {
        transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, initSize);
        transform.localPosition = initPos;
        sharedVariables.retractHook = false;
        isPullingWielder = false;
        sharedVariables.specialAttack = false;
        justStartedThrowing = true;                
    }

    public void Rotate()
    {
        transform.Rotate(Vector3.forward, rotateSpeed * Time.deltaTime);
        charging = true;
    }

    #endregion

    #region Private Methods
    #endregion

    #region Private Structures
    #endregion

}
