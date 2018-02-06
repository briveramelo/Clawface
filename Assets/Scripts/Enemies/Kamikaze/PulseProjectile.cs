using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PulseProjectile : MonoBehaviour
{

    [SerializeField] private float scaleRate;
    [SerializeField] private float maxScale;
    [SerializeField] private SphereCollider sphereCollider;

    private Damager damager = new Damager();
    private float scaleValue = 0.0f;

    private void OnEnable()
    {
        scaleValue = 0.0f;
        transform.localScale = new Vector3(scaleValue, 0.1f, scaleValue);

    }

    private void OnTriggerEnter(Collider other)
    {
            if (other.gameObject.CompareTag(Strings.Tags.PLAYER))
            {

                if (Vector3.Distance(transform.position, other.transform.position) > sphereCollider.radius * scaleValue)
                {
                    Damage(other.gameObject.GetComponent<IDamageable>());
                }
            }
    }


    private void Update()
    {
        ScalePulse();

        if (scaleValue >= maxScale)
        {
            gameObject.SetActive(false);
        }
    }


    private void ScalePulse()
    {
        scaleValue += Time.deltaTime * scaleRate;
        transform.localScale = new Vector3(scaleValue, 0.1f, scaleValue);
    }

    private void OnDisable()
    {
        scaleValue = 0.0f;
        transform.localScale = new Vector3(scaleValue, 0.1f, scaleValue);
    }

    public void Damage(IDamageable damageable)
    {
        if (damageable != null)
        {
            damager.Set(10.0f, DamagerType.BlasterBullet, Vector3.zero);
            damageable.TakeDamage(damager);
        }
    }


}
