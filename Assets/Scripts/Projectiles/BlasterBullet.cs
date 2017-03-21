using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlasterBullet : MonoBehaviour {

    [SerializeField] private float speed;

    private VFXHandler vfxHandler;
    private Vector3 moveDirection;
    private float pushForce;
    private float damage = 1f;
    private bool push;


	// Use this for initialization
	void Start () {
        vfxHandler = new VFXHandler(transform);
        moveDirection = Vector3.forward;
        push = false;
    }

    void OnEnable()
    {
        push = false;        
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
        StopAllCoroutines();
    }

    // Update is called once per frame
    void Update () {
        transform.Translate(moveDirection* speed * Time.deltaTime);
	}

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == Strings.Tags.ENEMY && push)
        {
            Vector3 forceDirection = transform.forward;
            IMovable movable = other.GetComponent<IMovable>();
            if (movable != null)
            {
                movable.AddDecayingForce(forceDirection.normalized * pushForce);
            }
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        //print("Bullet hit " + other.gameObject.name);
        if (other.gameObject.tag != Strings.Tags.PLAYER)
        {
            if (other.gameObject.tag == Strings.Tags.ENEMY)
            {
                IDamageable damageable = other.gameObject.GetComponent<IDamageable>();
                if (damageable != null)
                {
                    damageable.TakeDamage(damage);
                }

                //TODO: Create Impact effect needs to take into account
                // the type of surface that it had hit.
                if (Mathf.Abs(transform.forward.y)<0.5f) {
                    vfxHandler.EmitBloodBilaterally();
                }
                else {                    
                    vfxHandler.EmitBloodInDirection(Quaternion.Euler(Vector3.right*90f), transform.position);
                }
            }
            push = true;
            
            //TODO find a better method for colliding with ground
            //right now it's unreliable            
            gameObject.SetActive(false);
        }
    }

    
}
