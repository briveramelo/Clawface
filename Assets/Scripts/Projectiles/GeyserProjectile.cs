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
    #endregion

    #region Unity Lifecycle

    // Use this for initialization
    void Start () {
		
	}

    void OnEnable()
    {
        StartCoroutine(WaitToDisable());
    }

    

    // Update is called once per frame
    void Update () {
       
	}    

    void OnDisable()
    {
        transform.localScale = Vector3.one;
    }

    void OnTriggerEnter(Collider other){       
        if (projectileProperties.shooterInstanceID != other.gameObject.GetInstanceID()) {
            IDamageable damageable = other.GetComponent<IDamageable>();
            if (damageable!=null) {
                damager.Set(projectileProperties.damage, DamagerType.Geyser, Vector3.down);
                damageable.TakeDamage(damager);
            }
            IMovable moveable = other.GetComponent<IMovable>();
            if(moveable != null)
            {
                moveable.AddDecayingForce(Vector3.up * upForce * transform.localScale.magnitude);
            }
        }
    }
    #endregion

    #region Public Methods
    public void SetProjectileProperties(ProjectileProperties projectileProperties) {
        this.projectileProperties = projectileProperties;
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
