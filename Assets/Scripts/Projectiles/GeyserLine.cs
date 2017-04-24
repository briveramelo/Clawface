using MovementEffects;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GeyserLine : MonoBehaviour {

    #region Public fields
    #endregion

    #region Serialized Unity Inspector fields
    [SerializeField]
    private float damageMultiplier;
    #endregion

    #region Private Fields
    private Vector3 initialScale;
    private ParticleSystem myParticleSystem;
    private ProjectileProperties projectileProperties;
    private Damager damager;
    #endregion

#region Unity Lifecycle
    void Awake()
    {
        myParticleSystem = GetComponent<ParticleSystem>();
        initialScale = transform.localScale;
        projectileProperties = new ProjectileProperties();
        damager = new Damager();
    }

    void OnDisable()
    {
        transform.localScale = initialScale;
    }

	// Use this for initialization
	void Start () {
        
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerEnter(Collider other)
    {
        if ((other.CompareTag(Strings.Tags.ENEMY) || other.CompareTag(Strings.Tags.PLAYER)))
        {
            if (projectileProperties.shooterInstanceID != other.gameObject.GetInstanceID())
            {
                IDamageable damageable = other.GetComponent<IDamageable>();
                if (damageable != null)
                {
                    damager.Set(projectileProperties.damage * damageMultiplier, DamagerType.Geyser, Vector3.up);
                    damageable.TakeDamage(damager);
                }
            }
        }
    }
#endregion

#region Public Methods
    public void Fire(float intensity, float liveTime, ProjectileProperties properties)
    {
        projectileProperties = properties;
        intensity = Mathf.Clamp(intensity, 0f, 1f);
        transform.localScale = initialScale * intensity;
        myParticleSystem.Play();
        Timing.RunCoroutine(WaitForParticleSystem(liveTime));
    }
    #endregion

    #region Private Methods
    private IEnumerator<float> WaitForParticleSystem(float liveTime)
    {
        yield return Timing.WaitForSeconds(liveTime);
        gameObject.SetActive(false);
        yield return 0;
    }
    #endregion

    #region Private Structures
    #endregion

}
