using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeyserProjectile : MonoBehaviour {

    #region Public fields    
    #endregion

    #region Serialized Unity Inspector fields
    [SerializeField] private float lifeTime;
    [SerializeField]
    private float upForce;
    #endregion

    #region Private Fields
    private ProjectileProperties projectileProperties;
    private Damager damager=new Damager();
    private bool isPlayer;
    private List<GameObject> objectsHitThisGush=new List<GameObject>();
    #endregion

    #region Unity Lifecycle

    // Use this for initialization
    void Start () {
		
	}

    void OnEnable()
    {
        StartCoroutine(WaitToDisable());
        objectsHitThisGush.Clear();
    }

    

    // Update is called once per frame
    void Update () {
       
	}    

    void OnDisable()
    {
        transform.localScale = Vector3.one;
    }


    void OnTriggerEnter(Collider other){
        if (!objectsHitThisGush.Contains(other.gameObject)) {
            if (projectileProperties.shooterInstanceID != other.gameObject.GetInstanceID()) {
                objectsHitThisGush.Add(other.gameObject);
                IDamageable damageable = other.GetComponent<IDamageable>();
                if (damageable!=null) {                    
                    damager.Set(projectileProperties.damage, DamagerType.Geyser, Vector3.down);

					if (isPlayer)
                	{
                    	AnalyticsManager.Instance.AddModDamage(ModType.Geyser, damager.damage);

                    	if (damageable.GetHealth() - damager.damage <= 0.01f)
                    	{
                        	AnalyticsManager.Instance.AddModKill(ModType.Geyser);
                    	}
                	}

                    
                    damageable.TakeDamage(damager);
                }
                IMovable moveable = other.GetComponent<IMovable>();
                if(moveable != null)
                {
                    moveable.AddDecayingForce(Vector3.up * upForce * transform.localScale.magnitude);
                }
            }
        }
    }
    #endregion

    #region Public Methods
    public void SetProjectileProperties(ProjectileProperties projectileProperties) {
        this.projectileProperties = projectileProperties;
    }

    public void SetShooterType(bool isPlayer)
    {
        this.isPlayer = isPlayer;
    }
    #endregion

    #region Private Methods
    private IEnumerator WaitToDisable()
    {
        yield return new WaitForSeconds(lifeTime);
        gameObject.SetActive(false);
    }
    #endregion

    #region Private Structures
    #endregion

}
