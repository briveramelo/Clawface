using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlasterBullet : MonoBehaviour {

    [SerializeField] private float speed;

    private Vector3 direction;
    private float pushForce;
    [SerializeField]
    private float damage;
    [SerializeField]
    private float damageMultiplier;
    private bool push;

    public bool isCharged;

	// Use this for initialization
	void Start () {
        direction = Vector3.forward;
        push = false;        
    }

    void OnEnable()
    {        
        isCharged = false;
        push = false;        
        StartCoroutine(DestroyAfter());
    }

    IEnumerator DestroyAfter() {
        if (gameObject.activeSelf)
        {
            yield return new WaitForSeconds(3);
            CreateImpactEffect();
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
        transform.Translate(direction * speed);
	}

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == Strings.Tags.ENEMY && push)
        {
            Vector3 forceDirection = other.gameObject.transform.position - transform.position;
            IMovable movable = other.gameObject.GetComponent<IMovable>();
            if (movable != null)
            {
                movable.AddDecayingForce(forceDirection.normalized * pushForce);
            }
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag != Strings.Tags.PLAYER)
        {
            if (other.gameObject.tag == Strings.Tags.ENEMY)
            {
                IDamageable damageable = other.gameObject.GetComponent<IDamageable>();
                if (damageable != null)
                {
                    damageable.TakeDamage(isCharged ? damage * damageMultiplier : damage);
                }
            }
            push = true;
            CreateImpactEffect();
            //TODO find a better method for colliding with ground
            //right not it's unreliable            
            gameObject.SetActive(false);
        }
    }

    private void CreateImpactEffect() {
        GameObject projectileImpactEffect = ObjectPool.Instance.GetObject(PoolObjectType.BlasterImpactEffect);
        projectileImpactEffect.SetActive(true);
        projectileImpactEffect.transform.position = transform.position;
        projectileImpactEffect.transform.rotation = transform.rotation;
    }
}
