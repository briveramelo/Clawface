using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlasterBullet : MonoBehaviour {

    [HideInInspector] public bool isCharged;

    [SerializeField] private float speed;
    [SerializeField] private float damage;
    [SerializeField] private float damageMultiplierCharged;

    private VFXHandler vfxHandler;
    private Vector3 moveDirection;
    private float pushForce;
    private int shooterInstanceID;

	// Use this for initialization
	void Start () {        
        vfxHandler = new VFXHandler(transform);
        moveDirection = Vector3.forward;
    }

    void OnEnable()
    {        
        isCharged = false;
        StartCoroutine(DestroyAfter());
    }

    IEnumerator DestroyAfter() {
        if (gameObject.activeSelf)
        {
            yield return new WaitForSeconds(3);
            vfxHandler.EmitForBulletCollision();
            gameObject.SetActive(false);
        }
    }

    void OnDisable()
    {
        isCharged = false;
        StopAllCoroutines();
    }

    // Update is called once per frame
    void Update () {
        transform.Translate(moveDirection* speed * Time.deltaTime);
	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetInstanceID()!=shooterInstanceID)
        {
            bool isEnemy = other.gameObject.CompareTag(Strings.Tags.ENEMY);
            bool isPlayer = other.gameObject.CompareTag(Strings.Tags.PLAYER);
            
            if (isEnemy || isPlayer)
            {
                Damage(other.gameObject.GetComponent<IDamageable>());
                Push(other.gameObject.GetComponent<IMovable>());
                if (isEnemy) {
                    EmitBlood();                                              
                }
                //TODO find a better method for colliding with ground
                //right now it's unreliable            
                gameObject.SetActive(false);
            }                        
        }
    }

    public void SetShooterInstanceID(int shooterInstanceID) {
        this.shooterInstanceID = shooterInstanceID;
    }

    private void Damage(IDamageable damageable) {        
        if (damageable != null) {
            damageable.TakeDamage(isCharged ? damage * damageMultiplierCharged : damage);
        }
    }

    private void Push(IMovable movable) {
        Vector3 forceDirection = transform.forward;        
        if (movable != null) {
            movable.AddDecayingForce(forceDirection.normalized * pushForce);
        }
    }

    private void EmitBlood() {
        //TODO: Create Impact effect needs to take into account
        // the type of surface that it had hit.
        if (Mathf.Abs(transform.forward.y) < 0.5f) {
            vfxHandler.EmitBloodBilaterally();
        }
        else {
            vfxHandler.EmitBloodInDirection(Quaternion.Euler(Vector3.right * 90f), transform.position);
        }
    }
}
