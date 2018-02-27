using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PulseProjectile : MonoBehaviour
{

    [SerializeField] ParticleSystem ps;
    [SerializeField] private SphereCollider sphereCollider;
    [SerializeField] Material ringMaterial;
    [SerializeField] Renderer ringRenderer;
    [SerializeField] AnimationCurve ringOpacity;
    Material materialInstance;

    private Damager damager = new Damager();
    private float scaleValue = 0.0f;
    private float scaleRate;
    private float maxScale;
    private float damage;

    private void OnEnable()
    {
        materialInstance = new Material(ringMaterial);
        materialInstance.CopyPropertiesFromMaterial(ringMaterial);
        ringRenderer.material = materialInstance;
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
        float ringWidth = materialInstance.GetFloat("_RingWidth");
        //materialInstance.SetFloat("_Radius", scaleValue * 8.91f - ringWidth);
        Color color = materialInstance.GetColor("_Color");
        color.a = ringOpacity.Evaluate(scaleValue / maxScale);
        //materialInstance.SetColor("_Color", color);

        MaterialPropertyBlock props = new MaterialPropertyBlock();
        props.SetFloat("_Radius", scaleValue * 8.91f - ringWidth);
        props.SetColor("_Color", color);
        ringRenderer.SetPropertyBlock(props);
    }

    private void OnDisable()
    {
        scaleValue = 0.0f;
        transform.localScale = new Vector3(scaleValue, 0.1f, scaleValue);
        materialInstance.SetFloat("_Radius", 0.0f);
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
