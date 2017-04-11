using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeyserProjectile : MonoBehaviour {

    #region Public fields
    [HideInInspector]
    public float damage;
    #endregion

    #region Serialized Unity Inspector fields
    [SerializeField]
    private VFXBlasterShoot vfx;
    [SerializeField]
    private float lifeTime;
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

    void OnTriggerEnter(Collider other)
    {
        print(other.tag);
        if(other.tag == Strings.Tags.ENEMY)
        {
            other.GetComponent<IDamageable>().TakeDamage(damage);
        }
    }
#endregion

#region Public Methods
#endregion

#region Private Methods
#endregion

#region Private Structures
#endregion

}
