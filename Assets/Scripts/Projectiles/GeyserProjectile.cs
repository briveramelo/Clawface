using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeyserProjectile : MonoBehaviour {

    #region Public fields    
    #endregion

    #region Serialized Unity Inspector fields
    [SerializeField] private VFXBlasterShoot vfx;
    [SerializeField] private ProjectileProperties projectileProperties;
    [SerializeField] private float lifeTime;
    #endregion

    #region Private Fields
    #endregion

    #region Unity Lifecycle
    // Use this for initialization
    void Start () {
		
	}

    void OnEnable()
    {
        StartCoroutine(WaitToDisable());
    }

    IEnumerator WaitToDisable()
    {
        yield return new WaitForSeconds(lifeTime);
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update () {
        if (gameObject.activeSelf)
        {
            vfx.Emit();
        }
	}    

    void OnDisable()
    {
        transform.localScale = Vector3.one;
    }

    void OnTriggerEnter(Collider other){
        if((other.CompareTag(Strings.Tags.ENEMY) || other.CompareTag(Strings.Tags.PLAYER))){
            if (projectileProperties.shooterInstanceID != other.gameObject.GetInstanceID()) {
                IDamageable damageable = other.GetComponent<IDamageable>();
                if (damageable!=null) {
                    damageable.TakeDamage(projectileProperties.damage);
                }
            }
        }
    }
    #endregion

    #region Public Methods
    public void SetShooterProperties(ProjectileProperties projectileProperties) {
        this.projectileProperties = projectileProperties;
    }
#endregion

#region Private Methods
#endregion

#region Private Structures
#endregion

}
