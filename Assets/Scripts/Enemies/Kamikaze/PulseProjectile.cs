using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Turing.VFX;

public class PulseProjectile : MonoBehaviour
{

    [SerializeField] ParticleSystem ps;
    [SerializeField] private SphereCollider sphereCollider;
    [SerializeField] Material ringMaterial;
    [SerializeField] Renderer ringRenderer;
    [SerializeField] AnimationCurve ringOpacity;
    [SerializeField] ParticleSystem sparkPS;
    [SerializeField] AudioSource audioSource;
    Material materialInstance;
    Camera mainCamera;
    GameObject player;

    private Damager damager = new Damager();
    private float scaleValue = 0.0f;
    private float scaleRate;
    private float maxScale;
    private float damage;
    private float offset = 0.5f;

    private void OnEnable()
    {
        materialInstance = new Material(ringMaterial);
        materialInstance.CopyPropertiesFromMaterial(ringMaterial);
        ringRenderer.material = materialInstance;
        scaleValue = 0.0f;
        transform.localScale = new Vector3(scaleValue, 0.1f, scaleValue);
    }

    private void OnTriggerStay(Collider other)
    {
            if (other.gameObject.CompareTag(Strings.Tags.PLAYER))
            {

                if (Vector3.Distance(transform.position, other.transform.position) < (sphereCollider.radius * scaleValue + offset) && Vector3.Distance(transform.position, other.transform.position) > sphereCollider.radius * scaleValue - offset)
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
        VFXOneOff vfx = ps.GetComponent<VFXOneOff>();
        vfx.Stop();
        ParticleSystem.MainModule main = ps.main;
        float newDuration = (maxScale / scaleRate) - main.startLifetime.constantMax;
        main.duration = newDuration;
        vfx.Play();
}

    public void SetRenderQueue (int queue)
    {
        ringRenderer.material.renderQueue = queue;
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
        props.SetFloat("_Radius", scaleValue * 8.91f);
        props.SetColor("_Color", color);
        ringRenderer.SetPropertyBlock(props);

        ParticleSystem.ShapeModule sparkShape = sparkPS.shape;
        sparkShape.scale = 8.91f * scaleValue * Vector3.one;

        audioSource.panStereo = GetPanPosition();
        audioSource.volume = 1.0f - Mathf.Abs(audioSource.panStereo);
    }

    float GetPanPosition () {
        if (mainCamera == null || !mainCamera.isActiveAndEnabled)
            mainCamera = Camera.main;

        if (!player)
            player = GameObject.FindGameObjectWithTag(Strings.Tags.PLAYER);

        if (!player) {
            return 0f;
        }
        float distToPlayer = Vector3.Distance (player.transform.position, transform.position);
        Vector3 closestPointToPlayer = Vector3.Lerp (transform.position, player.transform.position, 
            (scaleValue * 8.91f / (distToPlayer + 0.01f)));

        Debug.DrawLine (closestPointToPlayer, player.transform.position);

        Vector3 screenPos = mainCamera.WorldToViewportPoint(closestPointToPlayer);
        screenPos = screenPos - Vector3.one + 2.0f * screenPos;
        return screenPos.x / 2.0f;
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
