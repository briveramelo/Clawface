using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeyserShield : MonoBehaviour {

    #region Public fields
    #endregion

    #region Serialized Unity Inspector fields
    [SerializeField]
    private float normalLiveTime;
    [SerializeField]
    private float chargeLiveTime;
    [SerializeField]
    private float pushForce;
    [SerializeField]
    private GameObject vfx;
    #endregion

    #region Private Fields
    private bool isCharged;
    private ProjectileProperties projectileProperties;
    #endregion

    #region Unity Lifecycle   

    void OnDisable()
    {
        isCharged = false;
        vfx.SetActive(false);
    }

    // Use this for initialization
    void Start () {
        isCharged = false;
    }
	
	// Update is called once per frame
	void Update () {
        
    }

    void OnTriggerEnter(Collider other)
    {        
        if (other.tag == Strings.Tags.ENEMY)
        {
            IMovable moveable = other.gameObject.GetComponent<IMovable>();
            if (isCharged)
            {
                other.gameObject.GetComponent<IStunnable>().Stun();                
            }
            if (moveable != null)
            {
                moveable.AddDecayingForce(-moveable.GetForward() * pushForce);
            }
        }else if(other.tag == Strings.Tags.PROJECTILE)
        {           
            other.gameObject.transform.Rotate(Vector3.right, 180f);
            other.gameObject.GetComponent<BlasterBullet>().SetWielderInstanceID(projectileProperties.shooterInstanceID);
        }
    }
#endregion

#region Public Methods
    public void Fire()
    {
        vfx.SetActive(true);
        StartCoroutine(WaitForSweetDeath(normalLiveTime));
    }

    public void FireCharged()
    {
        vfx.SetActive(true);
        isCharged = true;
        StartCoroutine(WaitForSweetDeath(chargeLiveTime));
    }

    public void SetProjectileProperties(ProjectileProperties projectileProperties)
    {
        this.projectileProperties = projectileProperties;
    }
    #endregion

    #region Private Methods
    private IEnumerator WaitForSweetDeath(float time)
    {
        yield return new WaitForSeconds(time);
        gameObject.SetActive(false);
    }
#endregion

#region Private Structures
#endregion

}
