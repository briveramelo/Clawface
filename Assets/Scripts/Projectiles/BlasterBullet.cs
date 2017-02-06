using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlasterBullet : MonoBehaviour {

    Vector3 direction;

    float pushForce;
    float damage = 1f;
    bool push;

    [SerializeField]
    float speed;

	// Use this for initialization
	void Start () {
        direction = Vector3.forward;
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
            gameObject.SetActive(false);
        }
    }

    void OnDisable()
    {
        StopAllCoroutines();
    }

    // Update is called once per frame
    void Update () {
        transform.Translate(direction * speed);
	}

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == Strings.ENEMY && push)
        {
            Vector3 forceDirection = other.gameObject.transform.position - transform.position;
            IMovable movable = other.gameObject.GetComponent<IMovable>();
            if (movable != null)
            {
                movable.AddExternalForce(forceDirection.normalized * pushForce);
            }
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag != Strings.PLAYER)
        {
            if (other.gameObject.tag == Strings.ENEMY)
            {
                IDamageable damageable = other.gameObject.GetComponent<IDamageable>();
                if (damageable != null)
                {
                    damageable.TakeDamage(damage);
                }
            }
            push = true;
            gameObject.SetActive(false);
        }
    }
}
