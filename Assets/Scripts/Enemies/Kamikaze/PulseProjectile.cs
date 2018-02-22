using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PulseProjectile : MonoBehaviour
{

    [SerializeField] ParticleSystem ps;
    [SerializeField] private SphereCollider sphereCollider;

    private Damager damager = new Damager();
    private float scaleValue = 0.0f;
    private float scaleRate;
    private float maxScale;
    private float damage;

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

    public void SetPulseProjectileStats(float newScaleRate, float newMaxScale, float newDamage)
    {
        scaleRate = newScaleRate;
        maxScale = newMaxScale;
        damage = newDamage;

}


    private void ScalePulse()
    {
        scaleValue += Time.deltaTime * scaleRate;
        transform.localScale = new Vector3(scaleValue, 0.1f, scaleValue);

        // Adjust particle system scale
        ParticleSystem.ShapeModule shape = ps.shape;
        //shape.radius += Time.deltaTime * scaleRate;
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
            damager.Set(damage, DamagerType.BlasterBullet, Vector3.zero);
            damageable.TakeDamage(damager);
        }
    }


}
