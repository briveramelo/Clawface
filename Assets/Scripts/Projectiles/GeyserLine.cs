using MEC;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GeyserLine : RoutineRunner {

    #region Public fields
    #endregion

    #region Serialized Unity Inspector fields
    [SerializeField]
    private float damageMultiplier;
    [SerializeField]
    private float upForce;
    #endregion

    #region Private Fields
    private Vector3 initialScale;
    private ParticleSystem myParticleSystem;
    private ProjectileProperties projectileProperties = new ProjectileProperties();
    private Damager damager;
    private bool isPlayer;
    #endregion

#region Unity Lifecycle
    void Awake()
    {
        myParticleSystem = GetComponent<ParticleSystem>();
        initialScale = transform.localScale;
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
                IMovable moveable = other.GetComponent<IMovable>();
                if (damageable != null)
                {
                    damager.Set(projectileProperties.damage * damageMultiplier, DamagerType.Geyser, Vector3.down);

                    // Shooter is player

                    AnalyticsManager.Instance.AddModDamage(ModType.Geyser, damager.damage);

                    if (damageable.GetHealth() - damager.damage <= 0.01f)
                    {
                        AnalyticsManager.Instance.AddModKill(ModType.Geyser);
                    }



                    damageable.TakeDamage(damager);
                }
                if (moveable != null)
                {
                    moveable.AddDecayingForce(Vector3.up * upForce);
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
        Timing.RunCoroutine(WaitForParticleSystem(liveTime), coroutineName);
    }

    public void SetShooterType(bool isPlayer)
    {
        this.isPlayer = isPlayer;
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
