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
    #endregion

    #region Private Fields
    private float initSize;
    private Vector3 initPos;
    GrapplerMod.SharedVariables sharedVariables;
    private bool pullPlayer;
    private bool charging;
    #endregion

    #region Unity Lifecycle
    // Use this for initialization
    void Start () {
        initSize = transform.localScale.z;
        initPos = transform.localPosition;
        pullPlayer = false;
        charging = false;
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
                sharedVariables.throwHook = false;
                sharedVariables.retractHook = true;
                if(sharedVariables.specialAttack)
                {
                    sharedVariables.specialAttack = false;
                    pullPlayer = true;
                }
            }
            if (other.gameObject.tag == Strings.Tags.ENEMY && (sharedVariables.throwHook || sharedVariables.retractHook))
            {
                IDamageable damageable = other.gameObject.GetComponent<IDamageable>();
                if (damageable != null)
                {
                    damageable.TakeDamage(damage);
                }
            }
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
            float force = charging ? jumpForce * jumpForceMultiplier : jumpForce;
            sharedVariables.wielderMovable.AddDecayingForce(Vector3.up * force);
            charging = false;
        }
        if (transform.localScale.z > maxLength)
        {
            transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, maxLength);
            sharedVariables.throwHook = false;
            sharedVariables.retractHook = true;
        }
    }

    public void Retract()
    {
        transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, transform.localScale.z - shrinkRate * Time.deltaTime);
        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, transform.localPosition.z - shrinkRate * Time.deltaTime / 2.0f);
        if (pullPlayer)
        {
            sharedVariables.wielderMovable.AddDecayingForce(sharedVariables.wielderMovable.GetForward() * shrinkRate);
        }        
        if (transform.localScale.z < initSize)
        {
            transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, initSize);
            transform.localPosition = initPos;
            sharedVariables.retractHook = false;
            pullPlayer = false;
            sharedVariables.specialAttack = false;
        }
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
